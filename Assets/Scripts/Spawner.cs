using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour {

    public GameObject board;
    public GameObject cube;
    public int boardSize;
    public float cubeSpacing;

    private float cubeWidth, cubeHeight, cubeDepth;

	// Use this for initialization
	void Start () {
        cubeWidth = cube.renderer.bounds.size.x;
        cubeHeight = cube.renderer.bounds.size.y;
        cubeDepth = cube.renderer.bounds.size.z;

        float startX = -((boardSize * cubeWidth) / 2f) + (cubeWidth / 2f);
        float startY = -((boardSize * cubeHeight) / 2f) + (cubeHeight / 2f);
        float startZ = -((boardSize * cubeDepth) / 2f) + (cubeDepth / 2f);

        for (int x = 0; x < boardSize; x++)
        {
            for (int y = 0; y < boardSize; y++)
            {
                for (int z = 0; z < boardSize; z++)
                {
                    if(x > 0 && y > 0 && x < boardSize - 1 && y < boardSize - 1 && z > 0 && z < boardSize - 1) {
                        continue;
                    }

                    Vector3 xVector = Vector3.right * (startX + ((cubeWidth + cubeSpacing) * x));
                    Vector3 yVector = Vector3.up * (startY + ((cubeHeight + cubeSpacing) * y));
                    Vector3 zVector = Vector3.forward * (startZ + ((cubeDepth + cubeSpacing) * z));
                    Vector3 pos = xVector + yVector + zVector;
                    
                    GameObject spawnedCube = (GameObject) Instantiate(cube, pos, Quaternion.identity);
                    spawnedCube.transform.parent = board.transform;
                }
            }
        }
	}
	
	// Update is called once per frame
	void Update () {
	}

}
