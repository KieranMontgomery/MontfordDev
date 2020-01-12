using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    public CharacterController controller;

    public float speed = 12f;

    // Update is called once per frame
    void FixedUpdate()
    {
        float movHorizontal = Input.GetAxis("Horizontal");
        float movVertical = Input.GetAxis("Vertical");

        Vector3 move = transform.right * movHorizontal + transform.forward * movVertical;

        controller.Move(move * speed * Time.deltaTime);
    }
}
