using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalController : MonoBehaviour
{
    [Header("Cấu hình Cổng Dịch Chuyển")]
    [Tooltip("ID của cổng này. Dùng để cổng khác tìm đến khi quay về.")]
    public string portalID;

    [Tooltip("ID của cổng ở scene đích mà bạn muốn đến.")]
    public string targetPortalID;

    [Tooltip("Tên của scene cần tải (phải được thêm vào Build Settings).")]
    public string sceneToLoad;

    private bool isPlayerInZone = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInZone = true;
            // Tùy chọn: Hiển thị UI gợi ý "Nhấn F để vào" ở đây.
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInZone = false;
            // Tùy chọn: Ẩn UI gợi ý đi.
        }
    }

    private void Start()
    {
        // Logic này giữ nguyên: di chuyển người chơi đến cổng này khi scene được tải
        // và ID của cổng này khớp với targetPortalID từ GameManager.
        if (GameManager.Instance != null && !string.IsNullOrEmpty(GameManager.Instance.targetPortalID) && portalID == GameManager.Instance.targetPortalID)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                // Tạm thời vô hiệu hóa CharacterController để dịch chuyển vị trí
                CharacterController cc = player.GetComponent<CharacterController>();
                if (cc != null) cc.enabled = false;

                player.transform.position = transform.position;

                if (cc != null) cc.enabled = true;
            }
            // Reset targetPortalID để không bị dịch chuyển lại khi tải lại game
            GameManager.Instance.targetPortalID = null;
        }
    }

    private void Update()
    {
        // Nếu người chơi ở trong vùng và nhấn phím F
        if (isPlayerInZone && Input.GetKeyDown(KeyCode.F))
        {
            Teleport();
        }
    }

    /// <summary>
    /// Thực hiện việc dịch chuyển.
    /// </summary>
    private void Teleport()
    {
        if (GameManager.Instance != null)
        {
            // Lưu ID của cổng đích vào GameManager để scene tiếp theo biết nơi xuất hiện
            GameManager.Instance.targetPortalID = targetPortalID;
            SceneManager.LoadScene(sceneToLoad);
        }
        else
        {
            Debug.LogError("Không tìm thấy GameManager! Quá trình dịch chuyển không thể tiếp tục.");
        }
    }
}