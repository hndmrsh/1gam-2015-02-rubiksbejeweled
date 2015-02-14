using UnityEngine;
using System.Collections;

public class Cube : MonoBehaviour {

    private GameObject gameController;

    public float CubeWidth {
        get;
        set;
    }

    public float CubeHeight
    {
        get;
        set;
    }

    public float CubeDepth
    {
        get;
        set;
    }

    public Vector3 Index
    {
        get;
        set;
    }

    public Color Colour
    {
        get;
        set;
    }

    void Start()
    {
        this.gameController = GameObject.FindGameObjectWithTag("GameController");
    }

    public Cube Spawn(Vector3 position, Color colour, GameObject board, Vector3 index)
    {
        
        if(CubeWidth == 0f || CubeHeight == 0f || CubeDepth == 0f) {
            CubeWidth = renderer.bounds.size.x;
            CubeHeight = renderer.bounds.size.y;
            CubeDepth = renderer.bounds.size.z;
        }

        GameObject spawnedCube = (GameObject) Instantiate(gameObject, position, Quaternion.identity);

        spawnedCube.transform.parent = board.transform;
        spawnedCube.renderer.material.color = colour;
        spawnedCube.name = "Cube: (" + (int)index.x + "," + (int)index.y + "," + (int)index.z + ")";

        Cube newCube = spawnedCube.GetComponent<Cube>();
        newCube.Index = index;
        newCube.Colour = colour;

        return newCube;
    }

}
