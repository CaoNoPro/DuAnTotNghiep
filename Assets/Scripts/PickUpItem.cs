using UnityEngine;

public class PickupItem : MonoBehaviour
{
    public ItemData itemData;
    public bool destroyOnPickup = true;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            InventorySystem inventory = FindObjectOfType<InventorySystem>();
            if (inventory != null)
            {
                bool success = inventory.AddItem(itemData);
                if (success && destroyOnPickup)
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}
