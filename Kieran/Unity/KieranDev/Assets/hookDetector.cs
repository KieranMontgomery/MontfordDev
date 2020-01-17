using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hookDetector : MonoBehaviour
{

    public bool attached;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name != "Player") attached = true;
    }

    private void OnTriggerExit(Collider other)
    {
        attached = false;
    }
}
