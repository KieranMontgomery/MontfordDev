using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody ))]

public class PlayerMovement : MonoBehaviour
{

    // Public declerations

    public float walkspeed = 10f;
    public float sprintspeed = 15f;
    public Vector3 velocity;
    public float velocityMagnitude;
    public float gravity = -19.62f; // Two times 9.81 since it felt sluggish
    public float jumpHeight = 5f;
    [Range(3, 360)]
    public int numberOfGroundCheckRays;
    [Range(3, 30)]
    public int numberOfWallCheckRays;
    public Vector3 desiredDirection;

    // Private declerations

    private Rigidbody rb;
    private GrapplingHook grapplingHook;
    private WallRun wallRun;
    private CapsuleCollider capsuleCollider;


    private float speed;

    public bool isGrounded = true;
    private bool justLanded = false;
    private bool inAir = false;
    public bool isJumping;

    private bool isAttached = true;
    private float groundCheckDistance = 0.05f;
    private Vector3 currentDirection;

    // Declerations

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        grapplingHook = GetComponent<GrapplingHook>();
        wallRun = GetComponent<WallRun>();
        capsuleCollider = GetComponent<CapsuleCollider>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        isGrounded = GroundCheck();
        isAttached = GetComponent<GrapplingHook>().attached;

        // -------------------------------- Planar movement --------------------------------
        Vector3 input;
        GetInput(out input);
        desiredDirection = transform.forward * input.z + transform.right * input.x;
        currentDirection = desiredDirection;

        // -------------------------------- Other movement attributes --------------------------------
        Vector3 wallRunningDirection = wallRun.wallRun();
        sprint();
        jump();
        hook();

        // -------------------------------- Apply movement --------------------------------

        if (wallRunningDirection != Vector3.zero) currentDirection = wallRunningDirection;
        velocity = currentDirection * speed;
        Vector2 planarVelocity = new Vector2(velocity.x, velocity.z);
        if (planarVelocity.magnitude > speed) planarVelocity = Vector2.ClampMagnitude(planarVelocity, speed);
        velocity.x = planarVelocity.x;
        velocity.z = planarVelocity.y;
        rb.velocity = new Vector3(velocity.x, rb.velocity.y, velocity.z);
        velocity = rb.velocity;
        velocityMagnitude = planarVelocity.magnitude;


    }

    private void Update()
    {
        // Check for grapple hook
        grapplingHook.grapple();
        isJumping = Input.GetKey(KeyCode.Space);
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
            RaycastHit groundCheckInfohit;
            float angle = (float)i * 360f/(float)numberOfGroundCheckRays;
            angle = angle / 180.0f * Mathf.PI;
            // Debug.Log((i, angle));
            Vector3 positionOfRay = transform.position + Vector3.down + new Vector3(capsuleCollider.radius * Mathf.Cos(angle), 0.01f, capsuleCollider.radius * Mathf.Sin(angle));
            // Debug.DrawRay(positionOfRay, Vector3.down * groundCheckDistance, Color.red);
            if (Physics.Raycast(positionOfRay, Vector3.down, out groundCheckInfohit, groundCheckDistance))
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

    void jump()
    {
        if (isGrounded && Input.GetKey(KeyCode.Space))
        {
            rb.AddForce(new Vector3(0, jumpHeight, 0), ForceMode.Impulse);
        }
    }

    void hook()
    {
        if (grapplingHook.attached)
        {
            // Debug.Log("Swinging");
        }
    }

}
