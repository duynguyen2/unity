using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Controller;

public class TargetCollision : MonoBehaviour
{
    public Collider handSphere;
    public GameObject curObj;
    public Rigidbody curObjRB;
    public Collider curObjCol;
    public static string curObjTag;
    static bool isGrabbed = false;

    public Transform destination;

    private void Start()
    {
        handSphere = GameObject.Find("VRHand").GetComponent<SphereCollider>();
        if (handSphere != null)
            Debug.Log("FOUND " + handSphere.name);
        else
            Debug.Log("CANNOT FIND HAND SPHERE COLLIDER");

        curObjRB = curObj.GetComponent<Rigidbody>();
        curObjCol = curObj.GetComponent<Collider>();
        curObjTag = curObjCol.tag;
        destination = GameObject.Find("Destination").transform;
    }

    private void OnCollisionEnter(Collision collision)
    {
        string colTag = collision.collider.tag;
        Debug.Log("Collision type: " + colTag);
        if (!Controller.isHolding() && Controller.isGrabGesture() && (colTag.Equals("Controller")))
        {
            Debug.Log("Collision with: " + collision.collider.name);
            isGrabbed = true;
            this.transform.position = destination.position - this.transform.localScale;
            //this.transform.rotation = destination.rotation;
            SetParent(destination);
            this.GetComponent<Rigidbody>().useGravity = false;
            this.GetComponent<Collider>().enabled = false;
        }

    }

    private void OnCollisionStay(Collision collision)
    {
        if (!Controller.isGrabGesture())
        {
            isGrabbed = false;
            detachParent();
            this.GetComponent<Rigidbody>().isKinematic = true;
            this.GetComponent<Rigidbody>().useGravity = true;
            this.GetComponent<Collider>().enabled = true;
        }
    }

    public static bool isBeingGrabbed()
    {
        return isGrabbed;
    }

    public static string returnTag()
    {
        return curObjTag;
    }

    private void SetParent(Transform newParent)
    {
        this.transform.parent = newParent.transform;
        Debug.Log("New Parent: " + this.transform.parent.name);
        if (newParent.transform.parent != null)
        {
            Debug.Log("Grand Parent: " + this.transform.parent.parent.name);
        }
    }

    public void detachParent()
    {
        this.transform.parent = null;
        this.transform.parent = GameObject.Find("Pickup Objects").transform;
    }
}
