using UnityEngine;

public class PlayerDirectedMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 6.0f; // Tốc độ di chuyển khi được người chơi chỉ định
    public Transform playerReference; // Kéo thả Player GameObject vào đây
    public Camera playerCamera; // Kéo thả Player Camera vào đây

    private Rigidbody rb; // Nên dùng Rigidbody cho di chuyển vật lý
    private Vector3 moveDirection;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null) Debug.LogError("Rigidbody not found on " + gameObject.name + ". PlayerDirectedMovement requires a Rigidbody.");
    }

    void OnEnable()
    {
        // Đảm bảo AI không di chuyển khi script này được bật lần đầu
        moveDirection = Vector3.zero;
        if (rb != null)
        {
            rb.isKinematic = false; // Bật lại vật lý để di chuyển bằng Rigidbody.velocity
        }
    }

    void OnDisable()
    {
        // Khi script này bị tắt, dừng di chuyển
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.isKinematic = true; // Vô hiệu hóa vật lý để NavMeshAgent có thể hoạt động lại
        }
    }

    void Update()
    {
        // Lấy input từ người chơi (ví dụ: W, A, S, D)
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // Tính toán hướng di chuyển dựa trên hướng nhìn của camera người chơi
        Vector3 cameraForward = playerCamera.transform.forward;
        Vector3 cameraRight = playerCamera.transform.right;

        cameraForward.y = 0f; // Bỏ qua trục Y để chỉ di chuyển trên mặt phẳng XZ
        cameraRight.y = 0f;
        cameraForward.Normalize();
        cameraRight.Normalize();

        moveDirection = (cameraForward * verticalInput + cameraRight * horizontalInput).normalized;
    }

    void FixedUpdate()
    {
        // Áp dụng vận tốc cho Rigidbody để di chuyển
        if (rb != null)
        {
            rb.linearVelocity = moveDirection * moveSpeed;
        }
    }

    // Hàm public để người chơi gọi chuyển trạng thái AI sang PlayerDirected
    // Ví dụ: khi người chơi nhấn nút 'G' để điều khiển AI
    public void RequestPlayerControl()
    {
        if (playerReference != null)
        {
            PlayerAIController aiController = playerReference.GetComponent<PlayerAIController>();
            if (aiController != null)
            {
                aiController.SetAIState(PlayerAIController.AIState.PlayerDirected);
            }
        }
    }
}