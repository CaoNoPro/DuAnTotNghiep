using System.Collections.Generic;
using UnityEngine;




public class PlayerInventory : MonoBehaviour
{
    // Lưu chìa khóa theo loại
    private HashSet<KeyType> collectedKeys = new HashSet<KeyType>();

    // Lưu các item khác
    private HashSet<ItemType> inventory = new HashSet<ItemType>();

    // Thêm chìa khóa
    public void CollectKey(KeyType keyType)
    {
        collectedKeys.Add(keyType);
        Debug.Log("Collected Key: " + keyType);
    }

    // Kiểm tra chìa khóa
    public bool HasKey(KeyType keyType)
    {
        return collectedKeys.Contains(keyType);
    }

    // Thêm item thường
    public void AddItem(ItemType item)
    {
        inventory.Add(item);
        Debug.Log("Collected Item: " + item);
    }

    // Kiểm tra item thường
    public bool HasItem(ItemType item)
    {
        return inventory.Contains(item);
    }
}
