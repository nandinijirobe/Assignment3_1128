using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;

public class PlaneSelection : MonoBehaviour
{
    public ARRaycastManager raycastManager;
    public ARPlaneManager planeManager;
    public Camera arCamera;
    public Material selectedMaterial;

    public ARPlane selectedPlane = null;
    private GameObject virtualProxy = null;

    private static List<ARRaycastHit> hits = new List<ARRaycastHit>();

    void Awake()
    {
        raycastManager = GetComponent<ARRaycastManager>();
        planeManager = GetComponent<ARPlaneManager>();
        arCamera = GetComponentInChildren<Camera>();
    }

    void Update()
    {
        // Detect tap
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            Touch touch = Input.GetTouch(0);
            Ray ray = arCamera.ScreenPointToRay(touch.position);

            // Try a physics raycast (works because planes have colliders)
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
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
        //plane.GetComponent<MeshRenderer>().material.color = Color.green;
        plane.GetComponent<MeshRenderer>().material = selectedMaterial;

        // Hide all OTHER planes
        foreach (ARPlane p in planeManager.trackables)
        {
            if (p != plane)
            {
                p.gameObject.SetActive(false);
            }
        }

        // Stop all plane detection
        planeManager.requestedDetectionMode = PlaneDetectionMode.None;

        virtualProxy = plane.gameObject;


    }

    public bool IsPlaneSelected()
    {
        return planeManager.requestedDetectionMode == PlaneDetectionMode.None;
    }

    public ARPlane GetSelectedPlane()
    {
        return selectedPlane;
    }

    public GameObject GetVirtualProxy()
    {
        return virtualProxy;
    }
}
