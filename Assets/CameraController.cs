using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    private const float MOUSE_SPEED = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        // Randomly place the camera to create different views
        var cameraPosition = Random.insideUnitCircle * 5f;
        transform.position = new Vector3(cameraPosition.x, 15f, cameraPosition.y);
        transform.LookAt(Vector3.zero);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Look(InputAction.CallbackContext value)
    {
        Vector2 input = value.ReadValue<Vector2>() * MOUSE_SPEED;
        transform.eulerAngles += new Vector3(-input.y, input.x, 0);
    }
}
