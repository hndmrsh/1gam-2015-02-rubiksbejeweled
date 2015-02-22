using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour {

    public enum Axis
    {
        X = 0, Y = 1, Z = 2
    }

    public GameObject board;

    public LayerMask faceMask;

    public CornerCube cornerCube;
    public MidCube midCube;
    public EdgeCube edgeCube;
    
    public int boardSize;
    public float cubeSpacing;

    public Color[] colours;

    private Cube[,,] cubes;

    /// <summary>
    /// Usage: faces[face][x][y];
    /// The faces array is set up as follows:
    /// face = a Face.Direction enum
    /// x, y = the x/y index of the face if you were to look at the face "front on"
    /// </summary>
    private Face[][][] faces;

    private GameObject[][] axes;

    private Vector3 cachedAxisRotation;

    private float startX;
    private float startY;
    private float startZ;

	// Use this for initialization
	void Start () 
    {
        // initialize the 3D faces array
        faces = new Face[6][][];
        for (int i = 0; i < 6; i++)
        {
            faces[i] = new Face[boardSize][];
            for (int j = 0; j < boardSize; j++)
            {
                faces[i][j] = new Face[boardSize];
            }
        }

        cornerCube.CalculateSize();

        startX = -((boardSize * (Cube.CubeWidth + cubeSpacing)) / 2f) + ((Cube.CubeWidth + cubeSpacing) / 2f);
        startY = -((boardSize * (Cube.CubeHeight+ cubeSpacing)) / 2f) + ((Cube.CubeHeight + cubeSpacing) / 2f);
        startZ = -((boardSize * (Cube.CubeDepth + cubeSpacing)) / 2f) + ((Cube.CubeDepth + cubeSpacing) / 2f);

        SpawnCubes(startX, startY, startZ);
        SpawnAxes(startX, startY, startZ);

        GenerateFaceAdjacencies();
	}

    private void GenerateFaceAdjacencies()
    {
        for (int f = 0; f < 6; f++)
        {
            for (int x = 0; x < boardSize; x++)
            {
                for (int y = 0; y < boardSize; y++)
                {
                    Face face = faces[f][x][y];
                    face.Neighbours = new Face[4];

                    face.Neighbours[0] = GetFaceUp(f, x, y);
                    face.Neighbours[1] = GetFaceLeft(f, x, y);
                    face.Neighbours[2] = GetFaceRight(f, x, y);
                    face.Neighbours[3] = GetFaceDown(f, x, y);
                }
            }
        }
    }

    private Face GetFaceUp(int f, int x, int y)
    {
        Face face = faces[f][x][y];
        
        if (y < boardSize - 1)
        {
            return faces[f][x][y + 1];
        }
        else
        {
            switch (f)
            {
                case (int)Face.Direction.Front:
                    return faces[(int)Face.Direction.Top][x][0];
                case (int)Face.Direction.Left:
                    return faces[(int)Face.Direction.Top][0][boardSize - 1 - x];
                case (int)Face.Direction.Right:
                    return faces[(int)Face.Direction.Top][boardSize - 1][x];
                case (int)Face.Direction.Back:
                    return faces[(int)Face.Direction.Top][boardSize - 1 - x][boardSize - 1];
                case (int)Face.Direction.Top:
                    return faces[(int)Face.Direction.Back][boardSize - 1 - x][boardSize - 1];
                case (int)Face.Direction.Bottom:
                    return faces[(int)Face.Direction.Front][x][0];
            }
        }


        return null;
    }

    private Face GetFaceDown(int f, int x, int y)
    {
        Face face = faces[f][x][y];

        if (y > 0)
        {
            return faces[f][x][y - 1];
        }
        else
        {
            switch (f)
            {
                case (int) Face.Direction.Front:
                    return faces[(int)Face.Direction.Bottom][x][boardSize - 1];
                case (int) Face.Direction.Left:
                    return faces[(int)Face.Direction.Bottom][0][x];
                case (int)Face.Direction.Right:
                    return faces[(int)Face.Direction.Bottom][boardSize - 1][boardSize - 1 - x];
                case (int)Face.Direction.Back:
                    return faces[(int)Face.Direction.Bottom][boardSize - 1 - x][0];
                case (int)Face.Direction.Top:
                    return faces[(int)Face.Direction.Front][x][boardSize - 1];
                case (int)Face.Direction.Bottom:
                    return faces[(int)Face.Direction.Back][boardSize - 1 - x][0];
            }
        }

        return null;
    }

    private Face GetFaceLeft(int f, int x, int y)
    {
        Face face = faces[f][x][y];

        if (x > 0)
        {
            return faces[f][x - 1][y];
        }
        else
        {
            switch (f)
            {
                case (int)Face.Direction.Front:
                    return faces[(int)Face.Direction.Left][boardSize - 1][y];
                case (int)Face.Direction.Left:
                    return faces[(int)Face.Direction.Back][boardSize - 1][y];
                case (int)Face.Direction.Right:
                    return faces[(int)Face.Direction.Front][boardSize - 1][y];
                case (int)Face.Direction.Back:
                    return faces[(int)Face.Direction.Right][boardSize - 1][y];
                case (int)Face.Direction.Top:
                    return faces[(int)Face.Direction.Left][boardSize - 1 - y][boardSize - 1];
                case (int)Face.Direction.Bottom:
                    return faces[(int)Face.Direction.Left][y][0];
            }
        }

        return null;
    }

    private Face GetFaceRight(int f, int x, int y)
    {
        Face face = faces[f][x][y];

        if (x < boardSize - 1)
        {
            return faces[f][x + 1][y];
        }
        else
        {
            switch (f)
            {
                case (int)Face.Direction.Front:
                    return faces[(int)Face.Direction.Right][0][y];
                case (int)Face.Direction.Left:
                    return faces[(int)Face.Direction.Front][0][y];
                case (int)Face.Direction.Right:
                    return faces[(int)Face.Direction.Back][0][y];
                case (int)Face.Direction.Back:
                    return faces[(int)Face.Direction.Left][0][y];
                case (int)Face.Direction.Top:
                    return faces[(int)Face.Direction.Right][y][boardSize - 1];
                case (int)Face.Direction.Bottom:
                    return faces[(int)Face.Direction.Right][boardSize - 1 - y][0];
            }
        }

        return null;
    }

    private void SpawnCubes(float startX, float startY, float startZ)
    {
        cubes = new Cube[boardSize, boardSize, boardSize];

        for (int x = 0; x < boardSize; x++)
        {
            for (int y = 0; y < boardSize; y++)
            {
                for (int z = 0; z < boardSize; z++)
                {

                    if (x > 0 && y > 0 && x < boardSize - 1 && y < boardSize - 1 && z > 0 && z < boardSize - 1)
                    {
                        continue;
                    }

                    Vector3 xVector = Vector3.right * (startX + ((Cube.CubeWidth + cubeSpacing) * x));
                    Vector3 yVector = Vector3.up * (startY + ((Cube.CubeHeight + cubeSpacing) * y));
                    Vector3 zVector = Vector3.forward * (startZ + ((Cube.CubeDepth + cubeSpacing) * z));
                    Vector3 pos = xVector + yVector + zVector;

                    Cube spawned = GetCubeType(x,y,z).Spawn(pos, colours, board, boardSize, new Vector3(x, y, z));
                    cubes[x, y, z] = spawned;

                    GenerateFacesForCube(pos, x, y, z);
                }
            }
        }
    }

    private void GenerateFacesForCube(Vector3 pos, int x, int y, int z)
    {
        Vector3 origin = pos;
        if (z == 0)
        {
            origin.z += -1;
            Face f = RaycastToFace(origin, Vector3.forward);
            faces[(int)Face.Direction.Front][x][y] = f;
            f.FaceDirection = Face.Direction.Front;
        }
        else if (z == boardSize - 1)
        {
            origin.z += 1;
            Face f = RaycastToFace(origin, Vector3.back);
            faces[(int)Face.Direction.Back][boardSize - 1 - x][y] = f;
            f.FaceDirection = Face.Direction.Back;
        }

        origin = pos;
        if (x == 0)
        {
            origin.x += -1;
            Face f = RaycastToFace(origin, Vector3.right);
            faces[(int)Face.Direction.Left][boardSize - 1 - z][y] = f;
            f.FaceDirection = Face.Direction.Left;
        }
        else if (x == boardSize - 1)
        {
            origin.x += 1;
            Face f = RaycastToFace(origin, Vector3.left);
            faces[(int)Face.Direction.Right][z][y] = f;
            f.FaceDirection = Face.Direction.Right;
        }

        origin = pos;
        if (y == 0)
        {
            origin.y += -1;
            Face f = RaycastToFace(origin, Vector3.up);
            faces[(int)Face.Direction.Bottom][x][boardSize - 1 - z] = f;
            f.FaceDirection = Face.Direction.Bottom;
        }
        else if (y == boardSize - 1)
        {
            origin.y += 1;
            Face f = RaycastToFace(origin, Vector3.down);
            faces[(int)Face.Direction.Top][x][z] = f;
            f.FaceDirection = Face.Direction.Top;
        }
    }

    private Face RaycastToFace(Vector3 origin, Vector3 direction)
    {
        Ray screenRay = new Ray(origin, direction);

        RaycastHit hitInfo;
        Physics.Raycast(screenRay, out hitInfo, 1.1f, faceMask);

        if (hitInfo.transform)
        {
            return hitInfo.transform.gameObject.GetComponent<Face>();
        }

        return null;
    }

    private Cube GetCubeType(int x, int y, int z)
    {
        int endCount = 0;

        if (x == 0 || x == boardSize - 1)
        {
            endCount++;
        }

        if (y == 0 || y == boardSize - 1)
        {
            endCount++;
        }

        if (z == 0 || z == boardSize - 1)
        {
            endCount++;
        }

        switch (endCount)
        {
            case 1:
                return midCube;
            case 2:
                return edgeCube;
            case 3:
                return cornerCube;
            default:
                throw new System.InvalidOperationException("Impossible location");
        }
    }

    private void SpawnAxes(float startX, float startY, float startZ)
    {
        axes = new GameObject[3][];
        for (int a = 0; a < axes.Length; a++)
        {
            axes[a] = new GameObject[boardSize];
        }

        for (int i = 0; i < boardSize; i++)
        {
            axes[(int)Axis.X][i] = CreateEmpty("Axis: x" + i, Vector3.right * (startX + ((Cube.CubeWidth + cubeSpacing) * i)));
            axes[(int)Axis.Y][i] = CreateEmpty("Axis: y" + i, Vector3.up * (startY + ((Cube.CubeHeight + cubeSpacing) * i)));
            axes[(int)Axis.Z][i] = CreateEmpty("Axis: z" + i, Vector3.forward * (startZ + ((Cube.CubeDepth + cubeSpacing) * i)));
        }
    }

    private GameObject CreateEmpty(string name, Vector3 position)
    {
        GameObject empty = new GameObject(name);
        empty.transform.position = position;
        empty.transform.parent = board.transform;
        return empty;
    }

    #region Cube mapping

    public GameObject MapCubesToAxis(Axis axis, int index, out Cube[] mappedChildrenCubes) {
        GameObject ax = axes[(int)axis][index];
        SetParentAllCubes(axis, index, ax, out mappedChildrenCubes);

        cachedAxisRotation = ax.transform.localEulerAngles;

        return ax;
    }

    public void UnmapCubesFromAxis(Axis axis, int index, out Cube[] mappedChildrenCubes)
    {
        SetParentAllCubes(axis, index, board, out mappedChildrenCubes);

        GameObject ax = axes[(int)axis][index];
        Vector3 diff = ax.transform.localEulerAngles - cachedAxisRotation;

        Logger.SetValue("diff", (Mathf.RoundToInt(diff.x) / 90).ToString());
    }

    private void SetParentAllCubes(Axis axis, int index, GameObject newParent, out Cube[] mappedChildrenCubes)
    {
        if (index == 0 || index == boardSize - 1) 
        {
            mappedChildrenCubes = new Cube[boardSize * boardSize];
        }
        else
        {
            mappedChildrenCubes = new Cube[boardSize * 4 - 4];
        }

        int childIndex = 0;
        for (int i = 0; i < boardSize; i++)
        {
            for (int j = 0; j < boardSize; j++)
            {
                Cube child = null;

                switch (axis)
                {
                    case Axis.X:
                        child = cubes[index, i, j];
                        break;
                    case Axis.Y:
                        child = cubes[i, index, j];
                        break;
                    case Axis.Z:
                        child = cubes[i, j, index];
                        break;
                }

                if (child)
                {
                    mappedChildrenCubes[childIndex] = child;
                    childIndex++;

                    child.transform.parent = newParent.transform;
                }
            }
        }
    }
    
    #endregion

    /// <summary>
    /// Determine the correct axis to rotate the selected cubes around.
    /// </summary>
    /// <param name="dragAngle"></param>
    /// <param name="cubeTouchIndex"></param>
    /// <returns></returns>
    public Axis DetermineAxis(float dragAngle, Face.Direction faceDirection, out Vector2 directionCorrection) {
        switch (faceDirection)
        {
            case Face.Direction.Front:
            case Face.Direction.Back:
                directionCorrection = new Vector2(1, -1);
                if (dragAngle < 45 || dragAngle > 135)
                {
                    return Spawner.Axis.Y;
                }
                else
                {
                    return Spawner.Axis.X;
                }
            case Face.Direction.Left:
            case Face.Direction.Right:
                directionCorrection = new Vector2(-1, -1);
                if (dragAngle < 45 || dragAngle > 135)
                {
                    return Spawner.Axis.Y;
                }
                else
                {
                    return Spawner.Axis.Z;
                }
            case Face.Direction.Top:
            case Face.Direction.Bottom:
            default:
                directionCorrection = new Vector2(1, -1);
                if (dragAngle < 45 || dragAngle > 135)
                {
                    return Spawner.Axis.Z;
                }
                else
                {
                    return Spawner.Axis.X;
                }
        }
    }
}
