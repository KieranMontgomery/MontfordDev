using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallRun : MonoBehaviour
{
    public float wallDistanceCheck;
    public int numberOfRays;

    private Rigidbody rb;
    private PlayerMovement playerMovement;
    private bool foundWall;
    private RaycastHit closestWall;

    bool isGrounded;

    void Start()
    {
        foundWall = false;
        rb = GetComponent<Rigidbody>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = playerMovement.isGrounded;
    }

    public Vector3 wallRun()
    {
        getClosestWall();
        if (foundWall && !isGrounded)
        {
            Vector3 currentDirection = rb.velocity / rb.velocity.magnitude;
            Vector3 wallRunningVector = Vector3.Cross(closestWall.normal, Vector3.up);
            float magnitude1 = (currentDirection - wallRunningVector).magnitude;
            float magnitude2 = (currentDirection + wallRunningVector).magnitude;

            return magnitude1 < magnitude2 ? wallRunningVector : -1 * wallRunningVector;

        }
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
}