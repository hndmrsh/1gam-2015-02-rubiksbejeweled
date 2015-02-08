using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InputHandler : MonoBehaviour {

    public enum TwoFingerTouchMode
    {
        None, Pinch, Drag
    }

    private TwoFingerTouchMode twoFingerTouchMode;

    public interface InputListener
    {
        void OnPinch(float amount);
        void OnTwoFingerDrag(Vector2 direction);
        void OnOneFingerDrag(Vector2 direction);
    }

    private List<InputListener> listeners;

    void Start()
    {
        twoFingerTouchMode = TwoFingerTouchMode.None;
    }

	// Update is called once per frame
	void Update () {
        Debug.Log("touches = " + Input.touches.Length);

        if (listeners != null && listeners.Count > 0)
        {
            if (Input.touches.Length == 2 && (Input.touches[0].phase == TouchPhase.Moved || Input.touches[1].phase == TouchPhase.Moved))
            {
                // two fingers touching

                // if we're not in the middle of a two-finger movement, work out which one we're in at the moment
                if (twoFingerTouchMode == TwoFingerTouchMode.None)
                {
                    float angleBetweenTouches = Vector3.Angle(Input.touches[0].deltaPosition, Input.touches[1].deltaPosition);

                    if (angleBetweenTouches < 90f)
                    {
                        twoFingerTouchMode = TwoFingerTouchMode.Drag;
                    }
                    else
                    {
                        twoFingerTouchMode = TwoFingerTouchMode.Pinch;
                    }
                }

                if (twoFingerTouchMode == TwoFingerTouchMode.Drag)
                {
                    Vector2 average = (Input.touches[0].deltaPosition + Input.touches[1].deltaPosition) / 2;
                    foreach (InputListener l in listeners)
                    {
                        l.OnTwoFingerDrag(average);
                    }
                }
                else
                {
                    // Source: http://unity3d.com/learn/tutorials/modules/beginner/platform-specific/pinch-zoom

                    // Find the position in the previous frame of each touch.
                    Vector2 prevPosA = Input.touches[0].position - Input.touches[0].deltaPosition;
                    Vector2 prevPosB = Input.touches[1].position - Input.touches[1].deltaPosition;

                    // Find the magnitude of the vector (the distance) between the touches in each frame.
                    float prevTouchDeltaMag = (prevPosA - prevPosB).magnitude;
                    float touchDeltaMag = (Input.touches[0].position - Input.touches[1].position).magnitude;

                    // Find the difference in the distances between each frame.
                    float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

                    foreach (InputListener l in listeners)
                    {
                        l.OnPinch(deltaMagnitudeDiff);
                    }
                }

            }
            else if (Input.touches.Length == 1 && Input.touches[0].phase == TouchPhase.Moved)
            {
                // one finger drag
                Touch singleTouch = Input.touches[0];
                foreach (InputListener l in listeners)
                {
                    // if we started with two fingers, continue dragging with two fingers even though we've released one (though not for pinching)

                    if (twoFingerTouchMode == TwoFingerTouchMode.None)
                    {
                        l.OnOneFingerDrag(singleTouch.deltaPosition);
                    }
                    else if(twoFingerTouchMode == TwoFingerTouchMode.Drag)
                    {
                        l.OnTwoFingerDrag(singleTouch.deltaPosition);
                    }
                }
            }
            else if (Input.touches.Length == 0)
            {
                // clear two-finger touch mode
                twoFingerTouchMode = TwoFingerTouchMode.None;
            }
        }
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

}
