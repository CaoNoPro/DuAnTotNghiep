using UnityEngine;
using UnityEngine.UI;

public class InventorySystem : MonoBehaviour
{
    [Header("UI References")]
    public GameObject inventoryPanel; // Panel chứa toàn bộ UI inventory
    public KeyCode toggleKey = KeyCode.I; // Phím bật/tắt inventory
    public KeyCode closeKey = KeyCode.Y; // Phím đóng nhanh

    [Header("Slot Settings")] 
    public GameObject slotPrefab;
    public Transform slotsParent;
    public int slotCount = 12;

    private bool isOpen = false;

    void Start()
    {
        // Đảm bảo panel tắt khi bắt đầu
        if (inventoryPanel != null)
            inventoryPanel.SetActive(false);
        
        InitializeSlots();
    }

    void Update()
    {
        HandleInput();
    }

    void HandleInput()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            ToggleInventory();
        }
        
        if (Input.GetKeyDown(closeKey) && isOpen)
        {
            CloseInventory();
        }
    }

    public void ToggleInventory()
    {
        isOpen = !isOpen;
        
        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(isOpen);
            
            // Tạm dừng game khi mở inventory (tuỳ chọn)
            Time.timeScale = isOpen ? 0f : 1f;
            
            // Hiển thị con trỏ chuột
            Cursor.lockState = isOpen ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = isOpen;
        }
    }

    public void CloseInventory()
    {
        isOpen = false;
        inventoryPanel.SetActive(false);
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void InitializeSlots()
    {
        if (slotPrefab == null || slotsParent == null)
        {
            Debug.LogError("Chưa gán slotPrefab hoặc slotsParent!");
            return;
        }

        for (int i = 0; i < slotCount; i++)
        {
            Instantiate(slotPrefab, slotsParent);
        }
    }
}
