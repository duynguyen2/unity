using System;
using System.Net;
using System.IO;
using System.Threading;
using UnityEngine;

public class Phalanx : MonoBehaviour
{

    public static bool touching = false;
    public static Transform assignedPhalanx;
    public static string phalanxName;
    static float force = 0f;
    GameObject handObj;
    Transform handTransform;

    // Start is called before the first frame update
    void Start()
    {
        handObj = GameObject.Find("VRHand");
        handTransform = handObj.transform;

        if (handObj != null)
            Debug.Log("FOUND HAND NAME: " + handObj.name);
        else
            Debug.Log("CANNOT FIND HAND");
        if (handTransform != null)
            Debug.Log("FOUND HAND TRANSFORM: " + handTransform.name);
        else
            Debug.Log("CANNOT FIND HAND TRANSFORM");

        assignedPhalanx = this.transform;
        phalanxName = assignedPhalanx.name;
    }

    void Update()
    {
        Debug.Log(phalanxName + "'s touching: " + isTouching());
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision != null)
            touching = true;
        else
            touching = false;
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision != null)
            touching = true;
        else
            touching = false;
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision != null)
            touching = true;
        else
            touching = false;
    }

    public static bool isTouching()
    {
        return touching;
    }

    public static string returnName()
    {
        return phalanxName;
    }

    public static Transform returnTransform()
    {
        return assignedPhalanx;
    }

}
