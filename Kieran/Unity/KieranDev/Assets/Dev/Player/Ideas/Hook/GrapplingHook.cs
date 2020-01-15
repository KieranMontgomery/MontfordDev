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
    public bool attached;
    public bool foundTarget;

    private Collider hookCollider;
    private float travelSpeed;

    int layerMask = 1 << 10; // Bit shift the index of the layer (8) to get a bit mask

    Vector3 target;
    private RaycastHit hookHitTarget;

    void Start()
    {
        hookCollider = hook.GetComponent<Collider>();
        layerMask = ~layerMask; // Invert to get everything but layer.
        travelSpeed = hookTravelSpeed / 100f;
        havePointOnWall = false;
        allowedToShoot = true;
        attached = false;
        foundTarget = false;
    }
    void Update()
    {
        // Fired hook
        if (Input.GetKey(KeyCode.E) && allowedToShoot) // Not shooting
        {
            if (!havePointOnWall) // Dont have point on wall
            {

                // Try look at wall
                foundTarget = Physics.Raycast(hookHolder.transform.position, hookHolder.transform.forward, out hookHitTarget, maxDistance, layerMask);
                if (foundTarget) // Found wall
                {
                    havePointOnWall = true;
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

            if (hook.transform.position == hookHitTarget.point)
            {
                attached = true;
            }
            else
            {
                attached = false;
            }

            if (attached)
            {
                RaycastHit hookClip;
                Vector3 direction;
                direction = hook.transform.position - hookHolder.transform.position;
                Physics.Raycast(hookHolder.transform.position, direction, out hookClip, maxDistance, layerMask);
                Debug.DrawRay(hookHolder.transform.position, direction, Color.green);

                if (hookClip.point != hookHitTarget.point && hookClip.transform.gameObject.name != "Player")
                {
                    Debug.Log(("Should break", hookClip.transform.gameObject.name));
                    ReturnHook();
                    allowedToShoot = false;
                    havePointOnWall = false;
                }
            }
            
          
        }

        if (Input.GetKeyUp(KeyCode.E))
        {
            ReturnHook();
            allowedToShoot = true;
            havePointOnWall = false;
            attached = false;
        }
    }

    void ReturnHook()
    {
        hook.transform.position = hookHolder.transform.position;
        attached = false;
    }
}

