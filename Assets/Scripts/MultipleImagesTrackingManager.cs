// Tutorial used: https://www.youtube.com/watch?v=7GiDoWviQEM

using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;


public class MultipleImagesTrackingManager : MonoBehaviour
{
    [SerializeField] private GameObject[] prefabsToSpawn; // List of prefabs to spawn for each tracked image
    private ARTrackedImageManager _aRTrackedImageManager; // Reference to ARTrackedImageManager
    private Dictionary<string, GameObject> _arObjects = new Dictionary<string, GameObject>(); // contains image name and spawned prefab

    // Get reference to ARTrackedImageManager
    private void Awake()
    {
        _aRTrackedImageManager = GetComponent<ARTrackedImageManager>();
        _arObjects = new Dictionary<string, GameObject>();
    }

    // Listen the events related to any change in TrackedImageManager

    private void Start()
    {
        // Listen to tracked images changed event
        _aRTrackedImageManager.trackedImagesChanged += OnTrackedImageChanged;

        // Spawn prefabs for each tracked image and hide them initially
        foreach (GameObject prefab in prefabsToSpawn)
        {
            GameObject newARObject = Instantiate(prefab, Vector3.zero, Quaternion.identity);
            newARObject.name = prefab.name;
            newARObject.SetActive(false); // Hide initially
            _arObjects.Add(newARObject.name, newARObject);
        }

    }

    private void OnDestroy()
    {
        // Unsubscribe from tracked images changed event
        _aRTrackedImageManager.trackedImagesChanged -= OnTrackedImageChanged;

    }


    private void OnTrackedImageChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (ARTrackedImage trackedImage in eventArgs.added)
        {
            // show, hide or position the gameobject based on the tracked image
            UpdateTrackedImage(trackedImage);
        }

        foreach (ARTrackedImage trackedImage in eventArgs.updated)
        {
            // show, hide or position the gameobject based on the tracked image
            UpdateTrackedImage(trackedImage);
        }

        foreach (ARTrackedImage trackedImage in eventArgs.removed)
        {
            // show, hide or position the gameobject based on the tracked image
            _arObjects[trackedImage.referenceImage.name].gameObject.SetActive(false);
        }

    }

    private void UpdateTrackedImage(ARTrackedImage trackedImage)
    {
        // check tracking status of the tracked image
        if (trackedImage.trackingState == TrackingState.Limited || trackedImage.trackingState == TrackingState.None)
        {

            _arObjects[trackedImage.referenceImage.name].gameObject.SetActive(false);
            return;
        }

        // show, hide or position the gameobject based on the tracked image
        if (prefabsToSpawn != null)
        {
            _arObjects[trackedImage.referenceImage.name].SetActive(true);
            _arObjects[trackedImage.referenceImage.name].transform.position = trackedImage.transform.position;
        }
    }

}