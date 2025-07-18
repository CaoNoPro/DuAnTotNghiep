using UnityEngine;

[CreateAssetMenu(fileName = "NewItemData", menuName = "Inventory/Item Data")]
public class ItemData : ScriptableObject
{
    public string itemName = "New Item";
    public Sprite icon;
    public ItemType itemType = ItemType.Other;

    [Header("Effects")]
    public float healAmount = 0;
    public float hungerRestore = 0;
    public float thirstRestore = 0;
}
