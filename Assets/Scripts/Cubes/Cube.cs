using UnityEngine;
using System.Collections;

public abstract class Cube : MonoBehaviour
{

    #region Child face constants
    public const string xFace = "XGlobalFace";
    public const string yFace = "YGlobalFace";
    public const string zFace = "ZGlobalFace";
    #endregion

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

    public Cube Spawn(Vector3 position, Board board, int boardSize, Vector3 index)
    {
        int x = (int)index.x;
        int y = (int)index.y;
        int z = (int)index.z;

        GameObject spawnedCube = (GameObject) Instantiate(gameObject, position, CalculateRotation(x, y, z, boardSize));
        spawnedCube.transform.parent = board.gameObject.transform;

        spawnedCube.name = "Cube: (" + x + "," + y + "," + z + ")";

        Cube newCube = spawnedCube.GetComponent<Cube>();
        newCube.Index = index;

        foreach (Face f in spawnedCube.GetComponentsInChildren<Face>())
        {
            f.gameObject.name += ": (" + x + "," + y + "," + z + ")";
        }
        
        return newCube;
    }

    protected abstract Quaternion CalculateRotation(int x, int y, int z, int boardSize);

}
