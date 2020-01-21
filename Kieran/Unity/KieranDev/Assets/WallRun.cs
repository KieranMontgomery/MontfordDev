using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallRun : MonoBehaviour
{
    // Public
    public bool canWallRun;
    public bool canWallClimb;
    public bool isWallRunning;
    public bool isWallClimbing;

    // Private
    private PlayerMovement playerMovement;

    // Declerations
    bool isGrounded;
    float distanceToWall;

    // Start is called before the first frame update
    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = GetComponent<PlayerMovement>().isGrounded;
    }

    public void wallRun()
    {
        GameObject closestWall = determineClosetWall();
        calculateWhatTypeOfRun(closestWall);
    }

    GameObject determineClosetWall() // Determines closest wall to player
    {
        RaycastHit[] castArray = Physics.SphereCastAll(transform.position, 2.0f, transform.forward, 0);

        distanceToWall = 5.0f;
        int indexOfClosestWall = -999;

        for (int i = 0; i < castArray.Length; i++)
        {
            if (castArray[i].transform != transform && castArray[i].collider.gameObject.layer == 9)
            {
                if (castArray[i].distance < distanceToWall)
                {
                    distanceToWall = castArray[i].distance;
                    indexOfClosestWall = i;
                }
            }
        }
        if (indexOfClosestWall == -999) return null;
        else return castArray[indexOfClosestWall].transform.gameObject;
    }

    void calculateWhatTypeOfRun(GameObject wall) // Determine if wall running is possible
    {
        bool criteria;
        criteria = playerMovement.velocityMagnitude > 5 && playerMovement.velocity.y > -10f && playerMovement.velocity.y < 10f;

        if (wall != null && criteria)
        {
            // Get closest point to wall
            Vector3 closestPointOnWall = wall.GetComponent<Collider>().ClosestPoint(transform.position);

            // Calculate angle between forward and closet point
            Vector3 targetDir = closestPointOnWall - transform.position;
            float angle = Vector3.Angle(targetDir, transform.forward);

            canWallRun = 30 <= angle && angle < 95 ? true : false;
            canWallClimb = angle < 30 && !Input.GetKey(KeyCode.S) ? true : false;

        }
        else
        {
            canWallRun = false;
            canWallClimb = false;
        }
    }
}
