﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]

public class PlayerMovement : MonoBehaviour
{

    // Public declerations

    public float walkspeed = 10f;
    public float sprintspeed = 15f;
    public Vector3 velocity;
    public float velocityMagnitude;
    public float gravity = -19.62f; // Two times 9.81 since it felt sluggish
    public float jumpHeight = 3f;
    public float grouncCheckDistance = 1.05f;

    // Private declerations

    private CharacterController characterController;
    private float speed;
    private bool isGrounded = false;

    // Declerations

    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        GroundCheck();
        // -------------------------------- Planar movement --------------------------------
        Vector3 input;
        GetInput(out input);
        Vector3 desiredDirection = transform.forward * input.z + transform.right * input.x;
        velocity.x = desiredDirection.x * speed;
        velocity.z = desiredDirection.z * speed;

        if (Input.GetKey(KeyCode.Space) && isGrounded)
        {
            isGrounded = false;
            velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
        }

        // -------------------------------- Vertical movement --------------------------------
        if (!isGrounded)
        {
            velocity.y += gravity * Time.deltaTime;
        }
        else
        {
            velocity.y = -characterController.stepOffset / Time.deltaTime;
        }

        // -------------------------------- Other movement attributes --------------------------------
        sprint();

        // -------------------------------- Apply movement --------------------------------
        characterController.Move(velocity * Time.deltaTime);
        velocityMagnitude = velocity.magnitude;

    }

    void GetInput(out Vector3 input)
    {
        input.x = Input.GetAxisRaw("Horizontal");
        input.y = 0;
        input.z = Input.GetAxisRaw("Vertical");
        if (input.sqrMagnitude > 1)
        {
            input.Normalize();
        }
    }

    public bool GroundCheck()
    {
        Debug.DrawRay(transform.position, Vector3.down * grouncCheckDistance, Color.red);       //draw the line to be seen in scene window
        RaycastHit hit;
        isGrounded = Physics.Raycast(transform.position, Vector3.down, out hit, grouncCheckDistance);
        // if (isGrounded) Debug.Log(hit.transform.gameObject.name);
        return isGrounded;
    }

    void sprint()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            speed += walkspeed * Time.deltaTime;
            speed = Mathf.Clamp(speed, walkspeed, sprintspeed);
        }
        else
        {
            speed = walkspeed;
        }
    }
}
