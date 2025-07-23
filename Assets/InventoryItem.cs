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

    private void Start()
    {
        itemInfoUI = InventorySystem.Instance.ItemInfoUI;
        itemInfoUI_itemName = itemInfoUI.transform.Find("itemName").GetComponent<TextMeshProUGUI>();
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
