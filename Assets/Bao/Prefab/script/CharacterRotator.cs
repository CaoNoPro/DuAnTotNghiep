using UnityEngine;

public class CharacterController3D : MonoBehaviour
{
    [Header("Rotation Settings")]
    public float rotationSpeed = 200f;
    public float mouseSensitivity = 2f;
    
    [Header("References")]
    public Transform characterBody;
    public Camera playerCamera;
    
    private float xRotation = 0f;
    private bool cursorLocked = true;

    void Start()
    {
        LockCursor(true);
    }

    void Update()
    {
        // Xử lý xoay nhân vật bằng chuột
        HandleMouseLook();
        
        // Xử lý xoay nhân vật bằng phím
        HandleKeyboardRotation();
        
        // Toggle ẩn/hiện con trỏ chuột bằng phím Esc
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            cursorLocked = !cursorLocked;
            LockCursor(cursorLocked);
        }
    }

    void HandleMouseLook()
    {
        if (!cursorLocked) return;
        
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Xoay nhân vật trái/phải (quanh trục Y)
        characterBody.Rotate(Vector3.up * mouseX);
        
        // Xoay camera lên/xuống (quanh trục X)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f); // Giới hạn góc nhìn
        
        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }

    void HandleKeyboardRotation()
    {
        float rotationInput = 0f;
        
        if (Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.LeftArrow))
            rotationInput = -1f;
        else if (Input.GetKey(KeyCode.E) || Input.GetKey(KeyCode.RightArrow))
            rotationInput = 1f;
            
        characterBody.Rotate(Vector3.up * rotationInput * rotationSpeed * Time.deltaTime);
    }

    void LockCursor(bool isLocked)
    {
        if (isLocked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
