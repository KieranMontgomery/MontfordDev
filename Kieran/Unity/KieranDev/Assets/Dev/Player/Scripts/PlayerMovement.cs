using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]

public class PlayerMovement : MonoBehaviour
{

    public float walkspeed = 10f;

    private CharacterController characterController;
    private Collider playerCollider;

    Vector3 velocity;
    public float gravity = -19.62f; // Two times 9.81 since it felt sluggish
    public bool isGrounded;
    public float distToGround;
    public float jumpHeight = 3f;

    private Vector3 playerOrigin;
    

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
        Vector2 input;
        GetInput(out input);
        Vector3 desiredDirection = transform.forward * input.y + transform.right * input.x;

        // Get a normal for the surface that is being touched to move along it
        RaycastHit hitInfo;
        Physics.SphereCast(transform.position, characterController.radius, Vector3.down, out hitInfo, characterController.height / 2f, Physics.AllLayers, QueryTriggerInteraction.Ignore);
        desiredDirection = Vector3.ProjectOnPlane(desiredDirection, hitInfo.normal).normalized;

        // Move Character
        characterController.Move(desiredDirection * walkspeed * Time.deltaTime);

        if (Input.GetButtonDown("Jump"))
        {
            Debug.Log("Pressed jump");
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
    }

    void GetInput(out Vector2 input)
    {
        input.x = Input.GetAxisRaw("Horizontal");
        input.y = Input.GetAxisRaw("Vertical");

        // Normalize input if it exceeds 1 in combined length
        if (input.sqrMagnitude > 1)
        {
            input.Normalize();
        }
    }


}
