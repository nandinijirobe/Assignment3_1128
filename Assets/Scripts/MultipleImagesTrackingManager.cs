// Tutorial used: https://www.youtube.com/watch?v=7GiDoWviQEM

using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;


public class MultipleImagesTrackingManager : MonoBehaviour
{

    [SerializeField] private GameObject[] treesToSpawn;
    [SerializeField] private GameObject[] buildingsToSpawn;
    [SerializeField] private GameObject[] streetDecorToSpawn;
    
    public Text statusText;
    private bool enableMarkerTracking = false;


    private ARTrackedImageManager _aRTrackedImageManager; // Reference to ARTrackedImageManager
    private Dictionary<string, List<GameObject>> _arObjects = new Dictionary<string, List<GameObject>>(); // contains image name and spawned prefab

    public GameObject XROrigin;
    private PlaneSelection planeSelectionScript;

    // Get reference to ARTrackedImageManager
    private void Awake()
    {
        _aRTrackedImageManager = GetComponent<ARTrackedImageManager>();
        statusText = GameObject.FindObjectOfType<Text>();
    }

    private void Start()
    {
        // Listen to tracked images changed event
        _aRTrackedImageManager.trackedImagesChanged += OnTrackedImageChanged;
        planeSelectionScript = XROrigin.GetComponent<PlaneSelection>();

        // Spawn prefabs for each tracked image and hide them initially
        _arObjects["trees"] = new List<GameObject>();
        _arObjects["buildings"] = new List<GameObject>();
        _arObjects["streetDecor"] = new List<GameObject>();

        for (int i = 0; i < treesToSpawn.Length; i++)
        {

            Vector3 prefabPosition = new Vector3(i*2.0f, 0, 0); // set initial position
            GameObject newTree = Instantiate(treesToSpawn[i], prefabPosition, Quaternion.identity); // instantiate prefab
            newTree.name = treesToSpawn[i].name + i.ToString(); // set name
            newTree.SetActive(false); // Hide initially
            _arObjects["trees"].Add(newTree); // add it to respective list
        }

        for (int i = 0; i < buildingsToSpawn.Length; i++)
        {
            Vector3 prefabPosition = new Vector3(i * 2.0f, 0, 0); // set initial position
            GameObject newBuilding = Instantiate(buildingsToSpawn[i], prefabPosition, Quaternion.identity); // instantiate prefab
            newBuilding.name = buildingsToSpawn[i].name + i.ToString(); // set name
            newBuilding.SetActive(false); // Hide initially
            _arObjects["buildings"].Add(newBuilding); // add it to respective list
        }

        for (int i = 0; i < streetDecorToSpawn.Length; i++)
        {
            Vector3 prefabPosition = new Vector3(i * 2.0f, 0, 0); // set initial position
            GameObject newStreetDecor = Instantiate(streetDecorToSpawn[i], prefabPosition, Quaternion.identity); // instantiate prefab
            newStreetDecor.name = streetDecorToSpawn[i].name + i.ToString(); // set name
            newStreetDecor.SetActive(false); // Hide initially
            _arObjects["streetDecor"].Add(newStreetDecor); // add it to respective list
        }

    }

    private void OnDestroy()
    {
        // Unsubscribe from tracked images changed event
        _aRTrackedImageManager.trackedImagesChanged -= OnTrackedImageChanged;

    }


    private void Update()
    {
        if (planeSelectionScript.selectedPlane != null && enableMarkerTracking == false)
        {
            statusText.text = "Search for markers!";
            _aRTrackedImageManager.enabled = true;
            enableMarkerTracking = true;
        }
        else if (planeSelectionScript.selectedPlane == null)
        {
            statusText.text = "Please select a plane.";
            _aRTrackedImageManager.enabled = false;
        }
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
            for (int i = 0; i < _arObjects[trackedImage.referenceImage.name].Count; i++)
            {
                GameObject obj = _arObjects[trackedImage.referenceImage.name][i];
                if (obj.activeSelf)
                {
                    obj.SetActive(false);
                }
            }
        }

    }

    private void UpdateTrackedImage(ARTrackedImage trackedImage)
    {
        // Only spawn the object once, when the image is first detected
        if (trackedImage.trackingState == TrackingState.Tracking)
        {
            for ( int i = 0; i < _arObjects[trackedImage.referenceImage.name].Count; i++)
            {
                GameObject obj = _arObjects[trackedImage.referenceImage.name][i];
                SimpleGestureInteractor objScript = obj.GetComponent<SimpleGestureInteractor>();

                // Position the object at the tracked image position
                if (!obj.activeSelf)
                {
                    obj.SetActive(true);
                    obj.transform.position = trackedImage.transform.position + new Vector3(-0.15f + (i * 0.15f), 0, 0); // Offset each object
                    obj.transform.rotation = trackedImage.transform.rotation; // keep same rotation as tracked image

                // Reset fallen state prefab
                } else if (obj.activeSelf == true && objScript.fallen == true) {
                    obj.transform.position = trackedImage.transform.position + new Vector3(-0.15f + (i * 0.15f), 0, 0); // Offset each object
                    obj.transform.rotation = trackedImage.transform.rotation; // keep same rotation as tracked image
                    objScript.fallen = false;
                }
            }

        }

    }

}