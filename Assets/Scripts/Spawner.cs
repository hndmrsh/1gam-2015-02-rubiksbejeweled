using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour {

    public GameObject board;
    public Cube cube;
    public int boardSize;
    public float cubeSpacing;

    public Color[] colours;

	// Use this for initialization
	void Start () {

        // SPAWN CUBES
        float startX = -((boardSize * cube.CubeWidth) / 2f) + (cube.CubeWidth / 2f);
        float startY = -((boardSize * cube.CubeHeight) / 2f) + (cube.CubeHeight / 2f);
        float startZ = -((boardSize * cube.CubeDepth) / 2f) + (cube.CubeDepth / 2f);

        for (int x = 0; x < boardSize; x++)
        {
            for (int y = 0; y < boardSize; y++)
            {
                for (int z = 0; z < boardSize; z++)
                {

                    if(x > 0 && y > 0 && x < boardSize - 1 && y < boardSize - 1 && z > 0 && z < boardSize - 1) {
                        continue;
                    }

                    Vector3 xVector = Vector3.right * (startX + ((cube.CubeWidth + cubeSpacing) * x));
                    Vector3 yVector = Vector3.up * (startY + ((cube.CubeHeight + cubeSpacing) * y));
                    Vector3 zVector = Vector3.forward * (startZ + ((cube.CubeDepth + cubeSpacing) * z));
                    Vector3 pos = xVector + yVector + zVector;

                    cube.Spawn(pos, colours[Random.Range(0, colours.Length)], board, new Vector3(x, y, z));
                }
            }
        }

        // SPAWN AXES

        for (int i = 0; i < boardSize; i++)
        {
            CreateEmpty("Axis: x" + i, Vector3.right * (startX + ((cube.CubeWidth + cubeSpacing) * i)));
            CreateEmpty("Axis: y" + i, Vector3.up * (startY + ((cube.CubeHeight + cubeSpacing) * i)));
            CreateEmpty("Axis: z" + i, Vector3.forward * (startZ + ((cube.CubeDepth + cubeSpacing) * i)));
        }
	}

    private GameObject CreateEmpty(string name, Vector3 position)
    {
        GameObject empty = new GameObject(name);
        empty.transform.position = position;
        empty.transform.parent = this.transform;
        return empty;
    }
	
	// Update is called once per frame
	void Update () {
	}

}
