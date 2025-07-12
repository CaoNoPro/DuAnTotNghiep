using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventoryItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("UI")]
    public Image image;

    [HideInInspector] public Transform parentAfterDrag;

    private InventorySystem inventorySystem;

    void Start()
    {
        // Tìm system để kiểm tra trạng thái mở balo
        inventorySystem = FindObjectOfType<InventorySystem>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!IsInventoryOpen()) return;

        image.raycastTarget = false;
        parentAfterDrag = transform.parent;
        transform.SetParent(transform.root);
        transform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!IsInventoryOpen()) return;

        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!IsInventoryOpen()) return;

        image.raycastTarget = true;
        transform.SetParent(parentAfterDrag);
    }

    bool IsInventoryOpen()
    {
        return inventorySystem != null && inventorySystem.gameObject.activeInHierarchy && inventorySystem.enabled;
    }
}
