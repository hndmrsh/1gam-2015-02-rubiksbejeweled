using UnityEngine;
using System.Collections;

public class CornerCube : Cube {

    protected override Quaternion CalculateRotation(int x, int y, int z, int boardSize)
    {
        Vector3 rotation = new Vector3();

        // rotate around Y
        if (x == boardSize - 1)
        {
            if (z == boardSize - 1)
            {
                rotation += Vector3.down * 180f;
            }
            else if (z == 0)
            {
                rotation += Vector3.down * 90f;
            }
        }
        else if (x == 0)
        {
            if (z == boardSize - 1)
            {
                rotation += Vector3.down * 270f;
            }
            else if (z == 0)
            {
                rotation += Vector3.down * 0f;
            }
        }
        
        // rotate around X
        if (y == 0)
        {
            rotation += Vector3.left * 90f;
        }

        return Quaternion.Euler(rotation);
    }

}