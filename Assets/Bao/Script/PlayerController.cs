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
    public float gravity = -9.8f;

    private CharacterController controller;
    private Vector3 velocity;
    private bool isSprinting;
    private float currentSpeed;
    [Header("Look System")]
    public float mouseSensitivity = 100f;
    public Transform playerBody;

    private float xRotation = 0f;
    [Header("Shooting System")]
    public Camera fpsCam;
    public float damage = 10f;
    public float range = 100f;
    public float fireRate = 0.5f;

    private float nextTimeToFire = 0f;

    [Header("Weapon & Aiming")]
    public GameObject currentWeaponModel;
    public float normalFOV = 60f;
    public float aimFOV = 40f;
    public float aimSpeed = 8f;
    public float normalSensitivityMultiplier = 1f;
    public float aimSensitivityMultiplier = 0.5f;
    private Vector3 initialWeaponPosition;

    [Header("UI & Game State")]
    public GameObject pauseMenuUI;
    public static bool GameIsPaused = false;


    private void Start()
    {
        controller = GetComponent<CharacterController>();
        currentSpeed = walkSpeed;

        Cursor.lockState = CursorLockMode.Locked;
        if (currentWeaponModel != null)
        {
            initialWeaponPosition = currentWeaponModel.transform.localPosition;
        }

        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(false);
        }
    }

    private void Update()
    {


        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        float currentHorizontalSpeed = new Vector3(controller.velocity.x, 0, controller.velocity.z).magnitude;

        if (animator != null)
        {
            animator.SetFloat("Speed", currentHorizontalSpeed);
            animator.SetBool("isSprinting", isSprinting);
        }

        if (fpsCam != null)
        {
            fpsCam.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        }

        playerBody.Rotate(Vector3.up * mouseX);


        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        if (Input.GetKey(KeyCode.LeftShift) && (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0) && playerVirtual.StaminaSlider.value > 0)
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



        if (Input.GetButton("Fire1") && Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + fireRate;
            Shoot();
        }

    }


    void Shoot()
    {
        RaycastHit hit;

        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range))
        {
            Debug.Log("Đã bắn trúng: " + hit.transform.name);
        }
    }
}