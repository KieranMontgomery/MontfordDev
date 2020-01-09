using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBoost : PlayerMovement
{

    private void OnTriggerEnter(Collider name)
    {
        if (name.name == "BoostPad")
        {
            zspeed += 100;
        }
        else if (name.name == "StopPad")
        {
            Vector3 pos = transform.position;
            rb.drag = 100f;
        }
    }

    private void OnTriggerStay(Collider name)
    {
        if (name.name == "StopPad")
        {
            rb.drag = 0f;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        
    }
}
