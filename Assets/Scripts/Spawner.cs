using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour {

    public enum Axis
    {
        X = 0, Y = 1, Z = 2
    }

    public GameObject board;
    public Cube cube;
    public int boardSize;
    public float cubeSpacing;

    public Color[] colours;

    private Cube[,,] cubes;
    private GameObject[][] axes;

	// Use this for initialization
	void Start () 
    {
        cube.CalculateSize();

        float startX = -((boardSize * Cube.CubeWidth) / 2f) + (Cube.CubeWidth / 2f);
        float startY = -((boardSize * Cube.CubeHeight) / 2f) + (Cube.CubeHeight / 2f);
        float startZ = -((boardSize * Cube.CubeDepth) / 2f) + (Cube.CubeDepth / 2f);

        SpawnCubes(startX, startY, startZ);
        SpawnAxes(startX, startY, startZ);
       
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

                    Cube spawned = cube.Spawn(pos, colours[Random.Range(0, colours.Length)], board, new Vector3(x, y, z));
                    cubes[x, y, z] = spawned;
                }
            }
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
        return ax;
    }

    public void UnmapCubesFromAxis(Axis axis, int index, out Cube[] mappedChildrenCubes)
    {
        SetParentAllCubes(axis, index, board, out mappedChildrenCubes);
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

}
