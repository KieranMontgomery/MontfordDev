using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]

public class PlayerMovement : MonoBehaviour
{

    // Public declerations

    public CapsuleCollider capsuleCollider;
    public float walkspeed = 10f;
    public float sprintspeed = 15f;
    public float currentspeed = 0f;
    public float airspeed = 10f;
    public float gravity = -19.62f; // Two times 9.81 since it felt sluggish
    public bool isGrounded = false;
    public bool isAir;
    public Vector3 directionOfJump;
    public float jumpHeight = 3f;
    public Vector3 currentVelocity;

    [Range(0.0f, 50.0f)]
    public float speedUpRamp = 40f;
    [Range(0.0f, 50.0f)]
    public float speedDownRamp = 40f;
    public Vector3 currentDirection;
    public float distance = 1.05f;
    public RaycastHit hit;

    // Private declerations

    private CharacterController characterController;
    private float speed;

    float startTime;

    // Declerations

    Vector3 velocity; // This is annoying to have

    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        GroundCheck();
        // -------------------------------- Move Player
        // Find desired direction
        Vector3 input;
        GetInput(out input);
        Vector3 desiredDirection = transform.forward * input.z + transform.right * input.x;

        // Move Character
        if (Input.GetKey(KeyCode.LeftShift))
        {
            speed = sprintspeed;
        }
        else
        {
            speed = walkspeed;
        }

        if (input == Vector3.zero)
        {
            if (isGrounded)
            {
                currentspeed -= Time.deltaTime * speedDownRamp;
                currentspeed = Mathf.Clamp(currentspeed, 0, speed);
                currentVelocity = currentDirection * currentspeed * Time.deltaTime;
                characterController.Move(currentVelocity);
            }
            else // in the air with no input
            {
                inAirMovement(currentDirection);
            }
            if (currentspeed == 0)
            {
                currentDirection = Vector3.zero;
            }

        }
        else
        {
            if (isGrounded)
            {
                currentspeed += Time.deltaTime * speedUpRamp;
                currentspeed = Mathf.Clamp(currentspeed, 0, speed);
                currentDirection = desiredDirection;
                currentVelocity = currentDirection * currentspeed * Time.deltaTime;
                characterController.Move(currentVelocity);
            }
            else // in the air with input
            {
                inAirMovement(desiredDirection);
            }
        }

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            isGrounded = false;
            velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
            characterController.Move(velocity * Time.deltaTime);
            directionOfJump = transform.forward;
        }

        // -------------------------------- Apply gravity
        if (!isGrounded)
        {
            velocity.y += gravity * Time.deltaTime;
            characterController.Move(velocity * Time.deltaTime);
        }
        else
        {
            velocity.y = -characterController.stepOffset / Time.deltaTime;
            characterController.Move(velocity * Time.deltaTime);
        }
        if (!isGrounded)
        {
            isAir = true;
        }
        if (isGrounded && isAir)
        {
            // Just landed
            isAir = false;
            Debug.Log(currentDirection);
            currentVelocity = directionOfJump * currentspeed * Time.deltaTime;
            characterController.Move(currentVelocity);
        }
        
    }

    void GetInput(out Vector3 input)
    {
        input.x = Input.GetAxisRaw("Horizontal");
        input.y = 0;
        input.z = Input.GetAxisRaw("Vertical");

        // Normalize input if it exceeds 1 in combined length
        if (input.sqrMagnitude > 1)
        {
            input.Normalize();
        }
    }

    void GroundCheck()
    {
        isGrounded = IsGroundedByRaycast();
    }

    public bool IsGroundedByRaycast()
    {
        Debug.DrawRay(transform.position, Vector3.down * distance, Color.green);       //draw the line to be seen in scene window

        if (Physics.Raycast(transform.position, Vector3.down, out hit, distance))
        {      //if we hit something
            return true;
        }
        return false;
    }

    void inAirMovement(Vector3 desiredDirection)
    {
        // Default below
        // currentVelocity = currentDirection * currentspeed * Time.deltaTime;
        // characterController.Move(currentVelocity);

        float maxAirSpeed = currentspeed + 2f;
        float minAirSpeed = currentspeed - 2f;
        currentDirection = Vector3.Lerp(currentDirection, desiredDirection, 0.005f);
 
        if (currentspeed == 0)
        {
            currentVelocity += currentDirection * airspeed * Time.deltaTime;

            currentVelocity.x = Mathf.Clamp(currentVelocity.x, currentDirection.x * 2f * Time.deltaTime, currentDirection.x * 2f * Time.deltaTime);
            currentVelocity.y = Mathf.Clamp(currentVelocity.y, currentDirection.y * 2f * Time.deltaTime, currentDirection.y * 2f * Time.deltaTime);
            currentVelocity.z = Mathf.Clamp(currentVelocity.z, currentDirection.z * 2f * Time.deltaTime, currentDirection.z * 2f * Time.deltaTime);
        }
        else
        {
            currentVelocity += currentDirection * currentspeed * Time.deltaTime;

            currentVelocity.x = Mathf.Clamp(currentVelocity.x, currentDirection.x * minAirSpeed * Time.deltaTime, currentDirection.x * maxAirSpeed * Time.deltaTime);
            currentVelocity.y = Mathf.Clamp(currentVelocity.y, currentDirection.y * minAirSpeed * Time.deltaTime, currentDirection.y * maxAirSpeed * Time.deltaTime);
            currentVelocity.z = Mathf.Clamp(currentVelocity.z, currentDirection.z * minAirSpeed * Time.deltaTime, currentDirection.z * maxAirSpeed * Time.deltaTime);
        }


        characterController.Move(currentVelocity);
    }
}
