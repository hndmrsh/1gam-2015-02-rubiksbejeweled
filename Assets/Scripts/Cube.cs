using UnityEngine;
using System.Collections;

public class Cube : MonoBehaviour {

    private GameObject gameController;

    public static float CubeWidth {
        get;
        set;
    }

    public static float CubeHeight
    {
        get;
        set;
    }

    public static float CubeDepth
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

    public void CalculateSize()
    {
        CubeWidth = renderer.bounds.size.x;
        CubeHeight = renderer.bounds.size.y;
        CubeDepth = renderer.bounds.size.z;
    }

    public Cube Spawn(Vector3 position, Color colour, GameObject board, Vector3 index)
    {
        GameObject spawnedCube = (GameObject) Instantiate(gameObject, position, Quaternion.identity);
        
        spawnedCube.transform.parent = board.transform;
        //spawnedCube.renderer.material.color = colour;
        spawnedCube.name = "Cube: (" + (int)index.x + "," + (int)index.y + "," + (int)index.z + ")";

        Cube newCube = spawnedCube.GetComponent<Cube>();
        newCube.Index = index;
        newCube.Colour = colour;



        return newCube;
    }

}
