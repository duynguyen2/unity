using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandCollision : MonoBehaviour
{

    void OnCollisionEnter(Collision collisionInfo)
    {
        Debug.Log("Collided with object: " + collisionInfo.collider.name);
    }

}
