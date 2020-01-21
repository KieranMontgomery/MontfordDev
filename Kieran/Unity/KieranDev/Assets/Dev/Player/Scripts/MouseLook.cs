using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{

    public float mouseSensitiviyX = 100f;
    public float mouseSensitiviyY = 100f;

    public Transform playerBody;

    private WallRun wallRun;
    private PlayerMovement playerMovement;

    private float xRotation = 0f;
    private float yRotation = 0f;

    float tilt = 0.0f;
    float retilt = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        wallRun = GetComponentInParent<WallRun>();
        playerMovement = GetComponentInParent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitiviyX * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitiviyY * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90, 90);

        if (wallRun.foundWall && !playerMovement.isGrounded) // On wall, should be independent
        {
            tilt += Time.deltaTime * 100.0f;
            tilt = Mathf.Clamp(tilt, 0f, 12.5f);
            if (tilt == 12.5f)
            {
                retilt += Time.deltaTime * 5f;
                retilt = Mathf.Clamp(retilt, 0, 12.5f);
            }
            yRotation += mouseX;
            if (wallRun.wallRunRight)
            {
                transform.localRotation = Quaternion.Euler(xRotation, yRotation, -tilt + retilt);
            }
            else
            {
                transform.localRotation = Quaternion.Euler(xRotation, yRotation, tilt - retilt);
            }
        }
        else // On ground, can be close to wall or not.
        {
            if (transform.localRotation.y != 0)
            {
                playerBody.rotation = Quaternion.Euler(0f, transform.eulerAngles.y, 0f);
            }
            tilt = 0.0f;
            retilt = 0.0f;
            playerBody.Rotate(Vector3.up * mouseX);
            transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            yRotation = mouseX;
        }

        // Debug.Log((playerBody.localRotation, transform.localRotation));
        
            
    }
}
