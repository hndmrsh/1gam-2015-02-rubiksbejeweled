using UnityEngine;
using System.Collections;

public class Rotator : MonoBehaviour, InputHandler.InputListener {

    private GameObject gameController;

    private InputHandler inputHandler;


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

    public void OnTwoFingerDrag(Vector2 direction)
    {
        transform.Rotate(new Vector3(direction.y, -direction.x), Space.World);
    }

    public void OnOneFingerDrag(Vector2 direction)
    {
        
    }
}
