using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallRun : MonoBehaviour
{
    public float wallDistanceCheck;
    public int numberOfRays;

    private Rigidbody rb;
    private PlayerMovement playerMovement;
    public bool foundWall;
    private RaycastHit closestWall;
    bool isGrounded;
    public bool wallRunRight;
    public bool isWallRunning;

    private bool canJumpOffWall;

    Vector3 wallRunningVector;


    // Timer 
    bool timerActive;
    float startTime;
    float currentTime;

    void Start()
    {
        foundWall = false;
        rb = GetComponent<Rigidbody>();
        playerMovement = GetComponent<PlayerMovement>();
        timerActive = false;
        isWallRunning = false;
        canJumpOffWall = true;
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = playerMovement.isGrounded;
    }

    private void FixedUpdate()
    {
        wallRunningVector = wallRun();
        if (wallRunningVector != Vector3.zero)
        {
            Vector3 velocity;
            Vector3 currentDirection = wallRunningVector;
            float speed = playerMovement.speed;
            velocity = currentDirection * speed;
            Vector2 planarVelocity = new Vector2(velocity.x, velocity.z);
            if (planarVelocity.magnitude > speed) planarVelocity = Vector2.ClampMagnitude(planarVelocity, speed);
            velocity.x = planarVelocity.x;
            velocity.z = planarVelocity.y;
            rb.velocity = new Vector3(velocity.x, rb.velocity.y, velocity.z);
            velocity = rb.velocity;
        }
 
    }

    public Vector3 wallRun()
    {
        getClosestWall();
        if (foundWall && !isGrounded)
        {
            isWallRunning = true;
            Vector3 currentDirection = rb.velocity / rb.velocity.magnitude;
            Vector3 wallRunningVector = Vector3.Cross(closestWall.normal, Vector3.up);
            float magnitude1 = (currentDirection - wallRunningVector).magnitude;
            float magnitude2 = (currentDirection + wallRunningVector).magnitude;
            rb.AddForce(new Vector3(0f, 4f, 0f), ForceMode.Acceleration);
            wallRunRight = magnitude1 < magnitude2 ? true : false;
            return magnitude1 < magnitude2 ? wallRunningVector : -1 * wallRunningVector;

        }
        isWallRunning = false;
        return Vector3.zero;
    }

    public void getClosestWall()
    {
        foundWall = false;
        RaycastHit[] arrayOfRayCastHits = new RaycastHit[numberOfRays];
        float smallestDistance = wallDistanceCheck + 1.0f;

        for (int i = 0; i < numberOfRays; i++)
        {
            float angle = (float)i * (180.0f / ((float)numberOfRays - 1)) - transform.eulerAngles.y;
            angle = angle / 180.0f * Mathf.PI;
            Vector3 direction = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle));
            if (Physics.Raycast(transform.position, direction, out arrayOfRayCastHits[i], wallDistanceCheck, 1 << LayerMask.NameToLayer("Walls")))
            { 
                if (arrayOfRayCastHits[i].distance < smallestDistance)
                {
                    smallestDistance = arrayOfRayCastHits[i].distance;
                    closestWall = arrayOfRayCastHits[i];
                    foundWall = true;
                }
            }
        }
    }

    void startWallTimer()
    {
        timerActive = true;
        startTime = Time.time;
        currentTime = Time.time;
    }

    void updateWallTimer()
    {
        if (timerActive)
        {
            currentTime += Time.deltaTime;
            if (currentTime > startTime + 1.5f)
            {
                timerActive = false;
                Debug.Log("Two seconds have passed since you jumped");
            }
        }
    }
}
