using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;


public class PlaneSelection: MonoBehaviour
{
    [Header("Materials")]
    [SerializeField] private Material selectedPlaneMaterial; // This is the green material for the selected plane

    [Header("Settings")]
    [SerializeField] private float virtualProxySize = 1.0f; // Size of the virtual proxy cube

    private ARRaycastManager raycastManager; // This is the laser that keeps track of where we touch
    private ARPlaneManager planeManager; // 
    private ARPlane selectedPlane;
    private GameObject virtualProxy;
    private bool planeSelected = false;

    private List<ARRaycastHit> hits = new List<ARRaycastHit>();

    void Awake()
    {
        raycastManager = GetComponent<ARRaycastManager>();
        planeManager = GetComponent<ARPlaneManager>();
    }

    void Start()
    {
        // Check if planes are being detected
        UnityEngine.Debug.Log($"Plane Manager enabled: {planeManager.enabled}");
        UnityEngine.Debug.Log($"Number of tracked planes at start: {planeManager.trackables.count}");
    }

    void Update()
    {
        // Only allow plane selection if no plane has been selected yet
        if (!planeSelected)
        {
            Vector2 inputPosition = Vector2.zero;
            bool inputDetected = false;

            // Check for touch input (on device)
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Began)
                {
                    inputPosition = touch.position;
                    inputDetected = true;
                    UnityEngine.Debug.Log($"Touch detected at position: {inputPosition}");
                }
            }
            // Check for mouse input (in Unity Editor)
            else if (Input.GetMouseButtonDown(0))
            {
                inputPosition = Input.mousePosition;
                inputDetected = true;
                UnityEngine.Debug.Log($"Mouse click detected at position: {inputPosition}");
            }

            if (inputDetected)
            {
                UnityEngine.Debug.Log($"Performing raycast from position: {inputPosition}");

                // Perform raycast to detect planes
                if (raycastManager.Raycast(inputPosition, hits, TrackableType.PlaneWithinPolygon))
                {
                    UnityEngine.Debug.Log($"Raycast hit {hits.Count} planes");

                    // Get the plane that was hit
                    ARRaycastHit hit = hits[0];
                    UnityEngine.Debug.Log($"Hit trackableId: {hit.trackableId}");

                    ARPlane hitPlane = planeManager.GetPlane(hit.trackableId);

                    if (hitPlane != null)
                    {
                        UnityEngine.Debug.Log($"Valid plane found: {hitPlane.name}");
                        SelectPlane(hitPlane, hit.pose.position);
                    }
                    else
                    {
                        UnityEngine.Debug.LogWarning("hitPlane is null - couldn't get plane from trackableId");
                    }
                }
                else
                {
                    UnityEngine.Debug.LogWarning("Raycast did not hit any planes");
                }
            }
        }
        else
        {
            // Uncomment below to see that plane selection is blocking further input
            // UnityEngine.Debug.Log("Plane already selected, ignoring input");
        }
    }

    void SelectPlane(ARPlane plane, Vector3 hitPosition)
    {
        selectedPlane = plane;
        planeSelected = true;

        // Change selected plane material to green
        MeshRenderer renderer = plane.GetComponent<MeshRenderer>();
        if (renderer != null && selectedPlaneMaterial != null)
        {
            renderer.material = selectedPlaneMaterial;
        }

        // Create virtual proxy at the hit location
        CreateVirtualProxy(hitPosition, plane.transform.rotation);

        // Hide all other planes
        HideOtherPlanes();

        // Disable plane detection
        planeManager.enabled = false;

        UnityEngine.Debug.Log($"Plane selected! Virtual proxy created at {hitPosition}");

        // Notify other systems that plane is selected (optional)
        // You can add an event here if needed
    }

    void CreateVirtualProxy(Vector3 position, Quaternion rotation)
    {
        // Create a simple cube as virtual proxy (you can customize this)
        virtualProxy = GameObject.CreatePrimitive(PrimitiveType.Cube);
        virtualProxy.name = "VirtualPlaneProxy";
        virtualProxy.transform.position = position;
        virtualProxy.transform.rotation = rotation;
        virtualProxy.transform.localScale = new Vector3(virtualProxySize, 0.01f, virtualProxySize);

        // Make it invisible (or semi-transparent for debugging)
        MeshRenderer proxyRenderer = virtualProxy.GetComponent<MeshRenderer>();
        if (proxyRenderer != null)
        {
            // Option 1: Make it invisible
            proxyRenderer.enabled = false;

            // Option 2: Make it semi-transparent for debugging (comment out line above and uncomment below)
            // Material proxyMat = new Material(Shader.Find("Standard"));
            // proxyMat.color = new Color(0, 1, 0, 0.3f);
            // proxyRenderer.material = proxyMat;
        }

        // Optional: Add a tag for easy reference later
        virtualProxy.tag = "SelectedPlane";
    }

    void HideOtherPlanes()
    {
        // Iterate through all tracked planes and hide those that aren't selected
        foreach (var plane in planeManager.trackables)
        {
            if (plane != selectedPlane)
            {
                plane.gameObject.SetActive(false);
            }
        }
    }

    // Public getters for other scripts to access
    public bool IsPlaneSelected()
    {
        return planeSelected;
    }

    public ARPlane GetSelectedPlane()
    {
        return selectedPlane;
    }

    public GameObject GetVirtualProxy()
    {
        return virtualProxy;
    }

    // Optional: Reset functionality for extra credit
    public void ResetPlaneSelection()
    {
        planeSelected = false;

        // Destroy virtual proxy
        if (virtualProxy != null)
        {
            Destroy(virtualProxy);
        }

        // Re-enable all planes and plane manager
        foreach (var plane in planeManager.trackables)
        {
            plane.gameObject.SetActive(true);
        }

        planeManager.enabled = true;
        selectedPlane = null;

        UnityEngine.Debug.Log("Plane selection reset");
    }
}