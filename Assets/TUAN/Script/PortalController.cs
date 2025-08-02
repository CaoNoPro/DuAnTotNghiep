using UnityEngine;
using UnityEngine.SceneManagement;
// Đảm bảo bạn có lớp SaveManager và GameManager trong dự án của mình
// Nếu GameManager.Instance chưa được định nghĩa hoặc targetPortalID không tồn tại,
// bạn cần tạo chúng hoặc điều chỉnh tên biến cho phù hợp.

public class PortalController : MonoBehaviour
{
    [Tooltip("ID độc nhất của cổng này, ví dụ: 'FromMainToMiniMap1'")]
    public string portalID;

    [Tooltip("ID của cổng ở scene đích mà người chơi sẽ xuất hiện")]
    public string targetPortalID;

    [Tooltip("Tên của scene sẽ được tải")]
    public string sceneToLoad;

    [Tooltip("Số slot sẽ được dùng để tự động lưu game khi đi qua cổng.")]
    [SerializeField] private int saveSlotNumber = 1; // Mặc định lưu vào slot 1

    private bool isPlayerInZone = false;

    // ... (Giữ nguyên OnTriggerEnter và OnTriggerExit) ...
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInZone = true;
            Debug.Log($"Player entered portal zone: {portalID}");
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInZone = false;
            Debug.Log($"Player exited portal zone: {portalID}");
        }
    }


    private void Start()
    {
        // Kiểm tra GameManager.Instance có tồn tại không
        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager.Instance is null. Make sure GameManager is initialized and persistent.");
            return;
        }

        // Khi scene được tải, kiểm tra xem có phải người chơi nên xuất hiện ở đây không
        // Nếu targetPortalID khớp với ID của cổng này, đặt vị trí người chơi tại đây.
        if (portalID == GameManager.Instance.targetPortalID)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                CharacterController cc = player.GetComponent<CharacterController>();
                if (cc != null) cc.enabled = false; // Tắt CharacterController tạm thời để di chuyển

                player.transform.position = transform.position; // Xuất hiện ngay tại vị trí của cổng này
                Debug.Log($"Player appeared at portal: {portalID}");

                if (cc != null) cc.enabled = true; // Bật lại CharacterController

                // Xóa ID để không bị dùng lại cho các lần tải scene khác
                GameManager.Instance.targetPortalID = null;
            }
            else
            {
                Debug.LogWarning("Player GameObject not found in the scene.");
            }
        }
    }

    void Update()
    {
        if (isPlayerInZone && Input.GetKeyDown(KeyCode.F))
        {
            // Kiểm tra SaveManager.Instance có tồn tại không
            if (SaveManager.Instance == null)
            {
                Debug.LogError("SaveManager.Instance is null. Make sure SaveManager is initialized and persistent.");
                return;
            }

            // 1. Lưu game trước khi chuyển scene
            Debug.Log($"Saving game to slot {saveSlotNumber} before portal transition.");
            SaveManager.Instance.SaveGame(saveSlotNumber); // Tự động lưu game vào slot đã chỉ định

            // 2. Lưu ID của cổng đích vào GameManager
            // ID này sẽ được PortalController ở scene đích sử dụng để đặt vị trí người chơi
            GameManager.Instance.targetPortalID = targetPortalID;

            // 3. Tải scene mới
            Debug.Log($"Loading scene: {sceneToLoad}");
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}