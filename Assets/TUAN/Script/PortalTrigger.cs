using UnityEngine;
using System.Collections;

/// <summary>

/// </summary>
public class PortalTrigger : MonoBehaviour
{
    [Header("Cài đặt Cổng")]
    [Tooltip("Số thứ tự của cổng này, dùng để hiển thị thông báo.")]
    public int portalNumber = 1;

    [Tooltip("Tag của chìa khóa cần thiết để mở cổng này (ví dụ: 'key1', 'key2'...).")]
    public string requiredKeyTag = "key1";

    [Header("Điểm Dịch Chuyển")]
    [Tooltip("Kéo thả GameObject là điểm đến của người chơi vào đây.")]
    public Transform teleportDestination;
    
    // Biến để kiểm soát việc hiển thị tin nhắn, tránh spam console
    private bool canShowMessage = true; 

    // Hàm này được gọi tự động khi một đối tượng khác đi vào vùng trigger
    private void OnTriggerEnter(Collider other)
    {
        // 1. Kiểm tra xem đối tượng va chạm có phải là người chơi không (dựa trên tag "Player")
        if (other.CompareTag("Player"))
        {
            Debug.Log($"Bạn đang ở gần Cổng Dịch Chuyển số {portalNumber}.");

            // 2. Kiểm tra xem người chơi có "giữ" vật phẩm với tag yêu cầu không
            if (PlayerHasKeyItem(other.gameObject, requiredKeyTag))
            {
                // Nếu có chìa khóa -> Dịch chuyển
                Debug.Log("Đã xác nhận chìa khóa! Đang dịch chuyển...");
                TeleportPlayer(other.gameObject);
            }
            else
            {
                // Nếu không có chìa khóa -> Thông báo
                if (canShowMessage)
                {
                    Debug.Log("You need a key to Continue");
                    // Đặt một khoảng nghỉ ngắn trước khi hiện lại thông báo này
                    StartCoroutine(MessageCooldownRoutine());
                }
            }
        }
    }

    /// <summary>
    /// Kiểm tra xem người chơi có một đối tượng con (child object) với tag được chỉ định không.
    /// </summary>
    /// <param name="player">Đối tượng người chơi để kiểm tra.</param>
    /// <param name="keyTag">Tag của chìa khóa cần tìm.</param>
    /// <returns>True nếu tìm thấy, False nếu không.</returns>
    private bool PlayerHasKeyItem(GameObject player, string keyTag)
    {
        // Duyệt qua tất cả các đối tượng con của người chơi
        foreach (Transform child in player.transform)
        {
            // Nếu tìm thấy một đứa con có tag trùng khớp
            if (child.CompareTag(keyTag))
            {
                return true; // Trả về true ngay lập tức
            }
        }
        
        // Nếu duyệt hết mà không tìm thấy, trả về false
        return false;
    }

    /// <summary>
    /// Hàm thực hiện việc dịch chuyển người chơi đến điểm đến.
    /// </summary>
    private void TeleportPlayer(GameObject player)
    {
        if (teleportDestination == null)
        {
            Debug.LogError($"LỖI: Cổng {portalNumber} chưa được thiết lập điểm đến (Teleport Destination)!");
            return;
        }

        // Tạm thời vô hiệu hóa CharacterController để việc dịch chuyển vị trí hoạt động đúng
        CharacterController cc = player.GetComponent<CharacterController>();
        if (cc != null)
        {
            cc.enabled = false;
        }

        // Dịch chuyển người chơi đến vị trí của điểm đến
        player.transform.position = teleportDestination.position;
        // Tùy chọn: Đồng bộ cả hướng quay của người chơi với điểm đến
        player.transform.rotation = teleportDestination.rotation; 

        // Kích hoạt lại CharacterController sau khi dịch chuyển
        if (cc != null)
        {
            cc.enabled = true;
        }
    }

    /// <summary>
    /// Coroutine để tạo độ trễ 3 giây cho việc hiển thị lại thông báo thiếu chìa khóa.
    /// </summary>
    private IEnumerator MessageCooldownRoutine()
    {
        canShowMessage = false;
        yield return new WaitForSeconds(3.0f); // Chờ 3 giây
        canShowMessage = true;
    }
}