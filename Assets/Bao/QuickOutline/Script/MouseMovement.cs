using UnityEngine;

public class MouseMovement : MonoBehaviour
{
    public float mouseSentivity = 500f;

    float xRotation = 0f;
    float yRotation = 0f;

    public float topClamp = -90f;
    public float bottomClamp = 90f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        //getting mouse
        float mouseX = Input.GetAxis("Mouse X") * mouseSentivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSentivity * Time.deltaTime;

        //rotating the camera
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, topClamp, bottomClamp); //clamping the rotation to prevent flipping
        yRotation += mouseX;
        transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);
    }
}
