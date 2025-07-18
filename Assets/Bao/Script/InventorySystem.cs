using UnityEngine;

public class InventorySystem : MonoBehaviour
{
    [Header("UI References")]
    public GameObject inventoryPanel;
    public KeyCode toggleKey = KeyCode.I;
    public KeyCode closeKey = KeyCode.Escape;

    [Header("Slot Settings")]
    public InventorySlot[] slots;

    private bool isOpen = false;

    void Start()
    {
        if (inventoryPanel != null)
            inventoryPanel.SetActive(false);
    }

    void Update()
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
        inventoryPanel.SetActive(isOpen);
        Time.timeScale = isOpen ? 0 : 1;
        Cursor.lockState = isOpen ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = isOpen;
    }

    public void CloseInventory()
    {
        isOpen = false;
        inventoryPanel.SetActive(false);
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public bool AddItem(ItemData itemData)
    {
        foreach (InventorySlot slot in slots)
        {
            if (!slot.HasItem)
            {
                slot.SetItem(itemData);
                return true;
            }
        }

        // Nếu không còn slot trống
        Debug.Log("Inventory đầy, không thể thêm: " + itemData.itemName);
        return false;
    }

}