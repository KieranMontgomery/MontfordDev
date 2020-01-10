using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeFloor : MonoBehaviour
{
    [Range(1, 100)]
    public int dimension;
    [Range(1, 20)]
    public int numberOfStairs;

    public GameObject CubeColour1;
    public GameObject CubeColour2;

    // Start is called before the first frame update
    void Start()
    {
        // Create floor.
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

        // Create stairs.

        int stairHeight = 5;
        System.Random random = new System.Random();

        for (int p = 0; p < numberOfStairs + 1; p++)
        {
            int ranx = random.Next(2, dimension - 2);
            int ranz = random.Next(2, dimension - 2);
            int direction = random.Next(1, 5);

            for (int i = 1; i < stairHeight + 1; i++)
            {
                for (int j = 0; j < i; j++)
                {
                    for (int k = 0; k < 2; k++)
                    {
                        Vector3 position = Vector3.zero;
                        if (direction == 1)
                        {
                            position += new Vector3(ranx + i, j / 2.0f + 0.75f, ranz + k);
                        }
                        else if (direction == 2)
                        {
                            position += new Vector3(ranx - i, j / 2.0f + 0.75f, ranz + k);
                        }
                        else if (direction == 3)
                        {
                            position += new Vector3(ranz + k, j / 2.0f + 0.75f, ranx - i);
                        }
                        else if (direction == 4)
                        {
                            position += new Vector3(ranz + k, j / 2.0f + 0.75f, ranx + i);
                        }
                        if ((i + j) % 2 == 0)
                        {
                            GameObject cube = Instantiate(CubeColour1, position, Quaternion.identity) as GameObject;
                            cube.transform.localScale = new Vector3(1f, 0.5f, 1f);
                        }
                        else
                        {
                            GameObject cube = Instantiate(CubeColour2, position, Quaternion.identity) as GameObject;
                            cube.transform.localScale = new Vector3(1f, 0.5f, 1f);
                        }
                    }
                }
            }
        }
    }
}
