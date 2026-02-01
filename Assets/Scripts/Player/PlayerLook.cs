using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLook : MonoBehaviour
{
    [Header("Input")]
    public InputActionReference lookAction; // Vector2

    [Header("References")]
    public Transform cameraRoot;          
    public Transform playerBody;

    [Header("Settings")]
    public float sensitivity = 1f;
    public float verticalClamp = 80f;

    private float pitch = 0f;
    
    private Vector2 lookInput;
    
    private void Start()
    {
        // Lock cursor to center
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    private void OnEnable()
    {
        lookAction.action.Enable();
    }

    private void OnDisable()
    {
        lookAction.action.Disable();
    }
    
    void Update()
    {
        //body rotation (yaw)
        lookInput = lookAction.action.ReadValue<Vector2>();
        float yaw = lookInput.x * sensitivity;
        playerBody.Rotate(0f, yaw, 0f);
        
        //camera rotation (pitch)
        pitch -= lookInput.y * sensitivity;
        pitch = Mathf.Clamp(pitch, -verticalClamp, verticalClamp);
        cameraRoot.localRotation = Quaternion.Euler(pitch, 0, 0);
    }
    
    void OnGUI()
    {
        float size = 40f; // size of the arrow
        float x = Screen.width / 2;
        float y = Screen.height / 2;
        
        GUI.Label(new Rect(x - size/2, y - size/2, size, size), "+"); 
    }
}
