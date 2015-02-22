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
        direction /= 2f;

        if (!touchingCube)
        {
            // one finger dragging outside the cube should rotate it also
            transform.Rotate(direction, Space.World);
        }
        else if (!snapping)
        {
            if (!touchingAxis && touchingCube && touchingFace)
            {
                // DEBUGGING!
                touchingFace.DebugColourFaces();
                // END DEBUGGING!

                // find the touching axis
                float dragAngle = Vector3.Angle(Vector3.up, direction);
                Logger.SetValue("drag angle", dragAngle.ToString());
                DragDirection dragDirection = dragAngle < 45 || dragAngle > 135 ? DragDirection.Horizontal : DragDirection.Vertical;
                cubeTouchAxis = DetermineAxis(dragDirection, touchingFace.FaceDirection, out correctedDragDirection);

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

                // DEBUGGING!
                foreach (Cube c in childrenCubes)
                {
                    c.renderer.material.color = Color.white;
                }
                // END DEBUGGING!
            }

            if (touchingAxis)
            {
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

    private Vector3 CorrectAngles(Vector2 direction)
    {
        Vector3 result = Vector3.zero;
        result.x = direction.x * correctedDragDirection.x;
        result.y = direction.y * correctedDragDirection.y;
        return result;
    }

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

    public void OnTouchStart() { 
        touchingCube = FindTouchingCube(out cubeHitNormal);

        if (touchingCube)
        {
            touchingFace = FindTouchingFace();
        }
    }

    public void OnTouchEnd()
    {
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
        touchingAxis = null;

        Cube[] released;
        spawner.UnmapCubesFromAxis(cubeTouchAxis, cubeTouchIndex, out released);

        foreach (Cube c in released)
        {
            c.renderer.material.color = c.Colour;
        }
    }

    /// <summary>
    /// Determine the correct axis to rotate the selected cubes around.
    /// </summary>
    /// <param name="dragAngle"></param>
    /// <param name="cubeTouchIndex"></param>
    /// <returns></returns>
    public Spawner.Axis DetermineAxis(DragDirection dragDirection, Face.Direction faceDirection, out Vector2 directionCorrection)
    {
        switch (faceDirection)
        {
            case Face.Direction.Front:
            case Face.Direction.Back:
                directionCorrection = new Vector2(1, -1);
                if (dragDirection == DragDirection.Horizontal)
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
                if (dragDirection == DragDirection.Horizontal)
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
                if (dragDirection == DragDirection.Horizontal)
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
