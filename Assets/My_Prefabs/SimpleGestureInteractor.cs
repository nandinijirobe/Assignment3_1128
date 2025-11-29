using System.Diagnostics;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class SimpleGestureInteractor : MonoBehaviour
{
    public float distanceFromCamera = 0.5f; // how far in front of the camera the object will hover

    private bool isFollowing = false;        // is the object currently following the camera?
    private Camera arCamera;

    void Awake()
    {
        // Get the AR camera in the scene
        arCamera = Camera.main;
        if (arCamera == null)
        {
            UnityEngine.Debug.LogError("AR Camera not found! Make sure your scene has an AR Camera.");
        }
    }

    void Update()
    {
        // Detect tap on this object
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            Touch touch = Input.GetTouch(0);
            Ray ray = arCamera.ScreenPointToRay(touch.position);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.gameObject == gameObject)
                {
                    // Toggle follow state
                    isFollowing = !isFollowing;
                }
            }
        }

        // If following, move the object in front of the camera
        if (isFollowing)
        {
            Vector3 targetPosition = arCamera.transform.position + arCamera.transform.forward * distanceFromCamera;
            transform.position = targetPosition;

            // Optional: rotate to face the camera
            //transform.LookAt(arCamera.transform);
        }
    }
}
