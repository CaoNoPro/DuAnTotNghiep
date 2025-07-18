using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    public Animator animator;
    public PlayerVirtual playerVirtual;
    [Header("Movement System")]
    public float walkSpeed = 5f;
    public float runSpeed = 10f;
    public float jumpForce = 8f;
    public float gravity = -9.8f * 2;

    private CharacterController controller;
    private Vector3 velocity;
    private bool isSprinting;
    private float currentSpeed;
    [Header("Look System")]
    public float mouseSensitivity = 100f;
    public Transform playerBody;
    float xRotation = 0f;
    float YRotation = 0f;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        currentSpeed = walkSpeed;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        //Mouse Sensitivity
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        YRotation += mouseX;
        transform.localRotation = Quaternion.Euler(xRotation, YRotation, 0f);

        //movement
        float currentHorizontalSpeed = new Vector3(controller.velocity.x, 0, controller.velocity.z).magnitude;

        if (animator != null)
        {
            animator.SetFloat("Speed", currentHorizontalSpeed);
            animator.SetBool("isSprinting", isSprinting);
        }

        playerBody.Rotate(Vector3.up * mouseX);


        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        if (Input.GetKey(KeyCode.LeftShift) && (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0))
        {
            isSprinting = true;
            currentSpeed = runSpeed;
        }
        else
        {
            isSprinting = false;
            currentSpeed = walkSpeed;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        controller.Move(move * currentSpeed * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && controller.isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

    }
}