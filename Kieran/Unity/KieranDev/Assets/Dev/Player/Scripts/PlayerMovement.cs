using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]

public class PlayerMovement : MonoBehaviour
{

    public float walkspeed = 10f;
    public float currentspeed = 0f;

    private CharacterController characterController;
    private Collider playerCollider;

    Vector3 velocity;
    public float gravity = -19.62f; // Two times 9.81 since it felt sluggish
    public bool isGrounded;
    private float distToGround;
    public float jumpHeight = 3f;

    private Vector3 playerOrigin;

    public SphereCollider sphereCollider;

    [Range(0.0f, 50.0f)]
    public float speedUpRamp = 40f;
    [Range(0.0f, 50.0f)]
    public float speedDownRamp = 40f;

    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        // -------------------------------- Move Player
        // Find desired direction

        Vector3 input;
        GetInput(out input);

        Vector3 desiredDirection = transform.forward * input.z + transform.right * input.x;

        // Get a normal for the surface that is being touched to move along it
        RaycastHit hitInfo;
        Physics.SphereCast(transform.position, characterController.radius, Vector3.down, out hitInfo, characterController.height / 2f, Physics.AllLayers, QueryTriggerInteraction.Ignore);
        desiredDirection = Vector3.ProjectOnPlane(desiredDirection, hitInfo.normal).normalized;

        // Move Character
        if (input == Vector3.zero)
        {
            currentspeed -= Time.deltaTime * speedDownRamp;
            currentspeed = Mathf.Clamp(currentspeed, 0, walkspeed);
            characterController.Move(transform.forward * currentspeed * Time.deltaTime);
        }
        else
        {
            currentspeed += Time.deltaTime * speedUpRamp;
            currentspeed = Mathf.Clamp(currentspeed, 0, walkspeed);
            characterController.Move(desiredDirection * currentspeed * Time.deltaTime);
        }


        if (Input.GetButtonDown("Jump"))
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
        }

        // -------------------------------- Apply gravity
        // Find distance to ground
        RaycastHit groundHitInfo;
        playerOrigin = transform.position;
        Physics.Raycast(playerOrigin, Vector3.down, out groundHitInfo);
        distToGround = groundHitInfo.distance - 1f;

        // If higher than value, apply negative velocity
        if (distToGround > 0.1f)
        {
            velocity.y += gravity * Time.deltaTime;
            characterController.Move(velocity * Time.deltaTime);
        }
        else
        {
            velocity.y = -2f;
        }

        // ---------------------------------- Wall run??

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
}
