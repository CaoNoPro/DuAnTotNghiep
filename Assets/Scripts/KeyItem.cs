using UnityEngine;

public enum KeyType { Red, Blue, Green, Black }

public class KeyItem : MonoBehaviour
{
    public KeyType keyType;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerInventory inventory = other.GetComponent<PlayerInventory>();
            if (inventory != null)
            {
                inventory.CollectKey(keyType);
                Destroy(gameObject);
            }
        }
    }
}
