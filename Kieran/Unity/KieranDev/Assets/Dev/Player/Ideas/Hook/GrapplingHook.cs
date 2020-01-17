using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingHook : MonoBehaviour
{
    public GameObject hook;
    public GameObject hookHolder;
    public SphereCollider sphereCollider;

    public float hookTravelSpeed;
    public float playerTravelSpeed;

    public static bool fired;
    public static bool hooked;

    public float maxDistance;
    private float currentDistance;

    public bool havePointOnWall;
    public bool allowedToShoot;
    public bool attached;

    private float travelSpeed;
    public float hookLineLength;

    int layerMask = 1 << 10; // Bit shift the index of the layer (8) to get a bit mask

    Vector3 target;
    private RaycastHit hookHitTarget;
    void Start()
    {
        layerMask = ~layerMask; // Invert to get everything but layer.
        travelSpeed = hookTravelSpeed / 100f;
        havePointOnWall = false;
        allowedToShoot = true;
        attached = false;
    }
    public void grapple()
    {
        // Fired hook
        if (Input.GetKey(KeyCode.E) && allowedToShoot) // Not shooting
        {
            if (!havePointOnWall) // Dont have point on wall
            {
                if (Physics.Raycast(hookHolder.transform.position, hookHolder.transform.forward, out hookHitTarget, maxDistance, layerMask)) // Found wall
                {
                    havePointOnWall = true;
                    target = hookHitTarget.point;
                    hookLineLength = hookHitTarget.distance;
                }
                else
                {
                    target = hookHolder.transform.position + hookHolder.transform.forward * maxDistance;
                }
            }

            hook.transform.position = Vector3.MoveTowards(hook.transform.position, target, travelSpeed);
            currentDistance = Vector3.Distance(hook.transform.position, hookHolder.transform.position);

            if (currentDistance >= maxDistance)
            {
                ReturnHook();
                allowedToShoot = false;
            }

            attached = hook.transform.position == hookHitTarget.point;

            if (attached)
            {
                RaycastHit hookClip;
                Vector3 direction;
                direction = hook.transform.position - hookHolder.transform.position;
                Physics.Raycast(hookHolder.transform.position, direction, out hookClip, maxDistance, layerMask);
                Debug.DrawRay(hookHolder.transform.position, direction, Color.green);

                if (hookClip.point != hookHitTarget.point && hookClip.transform.gameObject.name != "Player") // Break if hook goes out of LOS.
                {
                    ReturnHook();
                    allowedToShoot = false;
                }                
            }
        }

        if (Input.GetKeyUp(KeyCode.E))
        {
            ReturnHook();
            allowedToShoot = true;
            attached = false;
            Debug.Log("Let go");
        }
    }

    void ReturnHook()
    {
        hook.transform.position = hookHolder.transform.position;
        allowedToShoot = true;
        attached = false;
        havePointOnWall = false;
    }


}