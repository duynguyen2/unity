using System;
using System.Net.Sockets;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using System.Net;
using System.IO;
using static TargetCollision;

public class Controller : MonoBehaviour
{

    //wasd or arrow keys control the hand
    //spacebar moves it upward, left control moves it downward

    //when grab gesture and trigger are true, grab object and start adding to over grip ✓
    //****if overgrip reaches certain values () ✓
    //detect when it is already holding or grasping an object ✓
    //****if so, unable to pick up anything else until object is released and not conflicting ✓
    //****if overgrip reaches below the minimum value, release and over the max, destroy [tentative] ✓
    GameObject heldObj = null;
    Transform destination = null;

    // bounds for the grip, cannot go under the min and over the max
    readonly float MIN_GRIP = 0f;
    readonly float MAX_GRIP = 90f;

    public static float gripTracker = 0f;
    public static float grip = 0f;
    public static float gripRate = 1.5f;
    public static float overGripRate = gripRate / 3;

    //the grab threshold is whatever we set it to be, the default will be 45
    // grabThreshold = rotationRate * 30??
    readonly float[] grabThreshold = { 45f, 50f };
    public static bool grabGesture = false;

    // the given amount of grip that tries to overlap an object, if it reaches the degree of freedom then the glove gives, item is destroyed
    // if the overGrip is reaches the degree of release, the object is then released, this value is to give degree of freedom to actually release an item
    public static float degreeOfRelease = -6f;
    public static float degreeOfFreedom = 6f;
    protected static float overGrip = 0f;
    static int overCounter = 0;

    // variable to distinguish if it is touching an object
    // if so, if the user keeps trying to close the hand further, the counter goes up and nothing else happens
    // until the counter reaches a cerian number, e.g. 3, which distinguishes the user wanting to break the object
    // once the counter hits 3, break object and free the hand from all restrictions
    static bool inRange = false;
    static bool handTouching = false;
    public static bool handHolding = false;

    Animator anim;
    GameObject indexProximal, indexMiddle, indexDistal;
    public Phalanx indexProxPhal, indexMidPhal, indexDistPhal;
    Transform indexProximalTransform, indexMiddleTransform, indexDistalTransform;
    protected bool proxTouch = false, midTouch = false, distTouch = false;

    public Vector2 turn;
    public float sensitivity = 0.5f;
    public Vector3 deltaMove;
    public float speed = 1f;
    public float ascent = 1f;
    public GameObject move;

    static float force = 0f;
    public float MIN_FORCE = 0f;
    public float MAX_FORCE = 1f;

    void Awake()
    {
        destination = GameObject.Find("Destination").transform;
        indexProximal = GameObject.Find("indexProximal");
        indexMiddle = GameObject.Find("indexMiddle");
        indexDistal = GameObject.Find("indexDistal");
    }

    // Start is called before the first frame update
    void Start()
    {

        anim = gameObject.GetComponent<Animator>();
        Cursor.lockState = CursorLockMode.Locked;

    }

    // Update is called once per frame
    void Update()
    {

        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");
        float y = Input.GetAxisRaw("Jump");

        turn.x += Input.GetAxis("Mouse X");
        turn.y += Input.GetAxis("Mouse Y");

        turn.y = Mathf.Clamp(turn.y, -90, 90);

        move.transform.localRotation = Quaternion.Euler(0, turn.x, 0);
        transform.localRotation = Quaternion.Euler(turn.y, turn.x, 0);

        deltaMove = new Vector3(-x * speed * Time.deltaTime, y * speed * Time.deltaTime, -z * speed * Time.deltaTime);
        move.transform.Translate(deltaMove);

        if (!Input.anyKeyDown)
        {
            this.GetComponent<Rigidbody>().velocity = Vector3.zero;
            this.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        }

        if (Input.GetKeyDown(KeyCode.I))
            anim.SetBool("Proximal", !anim.GetBool("Proximal"));

        if (Input.GetKeyDown(KeyCode.O))
            anim.SetBool("Middle", !anim.GetBool("Middle"));

        if (Input.GetKeyDown(KeyCode.P))
            anim.SetBool("Distal", !anim.GetBool("Distal"));

        CalculateGrip();

    }

    void CalculateGrip()
    {
        float scrollVal = Input.mouseScrollDelta.y * gripRate;
        float deltaChange = grip + scrollVal;

        if (deltaChange >= grabThreshold[0] && deltaChange <= grabThreshold[1] && (!anim.GetBool("Proximal") && !anim.GetBool("Middle") && !anim.GetBool("Distal")))
        {
            grabGesture = true;
        }
        else
        {
            grabGesture = false;
        }

        if (handHolding)
        {
            OverGrip();
        }

        if (!handHolding)
        {
            gripTracker = Mathf.Clamp(gripTracker + scrollVal, MIN_GRIP, MAX_GRIP);
            grip = Mathf.Clamp(deltaChange, MIN_GRIP, MAX_GRIP);
            anim.SetFloat("Grip", grip);
        }

    }

    void OverGrip()
    {
        float overDelta = Input.mouseScrollDelta.y * overGripRate;
        float overGripChange = overGrip + overDelta;
        if (overDelta > 0)
            overCounter++;
        else if (overDelta < 0)
            overCounter--;

        grip = Mathf.Clamp(grip + overDelta, MIN_GRIP, MAX_GRIP);
        gripTracker = Mathf.Clamp(gripTracker + overDelta, MIN_GRIP, MAX_GRIP);
        overGrip = overGripChange;
        if (overGrip > degreeOfRelease && overGrip < degreeOfFreedom)
        { }

        else if (overGrip <= degreeOfRelease || overGrip >= degreeOfFreedom)
        {
            if (overGrip >= degreeOfFreedom)
                Destroy(heldObj);

            DetachParent();
            grabGesture = false;
            handTouching = false;
            anim.SetFloat("Grip", grip);
            overGrip = 0;
            overCounter = 0;
        }
    }

    private void SetParent(GameObject obj)
    {
        handHolding = true;
        obj.transform.parent = destination;
        obj.GetComponent<Rigidbody>().isKinematic = true;
        obj.GetComponent<Rigidbody>().useGravity = false;
        obj.GetComponent<Collider>().enabled = false;
        heldObj = obj;
        //heldObj.transform.rotation = destination.rotation;
        heldObj.transform.position = destination.position + new Vector3(0, heldObj.transform.localPosition.y / 2f, 0);
    }

    public void DetachParent()
    {
        handHolding = false;
        heldObj.transform.parent = null;
        heldObj.transform.position = destination.position - new Vector3(0, 0, 1f);
        heldObj.GetComponent<Rigidbody>().isKinematic = false;
        heldObj.GetComponent<Rigidbody>().useGravity = true;
        heldObj.GetComponent<Collider>().enabled = true;
        heldObj = null;
    }

    private void OnTriggerEnter(Collider collision)
    {
        inRange = true;
        if (grabGesture && !handHolding && collision.tag.Equals("Item"))
        {
            heldObj = GameObject.Find(collision.name);
            SetParent(heldObj);
        }

    }

    private void OnTriggerStay(Collider collision)
    {
        if (grabGesture && !handHolding && collision.tag.Equals("Item"))
        {
            heldObj = GameObject.Find(collision.name);
            SetParent(heldObj);
        }
        
    }

    private void OnTriggerExit(Collider other)
    {
        inRange = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision != null)
            handTouching = true;

        if (grabGesture && !handHolding && collision.collider.tag.Equals("Item"))
        {
            handHolding = true;
            heldObj = GameObject.Find(collision.collider.name);
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision != null || !collision.collider.tag.Equals("Item"))
            handTouching = true;
        else
            handTouching = false;

        if (grabGesture && !handHolding && collision.collider.tag.Equals("Item"))
        {
            handHolding = true;
            heldObj = GameObject.Find(collision.collider.name);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision != null || !collision.collider.tag.Equals("Item"))
            handTouching = true;
        else
            handTouching = false;

        if (grabGesture && !handHolding && collision.collider.tag.Equals("Item"))
        {
            handHolding = true;
            heldObj = GameObject.Find(collision.collider.name);
        }
    }

    public static int returnCounter()
    {
        return overCounter;
    }

    public static bool isInRange()
    {
        return inRange;
    }

    public static bool isGrabGesture()
    {
        return grabGesture;
    }

    public static bool isHolding()
    {
        return handHolding;
    }

    public static float returnForce()
    {
        return force;
    }

    public static float returnGrip()
    {
        return gripTracker;
    }

    public static float returnOverGrip()
    {
        return overGrip;
    }
}
