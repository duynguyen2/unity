using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]

public class IKController : MonoBehaviour
{

    protected Animator animator;

    public Transform indexProx = null;
    public Transform indexMid = null;
    public Transform indexDist = null;

    public bool ikActive = false;
    public GameObject indexProxGoal = null;
    public GameObject indexMidGoal = null;
    public GameObject indexDistGoal = null;

    public Transform indexProxTarget = null;
    public Transform indexMidTarget = null;
    public Transform indexDistTarget = null;

    protected GameObject createIKGoal(string name)
    {
        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        obj.name = name;
        obj.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
        return obj;
    }

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();

        indexProx = GameObject.Find("indexProximal").transform;
        indexMid = GameObject.Find("indexMiddle").transform;
        indexDist = GameObject.Find("indexDistal").transform;

        indexProxGoal = createIKGoal("indexProxGoal");
        indexMidGoal = createIKGoal("indexMidGoal");
        indexDistGoal = createIKGoal("indexDistGoal");

        if (indexProx)
            Debug.Log("Found PROXIMAL");
        else
            Debug.Log("Cannot Find PROXIMAL");
        if (indexMid)
            Debug.Log("Found MIDDLE");
        else
            Debug.Log("Cannot Find MIDDLE");
        if (indexDist)
            Debug.Log("Found DISTAL");
        else
            Debug.Log("Cannot Find DISTAL");
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (animator) {

            if (ikActive) {

                if (indexProxTarget != null)
                {
                    animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
                    animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
                    animator.SetIKPosition(AvatarIKGoal.RightHand, indexProxTarget.position);
                    animator.SetIKRotation(AvatarIKGoal.RightHand, indexProxTarget.rotation);
                }

                //if the IK is not active, set the position and rotation of the hand and head back to the original position
                else
                {
                    animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
                    animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0);
                    animator.SetLookAtWeight(0);
                }

                if (indexMidTarget != null)
                {
                    animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
                    animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
                    animator.SetIKPosition(AvatarIKGoal.RightHand, indexMidTarget.position);
                    animator.SetIKRotation(AvatarIKGoal.RightHand, indexMidTarget.rotation);
                }

                //if the IK is not active, set the position and rotation of the hand and head back to the original position
                else
                {
                    animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
                    animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0);
                    animator.SetLookAtWeight(0);
                }

                if (indexDistTarget != null)
                {
                    animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
                    animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
                    animator.SetIKPosition(AvatarIKGoal.RightHand, indexDistTarget.position);
                    animator.SetIKRotation(AvatarIKGoal.RightHand, indexDistTarget.rotation);
                }

                //if the IK is not active, set the position and rotation of the hand and head back to the original position
                else
                {
                    animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
                    animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0);
                    animator.SetLookAtWeight(0);
                }

            }

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void LateUpdate()
    {
        Vector3 targetPosition = indexProxTarget.position;
    }
}
