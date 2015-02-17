using UnityEngine;
using System.Collections;

public class MidCube : Cube {

    protected override void ColourFaces(Cube spawned, Color[] colourChoices)
    {
        spawned.transform.FindChild(zFace).renderer.material.color = colourChoices[(int)(Random.value * colourChoices.Length)];
    }


    protected override Quaternion CalculateRotation(int x, int y, int z, int boardSize)
    {
        Vector3 rotation = new Vector3();

        // rotate around Y
        if (y > 0 && y < boardSize - 1)
        {
            if (x == boardSize - 1)
            {
                rotation += Vector3.down * 90f;
            }
            else if (x == 0)
            {
                rotation += Vector3.down * 270f;
            }
            else if (z == boardSize - 1)
            {
                rotation += Vector3.down * 180f;
            }
        }
        else if (y == 0)
        {
            rotation += Vector3.left * 90f;
        }
        else if (y == boardSize - 1)
        {
            rotation += Vector3.right * 90f;
        }

        return Quaternion.Euler(rotation);
    }
}
