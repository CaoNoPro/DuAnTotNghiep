using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    [Header("Input Actions Asset")]
    [SerializeField] private InputActionAsset playerControls; 

    [Header("Map Input Actions")]
    [SerializeField] private string actionMapName = "Player";

    [Header("Action Name Reference")]
    [SerializeField] private string move= "Move";
    [SerializeField] private string jump = "Jump";
    [SerializeField] private string Look = "Look";
    [SerializeField] private string Sprint = "Sprint";

    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction lookAction;
    private InputAction sprintAction;

    public float SprintInput { get; private set; }
    public Vector2 MoveInput { get; private set; }
    public bool JumpInput { get; private set; }
    public Vector2 LookInput { get; private set; } 

    public static PlayerInputHandler Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject); // Optional, but common for singletons
        }

        moveAction = playerControls.FindActionMap(actionMapName).FindAction(move);
        jumpAction = playerControls.FindActionMap(actionMapName).FindAction(jump);
        lookAction = playerControls.FindActionMap(actionMapName).FindAction(Look);
        sprintAction = playerControls.FindActionMap(actionMapName).FindAction(Sprint);
        RegisterInputActions();
    }

    void RegisterInputActions()
    {
        moveAction.performed += context => MoveInput = context.ReadValue<Vector2>();
        moveAction.canceled += context => MoveInput = Vector2.zero;

        lookAction.performed += context => LookInput = context.ReadValue<Vector2>();
        lookAction.canceled += context => LookInput = Vector2.zero;

        jumpAction.performed += context => JumpInput = true;
        jumpAction.canceled += context => JumpInput = false;

        sprintAction.performed += context => SprintInput = context.ReadValue<float>();
        sprintAction.canceled += context => SprintInput = 0f;
    }

    private void OnEnable()
    {
        moveAction.Enable();
        jumpAction.Enable();
        lookAction.Enable();
        sprintAction.Enable();
    }
    private void OnDisable()
    {
        moveAction.Disable();
        jumpAction.Disable();
        lookAction.Disable();
        sprintAction.Disable();
    }
}
