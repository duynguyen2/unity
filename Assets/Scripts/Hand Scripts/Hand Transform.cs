using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandTransform : MonoBehaviour
{
    //Rigidbody hand;
    public Vector2 turn;
    public float sensitivity = 0.5f;
    public Vector3 deltaMove;
    public float speed = 1;
    public GameObject move;

    // Start is called before the first frame update
    void Start()
    {
        //hand = gameObject.GetComponent<Rigidbody>();
        //anim = gameObject.GetComponent<Animator>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {

        //hand.setGrip(0f);

        turn.x += Input.GetAxis("Mouse X");
        turn.y += Input.GetAxis("Mouse Y");
        move.transform.localRotation = Quaternion.Euler(0, turn.x, 0);
        transform.localRotation = Quaternion.Euler(turn.y, turn.x, 0);

        deltaMove = new Vector3(-Input.GetAxisRaw("Horizontal") * speed * Time.deltaTime, 0, -Input.GetAxisRaw("Vertical") * speed * Time.deltaTime);
        move.transform.Translate(deltaMove);

        if (Input.GetMouseButtonDown(0))
        {
            //anim.SetTrigger("Active");
        }

    }

}