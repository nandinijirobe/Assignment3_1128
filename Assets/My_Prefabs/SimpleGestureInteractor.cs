// Code from Unity AR Foundation Class Tutorial 

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class SimpleGestureInteractor : MonoBehaviour
{
    [Header("Scene Refs")]
    public ARRaycastManager raycastManager;
    public Camera arCamera;

    [Header("Tuning")]
    public float dragSmoothing = 10f;
    public float rotateSpeed = 0.2f;
    public float minScale = 0.1f;
    public float maxScale = 3f;

    private bool _selected;
    private Vector3 _targetPos;
    private static List<ARRaycastHit> hits = new List<ARRaycastHit>();

    private float _initialPinchDist;
    private Vector3 _initialScale;
    private float _lastTwoFingerAngle;


    void Awake()
    {
        if (raycastManager == null)
        {
            raycastManager = FindObjectOfType<ARRaycastManager>();
        }
        if (arCamera == null)
        {
            arCamera = Camera.main;
        }
    }

    void Start()
    {
        _targetPos = transform.position;
    }

    void Update()
    {
        if (Input.touchCount == 0)
            return;

        // ONE FINGER (tap + drag)
   
        if (Input.touchCount == 1)
        {
            Touch t0 = Input.GetTouch(0);

            // Select on tap if touching object
            if (t0.phase == TouchPhase.Began)
            {
                Ray ray = arCamera.ScreenPointToRay(t0.position);

                if (Physics.Raycast(ray, out RaycastHit hit) && hit.transform == transform)
                {
                    _selected = true;
                }
                else
                {
                    _selected = false;
                }
            }

            // Drag on plane
            if (_selected && (t0.phase == TouchPhase.Moved || t0.phase == TouchPhase.Stationary))
            {
                if (raycastManager.Raycast(t0.position, hits, TrackableType.PlaneWithinPolygon))
                {
                    Pose pose = hits[0].pose;
                    _targetPos = pose.position;
                }
            }
        }

        // TWO FINGER (pinch + rotate)

        else if (Input.touchCount == 2 && _selected)
        {
            Touch t0 = Input.GetTouch(0);
            Touch t1 = Input.GetTouch(1);

            float currentDist = Vector2.Distance(t0.position, t1.position);

            // Begin two-finger gesture
            if (t0.phase == TouchPhase.Began || t1.phase == TouchPhase.Began)
            {
                _initialPinchDist = currentDist;
                _initialScale = transform.localScale;
                _lastTwoFingerAngle = AngleBetweenTouches(t0.position, t1.position);
            }
            else
            {
                // Pinch scale
                if (_initialPinchDist > 0.001f)
                {
                    float scaleFactor = currentDist / _initialPinchDist;
                    Vector3 targetScale = _initialScale * scaleFactor;

                    float clamped = Mathf.Clamp(targetScale.x, minScale, maxScale);
                    transform.localScale = new Vector3(clamped, clamped, clamped);
                }

                // Rotate using two-finger twist
                float currentAngle = AngleBetweenTouches(t0.position, t1.position);
                float deltaAngle = Mathf.DeltaAngle(_lastTwoFingerAngle, currentAngle);

                transform.Rotate(Vector3.up, deltaAngle * rotateSpeed, Space.World);

                _lastTwoFingerAngle = currentAngle;
            }
        }

        // Smooth follow to reduce jitter during drag 
        transform.position = Vector3.Lerp(transform.position, _targetPos, Time.deltaTime * dragSmoothing);
    }

    // Helper: angle between two touches
    float AngleBetweenTouches(Vector2 a, Vector2 b)
    {
        return Mathf.Atan2(b.y - a.y, b.x - a.x) * Mathf.Rad2Deg;
    }
}
