using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour {

    public Board board;

    public LayerMask faceMask;

    public CornerCube cornerCube;
    public MidCube midCube;
    public EdgeCube edgeCube;
    
    public int boardSize;
    public float cubeSpacing;

    public Color[] colours;
    
    private Vector3 cachedAxisRotation;

    private float startX;
    private float startY;
    private float startZ;

	// Use this for initialization
	void Start () 
    {
        // initialize the 3D Faces array
        board.Init(boardSize);

        cornerCube.CalculateSize();

        startX = -((boardSize * (Cube.CubeWidth + cubeSpacing)) / 2f) + ((Cube.CubeWidth + cubeSpacing) / 2f);
        startY = -((boardSize * (Cube.CubeHeight+ cubeSpacing)) / 2f) + ((Cube.CubeHeight + cubeSpacing) / 2f);
        startZ = -((boardSize * (Cube.CubeDepth + cubeSpacing)) / 2f) + ((Cube.CubeDepth + cubeSpacing) / 2f);

        SpawnCubes(startX, startY, startZ);
        SpawnAxes(startX, startY, startZ);

        ColourFaces();
        GenerateFaceAdjacencies();
	}

    private void SpawnCubes(float startX, float startY, float startZ)
    {
        board.Cubes = new Cube[boardSize, boardSize, boardSize];

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

                    Cube spawned = GetCubeType(x,y,z).Spawn(pos, board, boardSize, new Vector3(x, y, z));
                    board.Cubes[x, y, z] = spawned;

                    GenerateFacesForCube(pos, x, y, z);
                }
            }
        }
    }

    public void GenerateFacesForCube(Vector3 pos, int x, int y, int z)
    {
        Vector3 origin = pos;
        if (z == 0)
        {
            origin.z += -1;
            Face f = RaycastToFace(origin, Vector3.forward);
            board.Faces[(int)Face.Direction.Front][x][y] = f;
            f.FaceDirection = Face.Direction.Front;
        }
        else if (z == boardSize - 1)
        {
            origin.z += 1;
            Face f = RaycastToFace(origin, Vector3.back);
            board.Faces[(int)Face.Direction.Back][x][y] = f;
            f.FaceDirection = Face.Direction.Back;
        }

        origin = pos;
        if (x == 0)
        {
            origin.x += -1;
            Face f = RaycastToFace(origin, Vector3.right);
            board.Faces[(int)Face.Direction.Left][z][y] = f;
            f.FaceDirection = Face.Direction.Left;
        }
        else if (x == boardSize - 1)
        {
            origin.x += 1;
            Face f = RaycastToFace(origin, Vector3.left);
            board.Faces[(int)Face.Direction.Right][z][y] = f;
            f.FaceDirection = Face.Direction.Right;
        }

        origin = pos;
        if (y == 0)
        {
            origin.y += -1;
            Face f = RaycastToFace(origin, Vector3.up);
            board.Faces[(int)Face.Direction.Bottom][x][z] = f;
            f.FaceDirection = Face.Direction.Bottom;
        }
        else if (y == boardSize - 1)
        {
            origin.y += 1;
            Face f = RaycastToFace(origin, Vector3.down);
            board.Faces[(int)Face.Direction.Top][x][z] = f;
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
        board.Axes = new GameObject[3][];
        for (int a = 0; a < board.Axes.Length; a++)
        {
            board.Axes[a] = new GameObject[boardSize];
        }

        for (int i = 0; i < boardSize; i++)
        {
            board.Axes[(int)Board.Axis.X][i] = CreateEmpty("Axis: x" + i, Vector3.right * (startX + ((Cube.CubeWidth + cubeSpacing) * i)));
            board.Axes[(int)Board.Axis.Y][i] = CreateEmpty("Axis: y" + i, Vector3.up * (startY + ((Cube.CubeHeight + cubeSpacing) * i)));
            board.Axes[(int)Board.Axis.Z][i] = CreateEmpty("Axis: z" + i, Vector3.forward * (startZ + ((Cube.CubeDepth + cubeSpacing) * i)));
        }
    }

    private GameObject CreateEmpty(string name, Vector3 position)
    {
        GameObject empty = new GameObject(name);
        empty.transform.position = position;
        empty.transform.parent = board.transform;
        return empty;
    }

    private void ColourFaces()
    {
        for (int f = 0; f < 6; f++)
        {
            for (int x = 0; x < boardSize; x++)
            {
                for (int y = 0; y < boardSize; y++)
                {
                    board.Faces[f][x][y].Colour(colours[(int)(Random.value * colours.Length)]);
                }
            }
        }
    }

    # region Face adjacencies

    public void GenerateFaceAdjacencies()
    {
        for (int f = 0; f < 6; f++)
        {
            for (int x = 0; x < boardSize; x++)
            {
                for (int y = 0; y < boardSize; y++)
                {
                    Face face = board.Faces[f][x][y];
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
        Face face = board.Faces[f][x][y];

        if (y < boardSize - 1)
        {
            return board.Faces[f][x][y + 1];
        }
        else
        {
            switch (f)
            {
                case (int)Face.Direction.Front:
                    return board.Faces[(int)Face.Direction.Top][x][0];
                case (int)Face.Direction.Left:
                    return board.Faces[(int)Face.Direction.Top][0][x];
                case (int)Face.Direction.Right:
                    return board.Faces[(int)Face.Direction.Top][boardSize - 1][x];
                case (int)Face.Direction.Back:
                    return board.Faces[(int)Face.Direction.Top][x][boardSize - 1];
                case (int)Face.Direction.Top:
                    return board.Faces[(int)Face.Direction.Back][x][boardSize - 1];
                case (int)Face.Direction.Bottom:
                    return board.Faces[(int)Face.Direction.Back][x][0];
            }
        }


        return null;
    }

    private Face GetFaceDown(int f, int x, int y)
    {
        Face face = board.Faces[f][x][y];

        if (y > 0)
        {
            return board.Faces[f][x][y - 1];
        }
        else
        {
            switch (f)
            {
                case (int)Face.Direction.Front:
                    return board.Faces[(int)Face.Direction.Bottom][x][0];
                case (int)Face.Direction.Left:
                    return board.Faces[(int)Face.Direction.Bottom][0][x];
                case (int)Face.Direction.Right:
                    return board.Faces[(int)Face.Direction.Bottom][boardSize - 1][x];
                case (int)Face.Direction.Back:
                    return board.Faces[(int)Face.Direction.Bottom][x][boardSize - 1];
                case (int)Face.Direction.Top:
                    return board.Faces[(int)Face.Direction.Front][x][boardSize - 1];
                case (int)Face.Direction.Bottom:
                    return board.Faces[(int)Face.Direction.Front][x][0];
            }
        }

        return null;
    }

    private Face GetFaceLeft(int f, int x, int y)
    {
        Face face = board.Faces[f][x][y];

        if (x > 0)
        {
            return board.Faces[f][x - 1][y];
        }
        else
        {
            switch (f)
            {
                case (int)Face.Direction.Front:
                    return board.Faces[(int)Face.Direction.Left][0][y];
                case (int)Face.Direction.Left:
                    return board.Faces[(int)Face.Direction.Front][0][y];
                case (int)Face.Direction.Right:
                    return board.Faces[(int)Face.Direction.Front][boardSize - 1][y];
                case (int)Face.Direction.Back:
                    return board.Faces[(int)Face.Direction.Left][boardSize - 1][y];
                case (int)Face.Direction.Top:
                    return board.Faces[(int)Face.Direction.Left][y][boardSize - 1];
                case (int)Face.Direction.Bottom:
                    return board.Faces[(int)Face.Direction.Left][y][0];
            }
        }

        return null;
    }

    private Face GetFaceRight(int f, int x, int y)
    {
        Face face = board.Faces[f][x][y];

        if (x < boardSize - 1)
        {
            return board.Faces[f][x + 1][y];
        }
        else
        {
            switch (f)
            {
                case (int)Face.Direction.Front:
                    return board.Faces[(int)Face.Direction.Right][0][y];
                case (int)Face.Direction.Left:
                    return board.Faces[(int)Face.Direction.Back][0][y];
                case (int)Face.Direction.Right:
                    return board.Faces[(int)Face.Direction.Back][boardSize - 1][y];
                case (int)Face.Direction.Back:
                    return board.Faces[(int)Face.Direction.Right][boardSize - 1][y];
                case (int)Face.Direction.Top:
                    return board.Faces[(int)Face.Direction.Right][y][boardSize - 1];
                case (int)Face.Direction.Bottom:
                    return board.Faces[(int)Face.Direction.Right][y][0];
            }
        }

        return null;
    }

    # endregion


    //  TODO This is probably dodgy shit and should be removed.

    public void UpdateFaces()
    {
        for (int x = 0; x < boardSize; x++)
        {
            for (int y = 0; y < boardSize; y++)
            {
                for (int z = 0; z < boardSize; z++)
                {

                    if (board.Cubes[x, y, z])
                    {
                        Vector3 xVector = Vector3.right * (startX + ((Cube.CubeWidth + cubeSpacing) * x));
                        Vector3 yVector = Vector3.up * (startY + ((Cube.CubeHeight + cubeSpacing) * y));
                        Vector3 zVector = Vector3.forward * (startZ + ((Cube.CubeDepth + cubeSpacing) * z));
                        Vector3 pos = xVector + yVector + zVector;

                        GenerateFacesForCube(pos, x, y, z);
                    }
                }
            }
        }

        GenerateFaceAdjacencies();
    }
}
