using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.UI;
using System.Collections;
//using System.Runtime.ExceptionServices;


public class SimpleGestureInteractor : MonoBehaviour
{
    public float distanceFromCamera = 0.5f; // how far in front of the camera the object will hover

    private bool isFollowing = false;        // is the object currently following the camera?
    private Camera arCamera;
    private bool isTouchingPlane = false;
    public Text statusText;
    private Rigidbody rb;
    private ARPlane touchedPlane = null;
    private Coroutine resetCoroutine = null;


    // Information other files will need
    public Vector3 initalPosition = Vector3.zero;
    public bool fallen = false;

    void Awake()
    {
        // Get the AR camera in the scene
        arCamera = Camera.main;
        statusText = GameObject.FindObjectOfType<Text>();
        rb = GetComponent<Rigidbody>();

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

                    if (!isFollowing) // If object is no longer following the camera (meaning it was released)
                    {
                        if (!isTouchingPlane) // If not touching a plane, let it fall
                        {
                            statusText.text = "Falling...";
                            rb.constraints &= ~RigidbodyConstraints.FreezePositionY; // unfreeze Y contraint so it fall vertically
                            rb.useGravity = true; // enable gravity so it can fall

                            // Stop any existing reset coroutine and start a new one
                            if (resetCoroutine != null)
                            {
                                // Stop any existing reset
                                StopCoroutine(resetCoroutine);
                            }

                            // Call goToBackInitalPosition and start counting to 3 seconds
                            resetCoroutine = StartCoroutine(goToBackInitalPosition());
                        }
                        else
                        { // If touching a plane, make it a child of the plane
                            rb.constraints = RigidbodyConstraints.FreezeAll;
                            transform.SetParent(touchedPlane.transform);
                        }

                    } 
                }
            }

        }


         // If following, move the object in front of the camera
         if (isFollowing){
            Vector3 targetPosition = arCamera.transform.position + arCamera.transform.forward * distanceFromCamera;
            transform.position = targetPosition;
        }
    }

    private IEnumerator goToBackInitalPosition() {
        yield return new WaitForSeconds(3f); // let object fall for 3 seconds

        // Reset so it can't keep falling
        isTouchingPlane = false;
        touchedPlane = null;
        rb.useGravity = false;
        fallen = true;
        rb.constraints |= RigidbodyConstraints.FreezePositionY; // freeze Y position again
        statusText.text = "Replaced";
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("HorizontalPlane")) {
            statusText.text = "Ready to Place";
            touchedPlane = collision.collider.GetComponent<ARPlane>();
            isTouchingPlane = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.CompareTag("HorizontalPlane"))
        {
            statusText.text = "Do NOT Place";
            isTouchingPlane = false;
            touchedPlane = null;
        }
    }
    
}
