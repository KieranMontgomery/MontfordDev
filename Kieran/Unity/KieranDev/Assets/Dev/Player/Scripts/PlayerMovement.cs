using System.Collections;
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
    public float groundCheckDistance = 0.05f;
    [Range(3, 360)]
    public int numberOfGroundCheckRays;

    // Private declerations

    private CharacterController characterController;
    private GrapplingHook grapplingHook;
    private SpringJoint joint;

    private float speed;
    private bool isGrounded = false;
    private bool isAttached = true;

    // Declerations
    Ray[] groundCheckRays;
    RaycastHit[] groundCheckRaysInfo;
    Vector3 impact = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        grapplingHook = GetComponent<GrapplingHook>();
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = GroundCheck();
        isAttached = GetComponent<GrapplingHook>().attached;

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
        if (!isGrounded && !isAttached)
        {
            velocity.y += gravity * Time.deltaTime;
        }
        else
        {
            velocity.y = -characterController.stepOffset / Time.deltaTime;
        }

        // -------------------------------- Other movement attributes --------------------------------
        sprint();
        //grapplingHook.grapple(transform.position);

        // -------------------------------- Apply movement --------------------------------
        characterController.Move(velocity * Time.deltaTime);
        velocityMagnitude = velocity.magnitude;

        grapplingHook.grapple(transform.position);

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
        for (int i=0; i < numberOfGroundCheckRays; i++)
        {
            RaycastHit hit;
            float angle = (float)i * 360f/(float)numberOfGroundCheckRays;
            angle = angle / 180.0f * Mathf.PI;
            //Debug.Log((i, angle));
            Vector3 positionOfRay = transform.position + Vector3.down + new Vector3(characterController.radius * Mathf.Cos(angle), 0f, characterController.radius * Mathf.Sin(angle));
            Debug.DrawRay(positionOfRay, Vector3.down * groundCheckDistance, Color.red);
            if (Physics.Raycast(positionOfRay, Vector3.down, out hit, groundCheckDistance))
            {
                return true;
            }

        }
        return false;
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
