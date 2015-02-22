using UnityEngine;
using System.Collections;

public class Rotator : MonoBehaviour, InputHandler.InputListener {

    public enum DragDirection
    {
        Horizontal, Vertical
    }

    private const float SNAP_TIME = 0.2f;

    public LayerMask cubeMask;
    public LayerMask faceMask;

    private GameObject gameController;
    private Spawner spawner;

    private InputHandler inputHandler;

    private GameObject board;

    private Cube touchingCube;
    private Face touchingFace;
    private GameObject touchingAxis;

    private Vector3 cubeHitNormal;
    private Spawner.Axis cubeTouchAxis;
    private int cubeTouchIndex;

    private Vector2 correctedDragDirection;

    private bool snapping = false;
    private float timeSnapping = 0f;
    private float startSnapAngle = 0f;
    private float snapToAngle = 0f;

    private Vector3 cachedAxisRotation;

    # region Start/Update

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

    # endregion

    # region Find touching functions

    private Cube FindTouchingCube(out Vector3 hitNormal)
    {
        Vector2 touchPos = Input.touches[0].position;
        Ray screenRay = Camera.main.ScreenPointToRay(touchPos);
        
        RaycastHit hitInfo;
        Physics.Raycast(screenRay, out hitInfo, Mathf.Infinity, cubeMask);

        if (hitInfo.transform)
        {
            hitNormal = hitInfo.transform.InverseTransformDirection(hitInfo.normal);
            Logger.SetValue("DRAG", hitInfo.transform.name + ", " + hitInfo.transform.InverseTransformDirection(hitInfo.normal));

            return hitInfo.transform.gameObject.GetComponent<Cube>();
        }

        hitNormal = Vector3.zero;
        return null;
    }

    private Face FindTouchingFace()
    {
        Vector2 touchPos = Input.touches[0].position;
        Ray screenRay = Camera.main.ScreenPointToRay(touchPos);

        RaycastHit hitInfo;
        Physics.Raycast(screenRay, out hitInfo, Mathf.Infinity, faceMask);

        if (hitInfo.transform)
        {
            return hitInfo.transform.gameObject.GetComponent<Face>();
        }

        return null;
    }

    # endregion

    private void FinishedSnap()
    {
        Vector3 fixedCached = FixedEulerAngles(cachedAxisRotation);
        Vector3 fixedNew = FixedEulerAngles(touchingAxis.transform.localEulerAngles);

        Logger.SetValue("OLD ROT", fixedCached.ToString());
        Logger.SetValue("NEW ROT", fixedNew.ToString());

        touchingAxis = null;

        Cube[] released;
        spawner.UnmapCubesFromAxis(cubeTouchAxis, cubeTouchIndex, out released);

        foreach (Cube c in released)
        {
            c.renderer.material.color = c.Colour;
        }
    }

    private Vector3 FixedEulerAngles(Vector3 eulerAngles)
    {
        if (cubeTouchAxis == Spawner.Axis.X && eulerAngles.y > 175f)
        {
            return new Vector3(eulerAngles.x + 180f,
                eulerAngles.y - 180f,
                eulerAngles.z - 180f);
        }

        return eulerAngles;
    }

    /// <summary>
    /// Determine the correct axis to rotate the selected cubes around.
    /// </summary>
    /// <param name="dragAngle"></param>
    /// <param name="cubeTouchIndex"></param>
    /// <returns></returns>
    public Spawner.Axis DetermineAxis(DragDirection dragDirection, Face.Direction faceDirection, out Vector2 directionCorrection)
    {
        bool switchAxis;

        switch (faceDirection)
        {
            case Face.Direction.Front:
            case Face.Direction.Back:
                switchAxis = ShouldSwitchAxisSelection(board.transform.rotation.eulerAngles.z);
                if (switchAxis)
                {
                    directionCorrection = new Vector2(-1, 1);
                }
                else
                {
                    directionCorrection = new Vector2(1, -1);
                }
                
                if (dragDirection == DragDirection.Horizontal != switchAxis)
                {
                    return Spawner.Axis.Y;
                }
                else
                {
                    return Spawner.Axis.X;
                }
            case Face.Direction.Left:
            case Face.Direction.Right:
                switchAxis = ShouldSwitchAxisSelection(board.transform.rotation.eulerAngles.x);
                directionCorrection = new Vector2(-1, -1);

                if (dragDirection == DragDirection.Horizontal != switchAxis)
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
                switchAxis = ShouldSwitchAxisSelection(board.transform.rotation.eulerAngles.y);
                if (switchAxis)
                {
                    directionCorrection = new Vector2(-1, 1);
                }
                else
                {
                    directionCorrection = new Vector2(1, -1);
                }

                if (dragDirection == DragDirection.Horizontal != switchAxis)
                {
                    return Spawner.Axis.Z;
                }
                else
                {
                    return Spawner.Axis.X;
                }
        }
    }

    private bool ShouldSwitchAxisSelection(float axisValue)
    {
        return (axisValue > 45 && axisValue < 135) || (axisValue > 225 && axisValue < 315);
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

    private Vector3 CorrectAngles(Vector2 direction)
    {
        Vector3 result = Vector3.zero;
        result.x = direction.x * correctedDragDirection.x;
        result.y = direction.y * correctedDragDirection.y;
        return result;
    }

    # region Input handling

    public void OnTouchStart()
    {

        // Find the touching cube and face

        touchingCube = FindTouchingCube(out cubeHitNormal);

        if (touchingCube)
        {
            touchingFace = FindTouchingFace();
        }
    }

    public void OnTouchEnd()
    {

        // Snap the touching axis and clear the touching cube and face

        if (touchingAxis)
        {
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

            touchingCube = null;
            touchingFace = null;
        }
    }

    public void OnRotate(float amount)
    {
        if (!touchingAxis)
        {
            // Rotate the board around global Z
            board.transform.Rotate(Vector3.forward, amount, Space.World);
            touchingFace = null;
            touchingCube = null;
        }
    }

    public void OnOneFingerDrag(Vector2 direction)
    {
        if (!touchingCube)
        {
            // Dragging outside the board spins it around
            transform.Rotate(direction / 2f, Space.World);
        }
        else if (!snapping)
        {
            if (!touchingAxis && touchingCube && touchingFace)
            {

                // DEBUGGING!
                touchingFace.DebugColourFaces();
                // END DEBUGGING!

                // Determine the touching axis
                float dragAngle = Vector3.Angle(Vector3.up, direction);
                Logger.SetValue("drag angle", dragAngle.ToString());
                DragDirection dragDirection = dragAngle < 45 || dragAngle > 135 ? DragDirection.Horizontal : DragDirection.Vertical;
                cubeTouchAxis = DetermineAxis(dragDirection, touchingFace.FaceDirection, out correctedDragDirection);

                // Now parent the cubes to the selected axis
                if (cubeTouchAxis == Spawner.Axis.X)
                {
                    cubeTouchIndex = (int)touchingCube.Index.x;
                }
                else if (cubeTouchAxis == Spawner.Axis.Y)
                {
                    cubeTouchIndex = (int)touchingCube.Index.y;
                }
                else if (cubeTouchAxis == Spawner.Axis.Z)
                {
                    cubeTouchIndex = (int)touchingCube.Index.z;
                }

                Cube[] childrenCubes;
                touchingAxis = spawner.MapCubesToAxis(cubeTouchAxis, cubeTouchIndex, out childrenCubes);

                cachedAxisRotation = touchingAxis.transform.localEulerAngles;

                // DEBUGGING!
                foreach (Cube c in childrenCubes)
                {
                    c.renderer.material.color = Color.white;
                }
                // END DEBUGGING!
            }

            if (touchingAxis)
            {
                // If we are touching an axis, rotate the axis according to the touch direction

                Vector3 worldDirection = board.transform.InverseTransformDirection(CorrectAngles(direction));

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

    # endregion
}
