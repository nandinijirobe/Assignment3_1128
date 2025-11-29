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
        // Turn the tapped plane green
        plane.GetComponent<MeshRenderer>().material.color = Color.green;

        // Disable the ARPlane component to "freeze" it
        //plane.enabled = false;

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

        //ARPlane planeComponent = plane.GetComponent<ARPlane>();
        //if (planeComponent != null)
        //    Destroy(planeComponent);

        // Optional: also stop showing the green plane mesh after selection
        // plane.gameObject.SetActive(false);
        


        //// Record the plane’s position and rotation
        //Vector3 position = plane.transform.position;
        //Quaternion rotation = plane.transform.rotation;

        //// Hide all other planes
        //foreach (ARPlane p in planeManager.trackables)
        //{
        //    if (p != plane)
        //        p.gameObject.SetActive(false);
        //}

        //// Stop plane detection
        //planeManager.requestedDetectionMode = PlaneDetectionMode.None;

        //// Create a virtual proxy at that location
        //GameObject proxy = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //proxy.transform.position = position;
        //proxy.transform.rotation = rotation;
        //proxy.transform.localScale = new Vector3(0.5f, 0.01f, 0.5f);
        //proxy.GetComponent<MeshRenderer>().material = selectedMaterial;
    }
}
