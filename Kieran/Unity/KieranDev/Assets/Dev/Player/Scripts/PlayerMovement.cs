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
    public Vector3 desiredDirection;

    // Private declerations

    private Rigidbody rb;
    private GrapplingHook grapplingHook;
    private CapsuleCollider capsuleCollider;


    private float speed;
    public bool isGrounded = false;
    public bool isJumping;

    private bool isAttached = true;
    private float groundCheckDistance = 0.05f;
    private Vector3 directionHolder;
    private Vector3 currentDirection;

    // Declerations
    public bool isWallRunForward = false;
    public bool isWallRunRight = false;
    public bool isWallRunLeft = false;
    public bool attached = false;
    public Vector3 wallRunVelocity;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        grapplingHook = GetComponent<GrapplingHook>();
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
        wallRun();
        sprint();
        jump();
        hook();

        // -------------------------------- Apply movement --------------------------------
        velocity = currentDirection * speed;
        rb.velocity = new Vector3(velocity.x, rb.velocity.y, velocity.z);
        velocity = rb.velocity;
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
            //Debug.Log((i, angle));
            Vector3 positionOfRay = transform.position + Vector3.down + new Vector3(capsuleCollider.radius * Mathf.Cos(angle), 0.01f, capsuleCollider.radius * Mathf.Sin(angle));
            Debug.DrawRay(positionOfRay, Vector3.down * groundCheckDistance, Color.red);
            if (Physics.Raycast(positionOfRay, Vector3.down, out groundCheckInfohit, groundCheckDistance))
            {
                isWallRunForward = false;
                isWallRunRight = false;
                isWallRunLeft = false;
                attached = false;
                return true;
            }

        }
        return false;
    }
    
    void sprint()
    {
        if (isJumping)
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
            Debug.Log("Swinging");
        }
    }

    void wallRun()
    {
        RaycastHit wallRunHitInfo;
        float angle_forward = (90f - transform.eulerAngles.y) / 180.0f * Mathf.PI;
        float angle_right = (0f - transform.eulerAngles.y) / 180.0f * Mathf.PI;
        float angle_left = (180f - transform.eulerAngles.y) / 180.0f * Mathf.PI;
        Vector3 positionOfRay_forward = transform.position + new Vector3(capsuleCollider.radius * Mathf.Cos(angle_forward), 0f, capsuleCollider.radius * Mathf.Sin(angle_forward));
        Vector3 positionOfRay_right = transform.position + new Vector3(capsuleCollider.radius * Mathf.Cos(angle_right), 0f, capsuleCollider.radius * Mathf.Sin(angle_right));
        Vector3 positionOfRay_left = transform.position + new Vector3(capsuleCollider.radius * Mathf.Cos(angle_left), 0f, capsuleCollider.radius * Mathf.Sin(angle_left));

        float wallRunCheckDistance = 2.0f;

        if (Physics.Raycast(positionOfRay_forward, transform.forward, out wallRunHitInfo, wallRunCheckDistance) && !isGrounded && !isWallRunLeft && !isWallRunRight && !isWallRunForward)
        {
            isWallRunForward = true;
            wallRunVelocity = rb.velocity;
            attached = true;
        }
        else if (Physics.Raycast(positionOfRay_right, transform.right, out wallRunHitInfo, wallRunCheckDistance) && !isGrounded && !isWallRunLeft && !isWallRunRight && !isWallRunForward)
        {
            isWallRunRight = true;
            wallRunVelocity = rb.velocity;
            directionHolder = desiredDirection;
            attached = true;
        }
        else if (Physics.Raycast(positionOfRay_left, -1f * transform.right, out wallRunHitInfo, wallRunCheckDistance) && !isGrounded && !isWallRunLeft && !isWallRunRight && !isWallRunForward)
        {
            isWallRunLeft = true;
            wallRunVelocity = rb.velocity;
            directionHolder = desiredDirection;
            attached = true;
        }

        Debug.DrawRay(positionOfRay_forward, transform.forward * wallRunCheckDistance, Color.red);
        Debug.DrawRay(positionOfRay_right, transform.right * wallRunCheckDistance, Color.red);
        Debug.DrawRay(positionOfRay_left, -1f * transform.right * wallRunCheckDistance, Color.red);

        if (isWallRunRight || isWallRunLeft || isWallRunForward)
        {
            // Make walls run slower as if you are grinding on wall
            if (attached) rb.AddForce(new Vector3(0f, 4f, 0f));

            // Keep velocity along wall
            if (attached) rb.velocity = new Vector3(wallRunVelocity.x, rb.velocity.y, wallRunVelocity.z);

            // Stop movement off wall
            if (attached) currentDirection = directionHolder;

            // Detach from wall
            if (Input.GetKeyDown(KeyCode.Space)) // NEED TO GET THIS PART INTO THE UPDATE LOOP
            {
                attached = false;

                isWallRunForward = false;
                isWallRunRight = false;
                isWallRunLeft = false;

                currentDirection = desiredDirection;
                rb.AddForce(desiredDirection + new Vector3(0, jumpHeight, 0), ForceMode.Impulse);
            }
        }
    }
}
