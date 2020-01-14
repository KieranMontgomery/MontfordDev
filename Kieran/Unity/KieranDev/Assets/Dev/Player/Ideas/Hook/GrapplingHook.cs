using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingHook : MonoBehaviour
{
    public GameObject hook;
    public GameObject hookHolder;

    public float hookTravelSpeed;
    public float playerTravelSpeed;

    public static bool fired;
    public static bool hooked;

    public float maxDistance;
    private float currentDistance;

    public bool havePointOnWall;
    public bool allowedToShoot;

    private Collider hookCollider;
    private float travelSpeed;

    int layerMask = 1 << 10; // Bit shift the index of the layer (8) to get a bit mask

    Vector3 target;

    void Start()
    {
        hookCollider = hook.GetComponent<Collider>();
        layerMask = ~layerMask; // Invert to get everything but layer.
        travelSpeed = hookTravelSpeed / 100f;
        havePointOnWall = false;
        allowedToShoot = true;
    }
    void Update()
    {
        // Fired hook
        if (Input.GetKey(KeyCode.E) && allowedToShoot) // Not shooting
        {
            RaycastHit hookHitTarget;
            if (!havePointOnWall) // Dont have point on wall
            {

                // Try look at wall
                if (Physics.Raycast(hookHolder.transform.position, hookHolder.transform.forward, out hookHitTarget, maxDistance, layerMask)) // Found wall
                {
                    havePointOnWall = true;
                    Debug.DrawRay(hookHolder.transform.position, hookHolder.transform.forward * hookHitTarget.distance, Color.yellow);
                    target = hookHitTarget.point;
                }
                else
                {
                    target = hookHolder.transform.position + hookHolder.transform.forward * maxDistance;
                }
            }

            hook.transform.position = Vector3.MoveTowards(hook.transform.position, target, travelSpeed);

            // If hook is too far, bring it back
            currentDistance = Vector3.Distance(hook.transform.position, hookHolder.transform.position);
            if (currentDistance >= maxDistance)
            {
                ReturnHook();
                allowedToShoot = false;
            }

            Debug.Log(hook.transform.position);

        }

        if (Input.GetKeyUp(KeyCode.E))
        {
            ReturnHook();
            allowedToShoot = true;
            havePointOnWall = false;
        }
    }

    void ReturnHook()
    {
        hook.transform.position = hookHolder.transform.position;
    }
}

