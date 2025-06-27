using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    public Image icon; // Hình ảnh hiển thị icon của item
    public GameObject itemInSlot; // Đối tượng vật phẩm trong slot

    public bool HasItem => itemInSlot != null; // Kiểm tra xem slot có item hay không

    public void SetItem(Item item)
    {
        if (item == null || item.prefab == null) return;

        // Tạo instance của prefab và gán vào slot
        itemInSlot = Instantiate(item.prefab, transform);
        icon.sprite = item.icon; // Gán icon cho slot
        icon.enabled = true; // Hiển thị icon
    }

    public void ClearSlot()
    {
        if (itemInSlot != null)
        {
            Destroy(itemInSlot); // Xóa item trong slot
        }
        icon.sprite = null; // Xóa icon
        icon.enabled = false; // Ẩn icon
    }

    public void OnDrop(PointerEventData eventData)
    {
        GameObject dropped = eventData.pointerDrag;
        if (dropped == null) return;

        DraggableItem draggableItem = dropped.GetComponent<DraggableItem>();
        if (draggableItem != null)
        {
            // Xử lý kéo thả vật phẩm
            if (HasItem)
            {
                // Hoán đổi vật phẩm nếu slot đã có item
                Item currentItem = itemInSlot.GetComponent<Item>();
                draggableItem.parentSlot.SetItem(currentItem);
            }

            draggableItem.parentSlot = this; // Gán slot hiện tại cho draggable item
            SetItem(draggableItem.GetComponent<Item>()); // Đặt item vào slot
        }
    }
}
