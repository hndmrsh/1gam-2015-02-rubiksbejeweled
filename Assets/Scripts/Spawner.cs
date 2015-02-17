﻿using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour {

    public enum Axis
    {
        X = 0, Y = 1, Z = 2
    }

    public GameObject board;

    public CornerCube cornerCube;
    public MidCube midCube;
    public EdgeCube edgeCube;
    
    public int boardSize;
    public float cubeSpacing;

    public Color[] colours;

    private Cube[,,] cubes;
    private Face[][][] faces;

    private GameObject[][] axes;

    private Vector3 cachedAxisRotation;

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

        float startX = -((boardSize * (Cube.CubeWidth + cubeSpacing)) / 2f) + ((Cube.CubeWidth + cubeSpacing) / 2f);
        float startY = -((boardSize * (Cube.CubeHeight+ cubeSpacing)) / 2f) + ((Cube.CubeHeight + cubeSpacing) / 2f);
        float startZ = -((boardSize * (Cube.CubeDepth + cubeSpacing)) / 2f) + ((Cube.CubeDepth + cubeSpacing) / 2f);


        SpawnCubes(startX, startY, startZ);
        SpawnAxes(startX, startY, startZ);

        // TODO: populate the faces array by raycasting at every face on the cube

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
                }
            }
        }
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

}
