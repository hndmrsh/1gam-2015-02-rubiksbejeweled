using UnityEngine;
using System.Collections;

public class Board : MonoBehaviour {

    public enum Axis
    {
        X = 0, Y = 1, Z = 2
    }

    public Cube[, ,] Cubes
    {
        get;
        set;
    }

    /// <summary>
    /// Usage: faces[face][x][y];
    /// The faces array is set up as follows:
    /// face = a Face.Direction enum
    /// x, y = the x/y index of the face if you were to look at the face "front on"
    /// </summary>
    public Face[][][] Faces
    {
        get;
        set;
    }

    public GameObject[][] Axes
    {
        get;
        set;
    }

    private int boardSize;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Init(int boardSize)
    {
        this.boardSize = boardSize;

        Faces = new Face[6][][];
        for (int i = 0; i < 6; i++)
        {
            Faces[i] = new Face[boardSize][];
            for (int j = 0; j < boardSize; j++)
            {
                Faces[i][j] = new Face[boardSize];
            }
        }
    }

    public void RotateAxis(Axis axis, int index, int numberTurns)
    {
        for (int i = 0; i < numberTurns; i++)
        {
            RotateCubesOnce(axis, index);
        }
    }

    private void RotateCubesOnce(Axis axis, int index)
    {
        Cube[,] original = new Cube[boardSize, boardSize];

        switch (axis)
        {
            case Axis.X:
                for (int i = 0; i < boardSize; i++)
                {
                    for (int j = 0; j < boardSize; j++)
                    {
                        original[i, j] = Cubes[index, i, j];
                    }
                }
                for (int i = 0; i < boardSize; i++)
                {
                    for (int j = 0; j < boardSize; j++)
                    {
                        Cubes[index, boardSize - j - 1, i] = original[i, j];
                        if (Cubes[index, boardSize - j - 1, i])
                        {
                            Cubes[index, boardSize - j - 1, i].Index = new Vector3(index, boardSize - j - 1, i);
                        }
                    }
                }
                break;
            case Axis.Y:
                for (int i = 0; i < boardSize; i++)
                {
                    for (int j = 0; j < boardSize; j++)
                    {
                        original[i, j] = Cubes[i, index, j];
                    }
                }
                for (int i = 0; i < boardSize; i++)
                {
                    for (int j = 0; j < boardSize; j++)
                    {
                        Cubes[j, index, boardSize - i - 1] = original[i, j];
                        if (Cubes[j, index, boardSize - i - 1])
                        {
                            Cubes[j, index, boardSize - i - 1].Index = new Vector3(j, index, boardSize - i - 1);
                        }
                    }
                }
                break;
            case Axis.Z:
                for (int i = 0; i < boardSize; i++)
                {
                    for (int j = 0; j < boardSize; j++)
                    {
                        original[i, j] = Cubes[i, j, index];
                    }
                }
                for (int i = 0; i < boardSize; i++)
                {
                    for (int j = 0; j < boardSize; j++)
                    {
                        Cubes[boardSize - j - 1, i, index] = original[i, j];
                        if (Cubes[boardSize - j - 1, i, index])
                        {
                            Cubes[boardSize - j - 1, i, index].Index = new Vector3(boardSize - j - 1, i, index);
                        }
                    }
                }
                break;


        }
    }

    #region Cube mapping

    public GameObject MapCubesToAxis(Axis axis, int index, out Cube[] mappedChildrenCubes)
    {
        GameObject ax = Axes[(int)axis][index];
        SetParentAllCubes(axis, index, ax, out mappedChildrenCubes);
        return ax;
    }

    public void UnmapCubesFromAxis(Axis axis, int index, out Cube[] mappedChildrenCubes)
    {
        SetParentAllCubes(axis, index, gameObject, out mappedChildrenCubes);

        GameObject ax = Axes[(int)axis][index];
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
                        child = Cubes[index, i, j];
                        break;
                    case Axis.Y:
                        child = Cubes[i, index, j];
                        break;
                    case Axis.Z:
                        child = Cubes[i, j, index];
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
