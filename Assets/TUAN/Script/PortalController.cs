using UnityEngine;
using UnityEngine.SceneManagement;

// Đặt enum KeyType ở đây để PortalController có thể sử dụng.
// Đảm bảo nó khớp với enum bạn dùng trong các script khác.


public class PortalController : MonoBehaviour
{
    public enum PortalRequirement
    {
        NoRequirement,
        RequiresKeyBlue,
        RequiresKeyBlack,
        RequiresKeyGreen,
        RequiresAllKeys
    }

    [Header("Cấu hình Cổng Dịch Chuyển")]
    public PortalRequirement requirement;
    public string portalID;
    public string targetPortalID;
    public string sceneToLoad;

    private bool isPlayerInZone = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) isPlayerInZone = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) isPlayerInZone = false;
    }

    private void Start()
    {
        // Logic này không cần thay đổi
        if (GameManager.Instance != null && !string.IsNullOrEmpty(GameManager.Instance.targetPortalID) && portalID == GameManager.Instance.targetPortalID)
        {
            // Code dịch chuyển người chơi đến vị trí cổng
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                CharacterController cc = player.GetComponent<CharacterController>();
                if (cc != null) cc.enabled = false;
                player.transform.position = transform.position;
                if (cc != null) cc.enabled = true;
            }
            GameManager.Instance.targetPortalID = null;
        }
    }

    private void Update()
    {
        if (isPlayerInZone && Input.GetKeyDown(KeyCode.F))
        {
            if (CheckRequirement())
            {
                // Lưu ID và tải scene mới
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.targetPortalID = targetPortalID;
                    SceneManager.LoadScene(sceneToLoad);
                }
                else
                {
                    Debug.LogError("Không tìm thấy GameManager!");
                }
            }
            else
            {
                Debug.Log("Cổng đã khóa!");
            }
        }
    }

    /// <summary>
    /// Kiểm tra điều kiện vào cổng, đã cập nhật để dùng script PlayerInventory của bạn.
    /// </summary>
    private bool CheckRequirement()
    {
        // Script này sẽ gọi đến PlayerInventory.Instance mà bạn đã cung cấp.
        // Đảm bảo bạn đã sửa lỗi Singleton trong PlayerInventory.cs! (Xem lưu ý bên dưới)
        
        switch (requirement)
        {
            case PortalRequirement.NoRequirement:
                return true;

            case PortalRequirement.RequiresKeyBlue:
                return PlayerInventory.Instance.HasKey(KeyType.BlueKey);

            case PortalRequirement.RequiresKeyBlack:
                return PlayerInventory.Instance.HasKey(KeyType.BlackKey);

            case PortalRequirement.RequiresKeyGreen:
                return PlayerInventory.Instance.HasKey(KeyType.GreenKey);

            case PortalRequirement.RequiresAllKeys:
                return PlayerInventory.Instance.HasKey(KeyType.BlueKey) &&
                       PlayerInventory.Instance.HasKey(KeyType.BlackKey) &&
                       PlayerInventory.Instance.HasKey(KeyType.GreenKey) &&
                       PlayerInventory.Instance.HasKey(KeyType.RedKey);

            default:
                return false;
        }
    }
}