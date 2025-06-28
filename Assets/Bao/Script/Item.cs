using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    public string itemName; // Tên vật phẩm
    public Sprite icon;     // Icon hiển thị
    public GameObject prefab; // Prefab vật phẩm trong game 3D
}
