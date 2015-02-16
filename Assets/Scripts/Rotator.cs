using UnityEngine;
using System.Collections;

public class Rotator : MonoBehaviour, InputHandler.InputListener {

    private const float SNAP_TIME = 0.2f;

    public LayerMask cubeMask;

    private GameObject gameController;
    private Spawner spawner;

    private InputHandler inputHandler;

    private GameObject board;

    private Cube touchingCube;
    private GameObject touchingAxis;

    private Vector3 cubeHitNormal;
    private Spawner.Axis cubeTouchAxis;
    private int cubeTouchIndex;

    private bool snapping = false;
    private float timeSnapping = 0f;
    private float startSnapAngle = 0f;
    private float snapToAngle = 0f;

    void Start()
    {
        gameController = GameObject.FindGameObjectWithTag("GameController");
        spawner = gameController.GetComponentInChildren<Spawner>();

        board = GameObject.FindGameObjectWithTag("Board");

        inputHandler = gameController.GetComponent<InputHandler>();

        inputHandler.RegisterListener(this);
    }

    void Update()
    {
        if (snapping)
        {
            Logger.SetValue("snapAngle", snapToAngle.ToString());

            Vector3 target;

            timeSnapping += Time.deltaTime;

            switch (cubeTouchAxis)
            {
                case Spawner.Axis.X:
                    target = new Vector3(
                        Mathf.Lerp(startSnapAngle, snapToAngle, timeSnapping / SNAP_TIME),
                        touchingAxis.transform.localEulerAngles.y,
                        touchingAxis.transform.localEulerAngles.z);
                    break;
                case Spawner.Axis.Y:
                    target = new Vector3(
                        touchingAxis.transform.localEulerAngles.x,
                        Mathf.Lerp(startSnapAngle, snapToAngle, timeSnapping / SNAP_TIME),
                        touchingAxis.transform.localEulerAngles.z);
                    break;
                case Spawner.Axis.Z:
                default:
                    target = new Vector3(
                        touchingAxis.transform.localEulerAngles.x,
                        touchingAxis.transform.localEulerAngles.y,
                        Mathf.Lerp(startSnapAngle, snapToAngle, timeSnapping / SNAP_TIME));
                    break;
            }

            touchingAxis.transform.localEulerAngles = target;

            if (timeSnapping >= SNAP_TIME)
            {
                snapping = false;

                timeSnapping = 0f;
                snapToAngle = 0f;
                startSnapAngle = 0f;
                FinishedSnap();
            }
        }
    }

    //public void OnPinch(float amount)
    //{
    //    Vector3 cameraPosition = Camera.main.transform.position + Vector3.forward * -(amount / 50f);
    //    cameraPosition.z = Mathf.Clamp(cameraPosition.z, -15, -10);
    //    Camera.main.transform.position = cameraPosition;
    //}

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
        if (!snapping)
        {
            // if we weren't already touching a cube, find the touching cube and set the correct axis

            if (!touchingCube)
            {
                touchingCube = FindTouchingCube(out cubeHitNormal);

                if (touchingCube)
                {
                    // need to set value to cubeTouchAxis and cubeTouchIndex


                    Cube[] childrenCubes;
                    touchingAxis = spawner.MapCubesToAxis(cubeTouchAxis, cubeTouchIndex, out childrenCubes);

                    foreach (Cube c in childrenCubes)
                    {
                        c.renderer.material.color = Color.white;
                    }

                }
            }

            if (touchingAxis)
            {
                Vector3 worldDirection = board.transform.TransformDirection(direction);

                Logger.SetValue("worldDirection", worldDirection.ToString());

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
    }

    private Cube FindTouchingCube(out Vector3 hitNormal)
    {
        Vector2 touchPos = Input.touches[0].position;
        Ray screenRay = Camera.main.ScreenPointToRay(touchPos);
        
        RaycastHit hitInfo;
        Physics.Raycast(screenRay, out hitInfo, Mathf.Infinity, cubeMask);

        hitNormal = hitInfo.transform.InverseTransformDirection(hitInfo.normal);
        Logger.SetValue("DRAG", hitInfo.transform.name + ", " + hitInfo.transform.InverseTransformDirection(hitInfo.normal));

        return hitInfo.transform.gameObject.GetComponent<Cube>();
    }

    public void OnTouchEnd()
    {
        touchingCube = null;
        snapping = true;

        switch (cubeTouchAxis)
        {
            case Spawner.Axis.X:
                startSnapAngle = touchingAxis.transform.localEulerAngles.x;
                snapToAngle = CalculateTargetAngle(touchingAxis.transform.localEulerAngles.x);
                break;
            case Spawner.Axis.Y:
                startSnapAngle = touchingAxis.transform.localEulerAngles.y;
                snapToAngle = CalculateTargetAngle(touchingAxis.transform.localEulerAngles.y);
                break;
            case Spawner.Axis.Z:
            default:
                startSnapAngle = touchingAxis.transform.localEulerAngles.z;
                snapToAngle = CalculateTargetAngle(touchingAxis.transform.localEulerAngles.z);
                break;
        }
    }

    private float CalculateTargetAngle(float currentAngle)
    {
        float diff = currentAngle % 90;
        float target = ((int)(currentAngle / 90)) * 90f;
        if (diff > 45)
        {
            target = (target + 90f);
        }
        return target;
    }

    private void FinishedSnap()
    {
        Cube[] released;
        spawner.UnmapCubesFromAxis(cubeTouchAxis, cubeTouchIndex, out released);

        foreach (Cube c in released)
        {
            c.renderer.material.color = c.Colour;
        }
    }

}
