using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EquipSystem : MonoBehaviour
{
    public static EquipSystem Instance { get; set; }

    //UI
    public GameObject quickSlotPanel;

    public List<GameObject> quickSlotsList = new List<GameObject>();
    public List<string> itemList = new List<string>();

    public GameObject numberHolder;

    public int selectedNumber = -1;
    public GameObject selectedItem;
    public Transform playerWeaponEquipPoint; // Kéo thả GameObject "WeaponEquipPoint" của người chơi vào đây trong Inspector

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        PopulateSlotList();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) SelectQuickSlot(1);
        else if (Input.GetKeyDown(KeyCode.Alpha2)) SelectQuickSlot(2);
        else if (Input.GetKeyDown(KeyCode.Alpha3)) SelectQuickSlot(3);
        else if (Input.GetKeyDown(KeyCode.Alpha4)) SelectQuickSlot(4);
        else if (Input.GetKeyDown(KeyCode.Alpha5)) SelectQuickSlot(5);
        else if (Input.GetKeyDown(KeyCode.Alpha6)) SelectQuickSlot(6);

        // Mới: Sử dụng vật phẩm khi nhấn một phím (ví dụ: Space hoặc nút chuột trái)
        if (selectedItem != null && Input.GetMouseButtonDown(0)) // Left mouse click
        {
            InventoryItem itemComponent = selectedItem.GetComponent<InventoryItem>();
            if (itemComponent != null)
            {
                // Nếu là vũ khí, gọi hàm EquipWeapon
                if (itemComponent.isWeapon)
                {
                    itemComponent.EquipWeapon(playerWeaponEquipPoint);
                }
                else
                {
                    itemComponent.Use(); // Sử dụng vật phẩm không phải vũ khí
                }
            }
        }
    }

    void SelectQuickSlot(int number)
    {
        // ... (phần code bạn đã sửa ở trên với TextMeshProUGUI) ...
        if (checkIfSlotIsFull(number))
        {
            // Bỏ chọn vật phẩm trước đó (nếu có)
            if (selectedItem != null)
            {
                InventoryItem previousItem = selectedItem.GetComponent<InventoryItem>();
                if (previousItem != null)
                {
                    previousItem.isSelected = false;
                    // Mới: Nếu vật phẩm trước là vũ khí, hãy bỏ trang bị nó
                    if (previousItem.isWeapon)
                    {
                        previousItem.UnequipWeapon(playerWeaponEquipPoint);
                    }
                }
            }

            selectedNumber = number;
            selectedItem = getSelectedItem(number);

            // Chọn vật phẩm mới
            if (selectedItem != null)
            {
                InventoryItem currentItem = selectedItem.GetComponent<InventoryItem>();
                if (currentItem != null)
                {
                    currentItem.isSelected = true;
                    // Mới: Nếu vật phẩm mới là vũ khí, hãy trang bị nó ngay lập tức
                    if (currentItem.isWeapon)
                    {
                        currentItem.EquipWeapon(playerWeaponEquipPoint);
                    }
                }
            }
            else
            {
                // Nếu không có vật phẩm nào được chọn (slot trống hoặc lỗi)
                selectedNumber = -1;
                // Đảm bảo không có vũ khí nào được trang bị
                if (playerWeaponEquipPoint != null)
                {
                    foreach (Transform child in playerWeaponEquipPoint)
                    {
                        Destroy(child.gameObject);
                    }
                }
            }


            // Đổi màu số hiển thị (phần đã sửa lỗi TextMeshProUGUI)
            if (numberHolder != null)
            {
                foreach (Transform child in numberHolder.transform)
                {
                    Transform textChild = child.transform.Find("Text");
                    if (textChild != null)
                    {
                        TextMeshProUGUI textMesh = textChild.GetComponent<TextMeshProUGUI>();
                        if (textMesh != null)
                        {
                            textMesh.color = Color.gray;
                        }
                    }
                }

                Transform targetNumberTransform = numberHolder.transform.Find("Number" + number);
                if (targetNumberTransform != null)
                {
                    Transform toBeChangedTextChild = targetNumberTransform.transform.Find("Text");
                    if (toBeChangedTextChild != null)
                    {
                        TextMeshProUGUI toBeChanged = toBeChangedTextChild.GetComponent<TextMeshProUGUI>();
                        if (toBeChanged != null)
                        {
                            toBeChanged.color = Color.white;
                        }
                        else
                        {
                            Debug.LogError($"TextMeshProUGUI component not found on 'Text' child of 'Number{number}' under 'numberHolder'.");
                        }
                    }
                    else
                    {
                        Debug.LogError($"'Text' child not found under 'Number{number}' within 'numberHolder'.");
                    }
                }
                else
                {
                    Debug.LogError($"'Number{number}' child not found directly under 'numberHolder'. Please check your UI hierarchy.");
                }
            }
            else
            {
                Debug.LogError("numberHolder is not assigned in the Inspector of EquipSystem!");
            }
        }
        else // if checkIfSlotIsFull(number) == false
        {
            selectedNumber = -1;

            if (selectedItem != null)
            {
                InventoryItem previousItem = selectedItem.gameObject.GetComponent<InventoryItem>();
                if (previousItem != null)
                {
                    previousItem.isSelected = false;
                    // Mới: Nếu slot trống, bỏ trang bị vũ khí cũ
                    if (previousItem.isWeapon)
                    {
                        previousItem.UnequipWeapon(playerWeaponEquipPoint);
                    }
                }
            }
            // Đảm bảo không có vũ khí nào được trang bị nếu không có gì trong quick slot
            if (playerWeaponEquipPoint != null)
            {
                foreach (Transform child in playerWeaponEquipPoint)
                {
                    Destroy(child.gameObject);
                }
            }


            foreach (Transform child in numberHolder.transform)
            {
                Transform textChild = child.transform.Find("Text");
                if (textChild != null)
                {
                    TextMeshProUGUI textMesh = textChild.GetComponent<TextMeshProUGUI>();
                    if (textMesh != null)
                    {
                        textMesh.color = Color.gray;
                    }
                }
            }
        }
    }

    GameObject getSelectedItem(int slotNumber)
    {
        // Kiểm tra xem quickSlotsList có đủ phần tử không
        if (slotNumber - 1 < quickSlotsList.Count && quickSlotsList[slotNumber - 1].transform.childCount > 0)
        {
            return quickSlotsList[slotNumber - 1].transform.GetChild(0).gameObject;
        }
        return null; // Trả về null nếu không có vật phẩm
    }

    bool checkIfSlotIsFull(int slotNumber)
    {
        // Kiểm tra hợp lệ index trước
        if (slotNumber - 1 >= 0 && slotNumber - 1 < quickSlotsList.Count)
        {
            return quickSlotsList[slotNumber - 1].transform.childCount > 0;
        }
        return false; // Nếu index không hợp lệ, coi như không đầy
    }

    private void PopulateSlotList()
    {
        if (quickSlotPanel != null)
        {
            foreach (Transform child in quickSlotPanel.transform)
            {
                if (child.CompareTag("QuickSlot"))
                {
                    quickSlotsList.Add(child.gameObject);
                }
            }
        }
        else
        {
            Debug.LogError("quickSlotPanel is not assigned in the Inspector of EquipSystem!");
        }
    }

    public void AddToQuickSlots(GameObject itemToEquip)
    {
        GameObject availableSlot = FindNextEmptySlot();

        if (availableSlot != null && availableSlot.name != "New Game Object") // Tránh GameObject rỗng được tạo
        {
            itemToEquip.transform.SetParent(availableSlot.transform, false);

            string cleanName = itemToEquip.name.Replace("(clone)", "");

            itemList.Add(cleanName);
        }
        else
        {
            Debug.LogWarning("No empty quick slot available or FindNextEmptySlot returned an invalid object.");
            // Có thể thêm logic để bỏ vật phẩm hoặc thông báo cho người chơi
        }
    }

    public GameObject FindNextEmptySlot()
    {
        foreach (GameObject slot in quickSlotsList)
        {
            if (slot.transform.childCount == 0)
            {
                return slot;
            }
        }
        // Thay vì trả về new GameObject(), trả về null và xử lý bên gọi
        return null;
    }

    public bool CheckIfFull()
    {
        int counter = 0;
        foreach (GameObject slot in quickSlotsList)
        {
            if (slot.transform.childCount > 0)
            {
                counter += 1;
            }
        }
        // So sánh với số lượng quick slots thực tế
        return counter == quickSlotsList.Count;
    }
}