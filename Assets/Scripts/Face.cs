
using UnityEngine;
using System.Collections;

public class Face : MonoBehaviour {

    private const float FADE_TIME = 2f;

    public enum Direction
    {
        Front = 0, Back = 1, Left = 2, Right = 3, Top = 4, Bottom = 5
    }

    public Direction FaceDirection
    {
        get;
        set;
    }

    public Face[] Neighbours
    {
        get;
        set;
    }

    private Color colour;
    
    // DEBUG COUNTER
    public float debugTime = 0f;

    void Update()
    {
        if (debugTime >= 0f)
        {
            debugTime = Mathf.Max(debugTime - Time.deltaTime, 0);
            renderer.material.color = Color.Lerp(colour, Color.white, debugTime / FADE_TIME);
        }
    }

    public void DebugColourFaces()
    {
        debugTime = FADE_TIME;

        for (int i = 0; i < 4; i++)
        {
            if (Neighbours[i])
            {
                Neighbours[i].debugTime = FADE_TIME;
            }
        }
    }

    public void Colour(Color colour)
    {
        this.colour = colour;
        renderer.material.color = colour;
    }

}
