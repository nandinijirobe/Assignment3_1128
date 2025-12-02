using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;

public class PlaneSelection : MonoBehaviour
{
    public ARPlaneManager planeManager;
    public Camera arCamera;

    // Selected Plane and Material
    public Material selectedMaterial;
    public ARPlane selectedPlane = null;

    void Awake()
    {
        planeManager = GetComponent<ARPlaneManager>();
        arCamera = GetComponentInChildren<Camera>();
    }

    void Update()
    {
        // Detect tap from user
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            Touch touch = Input.GetTouch(0);
            Ray ray = arCamera.ScreenPointToRay(touch.position); // Makes a ray from the touch position

            // Cast the ray to see if it hits any AR planes
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                // If we hit a plane, select it
                ARPlane selectedPlane = hit.collider.GetComponent<ARPlane>();
               
                if (selectedPlane != null)
                {
                    SelectPlane(selectedPlane);
                }
            }
        }
    }

    void SelectPlane(ARPlane plane)
    {
        selectedPlane = plane;

        // Turn the tapped plane green
        plane.GetComponent<MeshRenderer>().material = selectedMaterial;

        // Hide all other planes
        foreach (ARPlane p in planeManager.trackables)
        {
            if (p != plane)
            {
                p.gameObject.SetActive(false);
            }
        }

        // Stop all plane detection
        planeManager.requestedDetectionMode = PlaneDetectionMode.None;
    }

}
