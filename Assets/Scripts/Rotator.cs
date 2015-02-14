using UnityEngine;
using System.Collections;

public class Rotator : MonoBehaviour, InputHandler.InputListener {

    public LayerMask cubeMask;

    private GameObject gameController;
    private Spawner spawner;

    private InputHandler inputHandler;

    private Cube touchingCube;
    private GameObject touchingAxis;

    private Vector3 cubeHitNormal;
    private Spawner.Axis cubeTouchAxis;
    private int cubeTouchIndex;

    void Start()
    {
        gameController = GameObject.FindGameObjectWithTag("GameController");
        spawner = gameController.GetComponentInChildren<Spawner>();

        inputHandler = gameController.GetComponent<InputHandler>();

        inputHandler.RegisterListener(this);
    }

    public void OnPinch(float amount)
    {
        Vector3 cameraPosition = Camera.main.transform.position + Vector3.forward * -(amount / 50f);
        cameraPosition.z = Mathf.Clamp(cameraPosition.z, -15, -10);
        Camera.main.transform.position = cameraPosition;
    }

    public void OnRotate(float amount)
    {
        // TODO: need to call this in InputHandler also!
    }

    public void OnTwoFingerDrag(Vector2 direction)
    {
        transform.Rotate(direction * 0.5f, Space.World);
    }

    public void OnOneFingerDrag(Vector2 direction)
    {

        // if we weren't already touching a cube, find the cube and 
        if (!touchingCube)
        {
            touchingCube = FindTouchingCube(out cubeHitNormal);

            if (touchingCube)
            {
                // assume for now that we're moving up/down on the screen

                if (cubeHitNormal == Vector3.back || cubeHitNormal == Vector3.forward)
                {
                    cubeTouchAxis = Spawner.Axis.X;
                    cubeTouchIndex = (int)touchingCube.Index.x;
                }
                else if (cubeHitNormal == Vector3.up || cubeHitNormal == Vector3.down)
                {
                    cubeTouchAxis = Spawner.Axis.X;
                    cubeTouchIndex = (int)touchingCube.Index.x;
                }
                else if (cubeHitNormal == Vector3.left || cubeHitNormal == Vector3.right)
                {
                    cubeTouchAxis = Spawner.Axis.Z;
                    cubeTouchIndex = (int)touchingCube.Index.z;
                }

                Cube[] childrenCubes;
                touchingAxis = spawner.MapCubesToAxis(cubeTouchAxis, cubeTouchIndex, out childrenCubes);

                foreach (Cube c in childrenCubes)
                {
                    c.renderer.material.color = Color.white;
                }
            }
        }

        if (touchingCube)
        {
            Vector3 worldDirection = touchingCube.transform.TransformDirection(direction);

            Vector3 rotation;
            switch (cubeTouchAxis)
            {
                case Spawner.Axis.X:
                    rotation = Vector3.right * worldDirection.x;
                    break;
                case Spawner.Axis.Y:
                    rotation = Vector3.down * worldDirection.y;
                    break;
                case Spawner.Axis.Z:
                default:
                    rotation = Vector3.back * worldDirection.z;
                    break;
            }

            touchingAxis.transform.Rotate(rotation * 0.5f);
        }

    }

    private Cube FindTouchingCube(out Vector3 hitNormal)
    {
        Vector2 touchPos = Input.touches[0].position;
        Ray screenRay = Camera.main.ScreenPointToRay(touchPos);
        
        RaycastHit hitInfo;
        Physics.Raycast(screenRay, out hitInfo, Mathf.Infinity, cubeMask);

        hitNormal = hitInfo.transform.InverseTransformDirection(hitInfo.normal);
        // Debug.Log("DRAG: " + hitInfo.transform.name + ", " + hitInfo.transform.InverseTransformDirection(hitInfo.normal));

        return hitInfo.transform.gameObject.GetComponent<Cube>();
    }

    public void OnTouchEnd()
    {
        touchingCube = null;

        Cube[] released;
        spawner.UnmapCubesFromAxis(cubeTouchAxis, cubeTouchIndex, out released);

        foreach (Cube c in released)
        {
            c.renderer.material.color = c.Colour;
        }
    }

}
