using UnityEngine;
using System.Collections;

public class EdgeCube : Cube {

    protected override Quaternion CalculateRotation(int x, int y, int z, int boardSize)
    {
        Vector3 rotation = new Vector3();

        if (x == 0 && z == boardSize - 1)
        {
            rotation += Vector3.up * 90f;
        }
        else if (x == boardSize - 1 && z == 0)
        {
            rotation += Vector3.up * 270f;
        }
        else if (z == boardSize - 1)
        {
            rotation += Vector3.up * 180f;
            if (y == 0)
            {
                rotation += Vector3.forward * 90f;
            }
            else if (y == boardSize - 1)
            {
                rotation += Vector3.forward * 270f;
            }
        }
        else if (y == 0)
        {
            rotation += Vector3.forward * 90f;
        }
        else if (y == boardSize - 1)
        {
            rotation += Vector3.forward * 270f;
        }

        if (z > 0 && z < boardSize - 1)
        {
            if (x == 0)
            {
                rotation += Vector3.up * 90f;
            }
            else if (x == boardSize - 1)
            {
                rotation += Vector3.up * 270f;
            }
        }

        return Quaternion.Euler(rotation);
    }
}
