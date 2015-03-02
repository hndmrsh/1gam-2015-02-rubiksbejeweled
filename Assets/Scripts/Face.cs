
using UnityEngine;
using System.Collections.Generic;

public class Face : MonoBehaviour {

    private const bool DEBUGGING = false;
    private const float FADE_TIME = 2f;

    private Spawner spawner;

    public Cube Cube
    {
        get;
        set;
    }

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
    private ParticleSystem particles;

    public int F
    {
        get;
        set;
    }

    public int X
    {
        get;
        set;
    }

    public int Y
    {
        get;
        set;
    }

    // DEBUG COUNTER
    public float debugTime = 0f;

    void Start()
    {
        this.spawner = GameObject.FindGameObjectWithTag("GameController").GetComponent<Spawner>();

        this.particles = GetComponentInChildren<ParticleSystem>();
        this.particles.startColor = colour;
    }

    void Update()
    {
        if (DEBUGGING && debugTime >= 0f)
        {
            debugTime = Mathf.Max(debugTime - Time.deltaTime, 0);
            renderer.material.color = Color.Lerp(colour, Color.white, debugTime / FADE_TIME);
        }
    }

    public void DebugColourFaces()
    {
        if (DEBUGGING)
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
    }

    public void Colour(Color colour)
    {
        this.colour = colour;
        renderer.material.color = colour;
    }

    public void SafeColour(Color[] possibleColours, int val, int levelsToCheck)
    {
        do
        {
            Colour(possibleColours[val]);
            val = (val + 1) % possibleColours.Length;
        } while (CheckColourSafety(new List<Face>(), colour, levelsToCheck));
    }

    private bool CheckColourSafety(List<Face> checkedFaces, Color colour, int levelsToCheck)
    {
        if (levelsToCheck == 0)
        {
            return true;
        }

        foreach (Face n in Neighbours)
        {
            if (!checkedFaces.Contains(n))
            {
                checkedFaces.Add(n);
                if (n.colour == colour)
                {
                    return false;
                }
                else
                {
                    bool childMatch = !n.CheckColourSafety(checkedFaces, colour, levelsToCheck - 1);
                    if (childMatch)
                    {
                        return false;
                    }
                }
            }
        }

        return true;
    }

    public bool MatchNeighbours(List<Face> matchedFaces)
    {
        matchedFaces.Add(this);

        foreach (Face n in Neighbours)
        {
            if (!matchedFaces.Contains(n) && n.colour == colour)
            {
                n.MatchNeighbours(matchedFaces);
            }
        }

        return matchedFaces.Count >= spawner.minimumMatchCount;
    }

    public void FaceMatched()
    {
        particles.Play();
    }

    public void CopyFaceIndex(Face from, Board boardToUpdate)
    {
        F = from.F;
        X = from.X;
        Y = from.Y;

        boardToUpdate.UpdateFace(F, X, Y, this);
    }
}
