using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.UI;
using System.Collections;
using System.Runtime.ExceptionServices;


public class SimpleGestureInteractor : MonoBehaviour
{
    public float distanceFromCamera = 0.5f; // how far in front of the camera the object will hover

    private bool isFollowing = false;        // is the object currently following the camera?
    private Camera arCamera;

    private bool isTouchingPlane = false;
    public Text statusText;

    private Rigidbody rb;

    private Vector3 firstPosition = Vector3.zero;

    private ARPlane touchedPlane = null;
    private Coroutine resetCoroutine = null;

    public Button leftButton;
    public Button rightButton;

    private bool listenersAdded = false;

    public Vector3 initalPosition = Vector3.zero;

    public bool fallen = false;



    void Awake()
    {

        //leftButton = GameObject.Find("LeftButton_GameObject").GetComponent<Button>();
        //if (leftButton == null)
        //{
        //    UnityEngine.Debug.LogError("left Buttons not found! Make sure your scene has LeftButton_GameObject and RightButton_GameObject.");
        //}

        //rightButton = GameObject.Find("RightButton_GameObject").GetComponent<Button>();

        //if (leftButton == null)
        //{
        //    UnityEngine.Debug.LogError("right Buttons not found! Make sure your scene has LeftButton_GameObject and RightButton_GameObject.");
        //}

        // Get the AR camera in the scene
        arCamera = Camera.main;
        statusText = GameObject.FindObjectOfType<Text>();
        rb = GetComponent<Rigidbody>();
        firstPosition = transform.position; // store the initial position

        if (arCamera == null)
        {
            UnityEngine.Debug.LogError("AR Camera not found! Make sure your scene has an AR Camera.");
        }

        
    }

    //void Start()
    //{
    //    leftButton.onClick.AddListener(RotateLeft);
    //    rightButton.onClick.AddListener(RotateRight);
    //}


public void RotateLeft()
    {
        statusText.text = "Left button clicked";
        if (isFollowing)
        {
            statusText.text = "Rotating left";
            transform.Rotate(0, -15f, 0); // Rotate left by 15 degrees
        }

    }

    public void RotateRight()
    {
        statusText.text = "Right button clicked";
        if (isFollowing)
        {
            statusText.text = "Rotating Right";
            transform.Rotate(0, 15f, 0); // Rotate left by 15 degrees
        }

    }

    void Update()
    {

        UpdateTouchingUI();
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

                    if (!isFollowing)
                    {
                        if (!isTouchingPlane)
                        {
                            statusText.text = "FALLING OMG";
                            //rb.isKinematic = false; // let physics take over and let it fall down
                            rb.constraints &= ~RigidbodyConstraints.FreezePositionY; // unfreeze Y
                            
                            rb.useGravity = true;
                            if (resetCoroutine != null)
                            {
                                // // Stop any existing reset
                                StopCoroutine(resetCoroutine);
                            }

                            resetCoroutine = StartCoroutine(goToBackInitalPosition());
                        }
                        else {
                            
                            // become child of the plane to stay in place
                            rb.constraints = RigidbodyConstraints.FreezeAll;
                            transform.SetParent(touchedPlane.transform);
                        }

                    } 
                }
            }

        }


        //if (listenersAdded == false && leftButton != null && rightButton != null) { 
        //    statusText.text = "Adding listeners";
        //    leftButton.onClick.AddListener(RotateLeft);
        //    rightButton.onClick.AddListener(RotateRight);
        //    listenersAdded = true;
        //} 

         // If following, move the object in front of the camera
         if (isFollowing){
            

            Vector3 targetPosition = arCamera.transform.position + arCamera.transform.forward * distanceFromCamera;
            transform.position = targetPosition;

            // Optional: rotate to face the camera
            //transform.LookAt(arCamera.transform);
        }
    }

    private IEnumerator goToBackInitalPosition() {
        yield return new WaitForSeconds(3f);
        isTouchingPlane = false;
        touchedPlane = null;

  
        //rb.isKinematic = false;
        rb.useGravity = false;
        fallen = true;
        //transform.position = initalPosition + Vector3.up * 0.1f;

        //yield return new WaitForFixedUpdate();
        //transform.position = initalPosition;



        Collider collider = GetComponent<Collider>();
        if (collider == null)
        {
            statusText.text = "Missing! Re-adding BoxCollider...";
            BoxCollider newCol = gameObject.AddComponent<BoxCollider>();
            newCol.enabled = true;

            if (newCol != null) {
                statusText.text = "Enabled";
            }
        } else {
            statusText.text = "Exists.";
            collider.enabled = true;
        }

        rb.constraints |= RigidbodyConstraints.FreezePositionY; // freeze Y position again
        //statusText.text = "You may continue...";
    }

    private void OnCollisionEnter(Collision collision)
    {
        //statusText.text = "checking if hitting plane...";
        if (collision.collider.CompareTag("HorizontalPlane")) {
            statusText.text = "YES touching plane";
            touchedPlane = collision.collider.GetComponent<ARPlane>();
            isTouchingPlane = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.CompareTag("HorizontalPlane"))
        {
            isTouchingPlane = false;
            touchedPlane = null;
        }
    }

    private void UpdateTouchingUI()
    {
        if (statusText == null) return;

        if (isTouchingPlane)
        {
            //statusText.text = "YES touching plane";
            statusText.color = Color.green;
        }
        else
        {
            //statusText.text = "NO not touching plane";
            statusText.color = Color.red;
        }
    }

    

}
