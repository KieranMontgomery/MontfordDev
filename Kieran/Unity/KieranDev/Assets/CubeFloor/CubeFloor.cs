using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeFloor : MonoBehaviour
{
    [Range(1, 100)]
    public int dimension;

    public GameObject CubeColour1;
    public GameObject CubeColour2;

    // Start is called before the first frame update
    void Start()
    {
        // Main loop to create
        for (int i = 0; i < dimension; i++)
        {
            for (int j = 0; j < dimension; j++)
            {

                Vector3 position = new Vector3(i, 0f, j);

                if ((i + j) % 2 == 0)
                {
                    Instantiate(CubeColour1, position, Quaternion.identity);
                }
                else
                {
                    Instantiate(CubeColour2, position, Quaternion.identity);
                }
            }
        }
    }
}
