using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler ,IPointerUpHandler
{
    // vứt vật phẩm
    public bool isTrashable;

    // thông tin vật phẩm
    private GameObject itemInfoUI;

    private TextMeshProUGUI itemInfoUI_itemName;

    public string thisName, thisDes;

    // sử dụng vật phẩm trong túi đồ
    private GameObject itemPendingConsumption;
    public bool isConsumable;

    public float healthEffect;
    public float thirstEffect;
    public float hungerEffect;

    //trang bị vật phẩm
    public bool isEquippable;
    public GameObject itemPendingEquipping;
    public bool isInsideQuickSlot;
    public bool isSelected;

    public string itemName;
    public Sprite icon;
    public bool isWeapon = false;
    public GameObject weapondPrefab; // Corrected typo here, but it was `weapondPrefab` in your original. Let's keep `weapondPrefab` for consistency with your code.

    // Changed 'use' to 'Use'
    public virtual void Use()
    {
        Debug.Log($"Using {itemName}");
    }

    public void EquipWeapon(Transform equipPoint)
    {
        if (isWeapon && weapondPrefab != null)
        {
            // Clear any existing weapons at the equip point
            foreach (Transform child in equipPoint)
            {
                Destroy(child.gameObject);
            }

            GameObject equippedWeapon = Instantiate(weapondPrefab, equipPoint);
            equippedWeapon.transform.localPosition = Vector3.zero;
            equippedWeapon.transform.localRotation = Quaternion.identity;
            equippedWeapon.transform.localScale = Vector3.one;
        }
    }

    // Changed 'UnEquipWeapon' to 'UnequipWeapon'
    public void UnequipWeapon(Transform equipPoint)
    {
        if (isWeapon)
        {
            foreach (Transform child in equipPoint)
            {
                Destroy(child.gameObject);
            }
        }
    }
    private void Start()
    {
        itemInfoUI = InventorySystem.Instance.ItemInfoUI;
        itemInfoUI_itemName = itemInfoUI.transform.Find("itemName").GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        if (isSelected)
        {
            gameObject.GetComponent<DragDrop>().enabled = false;
        }
        else
        {
            gameObject.GetComponent<DragDrop>().enabled = true;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        itemInfoUI.SetActive(true);
        itemInfoUI_itemName.text = thisName;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        itemInfoUI.SetActive(false);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (isConsumable)
            {
                itemPendingConsumption = gameObject;
                ConsumFunction(healthEffect, thirstEffect, hungerEffect);
            }

            if (isEquippable && isInsideQuickSlot == false && EquipSystem.Instance.CheckIfFull() == false)
            {
                EquipSystem.Instance.AddToQuickSlots(gameObject);
                isInsideQuickSlot = true;
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (isConsumable && itemPendingConsumption == gameObject)
            {
                Destroy(gameObject);
                InventorySystem.Instance.ReCalculateList();
                CraftingSystem.Instance.RefreshNeededItems();
            }
        }
    }

    private void ConsumFunction(float healthEffect, float thirstEffect, float hungerEffect)
    {
        itemInfoUI.SetActive(false);
        healthEffectCalculate(healthEffect);
        thirstEffectCalculate(thirstEffect);
        hungerEffectCalculate(hungerEffect);
    }

    private static void healthEffectCalculate(float healthEffect)
    {
        float healthBeforeConsumption = PlayerState.Instance.currentHealth;
        float maxHealth = PlayerState.Instance.maxHealth;

        if (healthEffect != 0)
        {
            if ((healthBeforeConsumption + healthEffect) > maxHealth)
            {
                PlayerState.Instance.setHealth(maxHealth);
            }
            else
            {
                PlayerState.Instance.setHealth(healthBeforeConsumption + healthEffect);
            }
        }
    }
    private static void thirstEffectCalculate(float thirstEffect)
    {
        float thirstBeforeConsumption = PlayerState.Instance.currentThirst;
        float maxThirst = PlayerState.Instance.maxThirst;

        if (thirstEffect != 0)
        {
            if ((thirstBeforeConsumption + thirstEffect) > maxThirst)
            {
                PlayerState.Instance.setThirst(maxThirst);
            }
            else
            {
                PlayerState.Instance.setThirst(thirstBeforeConsumption + thirstEffect);
            }
        }
    }
    private static void hungerEffectCalculate(float hungerEffect)
    {
        float hungerBeforeConsumption = PlayerState.Instance.currentHunger;
        float maxHunger = PlayerState.Instance.maxHunger;

        if (hungerEffect != 0)
        {
            if ((hungerBeforeConsumption + hungerEffect) > maxHunger)
            {
                PlayerState.Instance.setHunger(maxHunger);
            }
            else
            {
                PlayerState.Instance.setHunger(hungerBeforeConsumption + hungerEffect);
            }
        }
    }
}