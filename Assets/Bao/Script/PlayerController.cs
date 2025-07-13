using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    public Animator animator;
    public PlayerVirtual playerVirtual; // Kéo PlayerVirtual vào đây trong Inspector     
    [Header("Movement System")]
    public float walkSpeed = 5f;
    public float runSpeed = 10f;
    public float jumpForce = 8f;
    public float gravity = -9.8f;

    private CharacterController controller;
    private Vector3 velocity;
    private bool isSprinting;
    private float currentSpeed;

    // --- Start: Thêm vào cho Rotation (Xoay góc nhìn) ---
    [Header("Look System")]
    public float mouseSensitivity = 100f; // Độ nhạy chuột
    public Transform playerBody; // Kéo GameObject "Player" vào đây trong Inspector

    private float xRotation = 0f; // Lưu trữ góc xoay dọc của camera

    // --- End: Thêm vào cho Rotation ---


    // --- Start: Thêm vào cho Shooting (Bắn) ---
    [Header("Shooting System")]
    public Camera fpsCam; // Kéo Main Camera vào đây trong Inspector
    public float damage = 10f; // Sát thương mỗi phát bắn
    public float range = 100f; // Tầm bắn
    public float fireRate = 0.5f; // Thời gian giữa các lần bắn (ví dụ: 0.5s = 2 viên/giây)

    private float nextTimeToFire = 0f; // Thời gian có thể bắn lần tiếp theo
    // --- End: Thêm vào cho Shooting ---

    [Header("Weapon & Aiming")]
    public GameObject currentWeaponModel; // Kéo GameObject "Gun" vào đây
    public float normalFOV = 60f; // FOV (Field of View) bình thường của camera
    public float aimFOV = 40f;    // FOV khi ngắm bắn
    public float aimSpeed = 8f;   // Tốc độ chuyển đổi FOV khi ngắm
    public float normalSensitivityMultiplier = 1f; // Hệ số nhân độ nhạy chuột khi bình thường
    public float aimSensitivityMultiplier = 0.5f; // Hệ số nhân độ nhạy chuột khi ngắm

    private Vector3 initialWeaponPosition; // Vị trí ban đầu của súng

    // --- Start: Thêm vào cho Menu và Tạm dừng game ---
    [Header("UI & Game State")]
    public GameObject pauseMenuUI; // Kéo GameObject chứa UI menu tạm dừng vào đây trong Inspector
    public static bool GameIsPaused = false; // Biến tĩnh để kiểm tra trạng thái tạm dừng
    // --- End: Thêm vào cho Menu và Tạm dừng game ---


    private void Start()
    {
        controller = GetComponent<CharacterController>();
        currentSpeed = walkSpeed;

        // Khóa con trỏ chuột vào giữa màn hình và ẩn đi khi bắt đầu game
        Cursor.lockState = CursorLockMode.Locked;
        if (currentWeaponModel != null)
        {
            initialWeaponPosition = currentWeaponModel.transform.localPosition;
        }

        // Đảm bảo menu tạm dừng bị tắt khi bắt đầu game
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(false);
        }
    }

    private void Update()
    {
        // --- Start: Logic Menu và Tạm dừng game ---
        if (Input.GetKeyDown(KeyCode.F1))
        {
            if (GameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }

        if (GameIsPaused)
        {
            return; // Ngừng xử lý các input khác khi game đang tạm dừng
        }
        // --- End: Logic Menu và Tạm dừng game ---


        // --- Start: Logic Xoay góc nhìn (Rotation) ---
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); // Giới hạn góc nhìn lên/xuống

        //Tính toán tốc độ ngang thực tế của người chơi
        float currentHorizontalSpeed = new Vector3(controller.velocity.x, 0, controller.velocity.z).magnitude;

        //cập nhật tham số animator
        if (animator != null)
        {
            animator.SetFloat("Speed", currentHorizontalSpeed);
            animator.SetBool("isSprinting", isSprinting);
        }

        if (fpsCam != null) // Đảm bảo đã gán camera trong Inspector
        {
            fpsCam.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        }

        // Xoay ngang toàn bộ Player (sử dụng playerBody)
        playerBody.Rotate(Vector3.up * mouseX);
        // --- End: Logic Xoay góc nhìn (Rotation) ---


        // trọng lực
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // Chạy
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

        // di chuyển ngang
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        controller.Move(move * currentSpeed * Time.deltaTime);

        // xử lý nhảy
        if (Input.GetButtonDown("Jump") && controller.isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
        }

        // áp dụng trọng lực
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);


        // --- Start: Logic Bắn (Shooting) ---
        // Nếu nhấn nút bắn (chuột trái) và đủ thời gian hồi chiêu
        if (Input.GetButton("Fire1") && Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + fireRate; // Cập nhật thời gian cho lần bắn tiếp theo
            Shoot(); // Gọi hàm bắn
        }
        // --- End: Logic Bắn (Shooting) ---
    }

    // --- Start: Hàm Shoot() cho Shooting ---
    void Shoot()
    {
        // (Tùy chọn) Phát âm thanh bắn, hiển thị hiệu ứng nòng súng
        // Ví dụ: AudioManager.PlaySound("Gunshot");
        // MuzzleFlash.Play(); // Nếu bạn có một Particle System cho hiệu ứng

        RaycastHit hit; // Biến lưu trữ thông tin về vật thể bị va chạm

        // Bắn một tia (ray) từ vị trí camera về phía trước với tầm bắn đã định
        // `Physics.Raycast(điểm xuất phát, hướng, biến đầu ra thông tin va chạm, tầm bắn)`
        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range))
        {
            Debug.Log("Đã bắn trúng: " + hit.transform.name);

            // --- Áp dụng sát thương (nếu có hệ thống máu kẻ địch) ---
            // Bạn có thể thêm code để gây sát thương cho kẻ địch ở đây
            // Ví dụ:
            // EnemyHealth enemyHealth = hit.transform.GetComponent<EnemyHealth>();
            // if (enemyHealth != null)
            // {
            //     enemyHealth.TakeDamage((int)damage);
            // }

            // --- Hiển thị hiệu ứng va chạm (vết đạn, tia lửa) ---
            // Ví dụ: Instantiate(impactEffectPrefab, hit.point, Quaternion.LookRotation(hit.normal));
        }
    }
    // --- End: Hàm Shoot() cho Shooting ---

    // --- Start: Thêm vào hàm Resume() và Pause() ---
    public void Resume()
    {
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(false); // Tắt UI menu
        }
        Time.timeScale = 1f; // Đặt lại tốc độ thời gian về bình thường
        Cursor.lockState = CursorLockMode.Locked; // Khóa con trỏ chuột
        Cursor.visible = false; // Ẩn con trỏ chuột
        GameIsPaused = false; // Cập nhật trạng thái game

    }

    void Pause()
    {
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(true); // Bật UI menu
        }
        Time.timeScale = 0f; // Dừng thời gian trong game
        Cursor.lockState = CursorLockMode.None; // Mở khóa con trỏ chuột
        Cursor.visible = true; // Hiển thị con trỏ chuột
        GameIsPaused = true; // Cập nhật trạng thái game
    }
    // --- End: Thêm vào hàm Resume() và Pause() ---
}