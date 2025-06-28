using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 3.0f;
    [SerializeField] private float jumpForce = 2.0f;

    [Header("Jump Parameters")]
    [SerializeField] private float jumpHeight = 5.0f;
    [SerializeField] private float gravity = 9.81f;

    [Header("Look Sensitivity")]
    [SerializeField] private float lookSensitivity = 2f;
    [SerializeField] private float maxLookAngle = 80.0f;

    private CharacterController characterController;
    private Camera mainCamera;
    private PlayerInputHandler inputHandler;
    private Vector3 currentMovent;
    private float verticalRotation;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        mainCamera = Camera.main;
        inputHandler = PlayerInputHandler.Instance;

        if (inputHandler == null)
    {
        Debug.LogError("FPSController: PlayerInputHandler.Instance is NULL! Make sure PlayerInputHandler exists in the scene and its Awake() runs first.", this);
    }
    }

    private void Update()
    {
        HandleMovement();
        HandleLook();
    }
    void HandleMovement()
    {
        float speed = moveSpeed * (inputHandler.SprintInput > 0 ? 2.0f : 1.0f); // Double speed if sprinting

        Vector3 inputDirection = new Vector3(inputHandler.MoveInput.x, 0, inputHandler.MoveInput.y);
        Vector3 worldDirection = transform.TransformDirection(inputDirection);
        worldDirection.Normalize(); // Ignore vertical movement for horizontal speed

        currentMovent.x = worldDirection.x * speed;
        currentMovent.y = worldDirection.z * speed;

        HandleJumping();
        characterController.Move(currentMovent * Time.deltaTime);
    }

    void HandleJumping()
    {
       if(characterController.isGrounded)
        {
            currentMovent.y = -0.5f; // Small downward force to keep grounded
            
            if (inputHandler.JumpInput)
            {
                currentMovent.y = jumpForce;
            }
        }
        else
        {
            currentMovent.y -= gravity * Time.deltaTime; // Apply gravity
        }
    }
    void HandleLook()
    {
        float mouseX = inputHandler.LookInput.x * lookSensitivity;
        transform.Rotate(0, mouseX, 0);

        verticalRotation -= inputHandler.LookInput.y * lookSensitivity;
        verticalRotation = Mathf.Clamp(verticalRotation, -maxLookAngle, maxLookAngle);
        mainCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
    }
}
