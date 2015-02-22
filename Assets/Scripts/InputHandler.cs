using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InputHandler : MonoBehaviour {

    public float dragDeadzone = 1.5f;

    private bool touchDown = false;

    public interface InputListener
    {
        void OnRotate(float amount);
        void OnOneFingerDrag(Vector2 direction);
        void OnTouchStart();
        void OnTouchEnd();
    }

    private List<InputListener> listeners;

	// Update is called once per frame
	void Update () {
        if (Application.isMobilePlatform && Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        if (listeners != null && listeners.Count > 0)
        {
            bool oldTouchDown = touchDown;

            if (Input.touches.Length == 2 && (Input.touches[0].phase == TouchPhase.Moved || Input.touches[1].phase == TouchPhase.Moved))
            {
                touchDown = true;
                // two fingers touching

                Touch touch1 = Input.touches[0];
                Touch touch2 = Input.touches[1];

                float turnAngle = Angle(touch1.position, touch2.position);
                float prevTurn = Angle(touch1.position - touch1.deltaPosition, touch2.position - touch2.deltaPosition);
                float turnAngleDelta = Mathf.DeltaAngle(prevTurn, turnAngle);

                foreach (InputListener l in listeners)
                {
                    l.OnRotate(turnAngleDelta);
                }
            }
            else if (Input.touches.Length == 1 && Input.touches[0].phase == TouchPhase.Moved)
            {
                touchDown = true;
                // one finger drag
                Touch singleTouch = Input.touches[0];
                
                if (singleTouch.deltaPosition.magnitude >= dragDeadzone)
                {
                    foreach (InputListener l in listeners)
                    {
                        l.OnOneFingerDrag(CorrectAngles(singleTouch.deltaPosition));
                    }
                }
            }
            else if (Input.touches.Length == 0)
            {
                touchDown = false;
            }

            // call OnTouchStart or End where appropriate

            if (oldTouchDown && !touchDown)
            {
                foreach (InputListener l in listeners)
                {
                    l.OnTouchEnd();
                }
            }
            else if (!oldTouchDown && touchDown)
            {
                foreach (InputListener l in listeners)
                {
                    l.OnTouchStart();
                }
            }
        }
	}

    private Vector2 CorrectAngles(Vector2 direction)
    {
        return new Vector2(direction.y, -direction.x);
    }

    public void RegisterListener(InputListener listener)
    {
        if (listeners == null)
        {
            listeners = new List<InputListener>();
        }

        listeners.Add(listener);
    }

    public bool UnregisterListener(InputListener listener)
    {
        if (listeners != null)
        {
            return listeners.Remove(listener);
        }

        return false;
    }

    private float Angle(Vector2 pos1, Vector2 pos2)
    {
        Vector2 from = pos2 - pos1;
        Vector2 to = new Vector2(1, 0);

        float result = Vector2.Angle(from, to);
        Vector3 cross = Vector3.Cross(from, to);

        if (cross.z > 0)
        {
            result = 360f - result;
        }

        return result;
    }

}
