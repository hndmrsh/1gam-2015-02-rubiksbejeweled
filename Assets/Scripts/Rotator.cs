using UnityEngine;
using System.Collections;

public class Rotator : MonoBehaviour, InputHandler.InputListener {

    private GameObject gameController;

    private InputHandler inputHandler;

    private 

    void Start()
    {
        gameController = GameObject.FindGameObjectWithTag("GameController");

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
        
    }

    public void OnTouchEnd()
    {

    }
}
