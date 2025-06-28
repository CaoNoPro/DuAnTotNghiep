using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalController : MonoBehaviour
{
    [Tooltip("ID độc nhất của cổng này, ví dụ: 'FromMainToMiniMap1'")]
    public string portalID;
    
    [Tooltip("ID của cổng ở scene đích mà người chơi sẽ xuất hiện")]
    public string targetPortalID;

    [Tooltip("Tên của scene sẽ được tải")]
    public string sceneToLoad;

    private bool isPlayerInZone = false;

    // ... (Giữ nguyên OnTriggerEnter và OnTriggerExit) ...
    private void OnTriggerEnter(Collider other) { if (other.CompareTag("Player")) isPlayerInZone = true; }
    private void OnTriggerExit(Collider other) { if (other.CompareTag("Player")) isPlayerInZone = false; }


    private void Start()
    {
        // Khi scene được tải, kiểm tra xem có phải người chơi nên xuất hiện ở đây không
        if (portalID == GameManager.Instance.targetPortalID)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            CharacterController cc = player.GetComponent<CharacterController>();
            if (cc != null) cc.enabled = false;

            player.transform.position = transform.position; // Xuất hiện ngay tại vị trí của cổng này
            
            if (cc != null) cc.enabled = true;

            // Xóa ID để không bị dùng lại
            GameManager.Instance.targetPortalID = null;
        }
    }

    void Update()
    {
        if (isPlayerInZone && Input.GetKeyDown(KeyCode.F))
        {
            // Lưu ID của cổng đích vào GameManager
            GameManager.Instance.targetPortalID = targetPortalID;
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}