// Simplified marker spawner - spawns objects once when marker is detected
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class SimpleMarkerSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] prefabsToSpawn; // List of prefabs to spawn for each tracked image

    private ARTrackedImageManager _aRTrackedImageManager;
    private Dictionary<string, bool> _alreadySpawned = new Dictionary<string, bool>(); // Track which markers have spawned objects

    private void Awake()
    {
        _aRTrackedImageManager = GetComponent<ARTrackedImageManager>();
    }

    private void OnEnable()
    {
        _aRTrackedImageManager.trackedImagesChanged += OnTrackedImageChanged;
    }

    private void OnDisable()
    {
        _aRTrackedImageManager.trackedImagesChanged -= OnTrackedImageChanged;
    }

    private void OnTrackedImageChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        // Only care about newly detected markers
        foreach (ARTrackedImage trackedImage in eventArgs.added)
        {
            SpawnObjectAtMarker(trackedImage);
        }
    }

    private void SpawnObjectAtMarker(ARTrackedImage trackedImage)
    {
        string markerName = trackedImage.referenceImage.name;

        // Check if we've already spawned for this marker
        if (_alreadySpawned.ContainsKey(markerName) && _alreadySpawned[markerName])
        {
            return; // Already spawned, don't spawn again
        }

        // Find the matching prefab by name
        GameObject prefabToSpawn = null;
        foreach (GameObject prefab in prefabsToSpawn)
        {
            if (prefab.name == markerName)
            {
                prefabToSpawn = prefab;
                break;
            }
        }

        // Spawn the object at the marker's position
        if (prefabToSpawn != null)
        {
            GameObject spawnedObject = Instantiate(
                prefabToSpawn,
                trackedImage.transform.position,
                trackedImage.transform.rotation
            );

            spawnedObject.name = markerName + "_spawned";
            _alreadySpawned[markerName] = true;

            UnityEngine.Debug.Log($"Spawned {spawnedObject.name} at marker position");
        }
        else
        {
            UnityEngine.Debug.LogWarning($"No prefab found matching marker name: {markerName}");
        }
    }

    // Optional: Reset to allow re-spawning (for testing or reset button)
    public void ResetSpawning()
    {
        _alreadySpawned.Clear();
        UnityEngine.Debug.Log("Spawning reset - markers can spawn objects again");
    }
}