using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public GameObject inventoryPanel; // Tham chiếu đến Panel Inventory

    void Update()
    {
        // Kiểm tra nếu phím "I" được nhấn
        if (Input.GetKeyDown(KeyCode.I))
        {
            ToggleInventory();
        }
    }

    void ToggleInventory()
    {
        // Đảo ngược trạng thái của InventoryPanel
        inventoryPanel.SetActive(!inventoryPanel.activeSelf);
    }
}
