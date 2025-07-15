using UnityEngine;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    private List<string> inventory = new List<string>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void AddItem(string itemName)
    {
        inventory.Add(itemName);
        Debug.Log("Đã thêm vào kho: " + itemName);
    }

    public bool HasItem(string itemName)
    {
        return inventory.Contains(itemName);
    }
}
