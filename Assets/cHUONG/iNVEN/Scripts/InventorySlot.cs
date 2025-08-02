using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySlot : MonoBehaviour
{
    public Image icon;
    public TextMeshProUGUI itemNameText; // Optional, to display item name
    public Button useButton; // The button for using the item

    private ItemData currentItem;
    private PlayerVirtual playerVirtual; // Reference to your PlayerVirtual script

    public bool HasItem { get; internal set; }
    public object CurrentItem { get; internal set; }

    void Awake()
    {
        // Get a reference to the PlayerVirtual script when the slot is created
        playerVirtual = FindObjectOfType<PlayerVirtual>();
        if (playerVirtual == null)
        {
            Debug.LogError("PlayerVirtual script not found in the scene! Make sure it's attached to your player.");
        }

        // Add a listener to the use button
        if (useButton != null)
        {
            useButton.onClick.AddListener(UseItemInSlot);
        }

        // Initially hide the button and icon if no item
        ClearSlot();
    }

    public void SetItem(ItemData newItem)
    {
        currentItem = newItem;
        if (currentItem != null)
        {
            icon.sprite = currentItem.icon;
            icon.enabled = true;
            if (itemNameText != null)
                itemNameText.text = currentItem.itemName;

            if (useButton != null)
                useButton.interactable = true; // Make button interactable when there's an item
        }
    }

    public void ClearSlot()
    {
        currentItem = null;
        icon.sprite = null;
        icon.enabled = false;
        if (itemNameText != null)
            itemNameText.text = "";

        if (useButton != null)
            useButton.interactable = false; // Disable button when slot is empty
    }

    // This method is called when the useButton is clicked
    private void UseItemInSlot()
    {
        if (currentItem != null && playerVirtual != null)
        {
            playerVirtual.UseItem(currentItem); // Call the UseItem method on PlayerVirtual
            ClearSlot(); // Remove the item from this slot after use
        }
    }
}