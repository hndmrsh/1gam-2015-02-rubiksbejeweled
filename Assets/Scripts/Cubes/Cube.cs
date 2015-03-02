using UnityEngine;
using System.Collections;

public abstract class Cube : MonoBehaviour
{
    private const float FLY_TIME = 0.66f;

    #region Child face constants
    public const string xFace = "XGlobalFace";
    public const string yFace = "YGlobalFace";
    public const string zFace = "ZGlobalFace";
    #endregion

    private const float DESTROY_TIME = 0.5f;

    private Spawner spawner;

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

    private bool destroyed = false;
    private float timeDestroying = 0f;

    private float timeToFly = 0f;

    private int cachedBoardSize;

    private Vector3 startingPosition;
    private Vector3 targetPosition;

    void Start()
    {
        spawner = GameObject.FindGameObjectWithTag("GameController").GetComponent<Spawner>();
    }

    void Update()
    {
        if (destroyed)
        {
            timeDestroying += Time.deltaTime;
            transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, timeDestroying / DESTROY_TIME);
        }

        if (timeToFly > 0f)
        {
            timeToFly -= Time.deltaTime;
            transform.position = Vector3.Lerp(targetPosition, startingPosition, timeToFly / FLY_TIME);
        }
    }

    public void CalculateSize()
    {
        CubeWidth = renderer.bounds.size.x;
        CubeHeight = renderer.bounds.size.y;
        CubeDepth = renderer.bounds.size.z;
    }

    public Cube Spawn(Vector3 position, Board board, int boardSize, Vector3 index, bool replacingPrevious)
    {
        int x = (int)index.x;
        int y = (int)index.y;
        int z = (int)index.z;

        cachedBoardSize = boardSize;

        Vector3 spawnPos = replacingPrevious ? position * 2 : position;
        Quaternion spawnRot = replacingPrevious ? transform.rotation : CalculateRotation(x, y, z, boardSize);
        GameObject spawnedCube = (GameObject) Instantiate(gameObject, spawnPos, spawnRot);
        spawnedCube.transform.parent = board.gameObject.transform;
        
        spawnedCube.name = "Cube: (" + x + "," + y + "," + z + ")";
        
        Cube newCube = spawnedCube.GetComponent<Cube>();
        newCube.Index = index;

        spawnedCube.transform.localScale = Vector3.one;

        foreach (Face f in spawnedCube.GetComponentsInChildren<Face>())
        {
            if (!replacingPrevious)
            {
                f.gameObject.name += ": (" + x + "," + y + "," + z + ")";
            }
            else
            {
                f.Colour(spawner.colours[(int)(Random.value * spawner.colours.Length)]);
                f.CopyFaceIndex(transform.Find(f.gameObject.name).GetComponent<Face>(), board);
            }

            f.Cube = newCube;
        }

        if (replacingPrevious)
        {
            newCube.timeToFly = FLY_TIME;
            newCube.targetPosition = position;
            newCube.startingPosition = spawnPos;

            board.UpdateCube(index, newCube);
        }
        
        return newCube;
    }

    public void Destroy()
    {
        if (!destroyed)
        {
            StartCoroutine("SpawnInTime", DESTROY_TIME * 0.7);
            StartCoroutine("DestroyInTime", DESTROY_TIME);
        }
    }

    IEnumerator DestroyInTime(float time)
    {
        destroyed = true;
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }

    IEnumerator SpawnInTime(float time)
    {
        yield return new WaitForSeconds(time);
        Spawn(transform.position, transform.parent.gameObject.GetComponent<Board>(), cachedBoardSize, new Vector3(Index.x, Index.y, Index.z), true);
    }

    protected abstract Quaternion CalculateRotation(int x, int y, int z, int boardSize);
    
}
