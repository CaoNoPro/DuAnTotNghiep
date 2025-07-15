using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerVirtual : MonoBehaviour
{
    [Header("Vital Stats")]
    public Slider HealthSlider;
    public int maxHealth;
    public float healFallRate;

    public Slider ThirstSlider;
    public int maxThirst;
    public float thirstFallRate;

    public Slider HungerSlider;
    public int maxHunger;
    public float hungerFallRate;

    public Slider StaminaSlider;
    public int maxStamina;
    public float staminaFallRatePerSecond = 10f;
    public float staminaRegainRatePerSecond = 5f;

    [Header("Game State")]
    public bool isDead = false;
    public GameObject GameOverUI;

    [Header("Inventory")]
    public GameObject inventoryPanel;
    public InventorySlot[] inventorySlots;
    private bool inventoryOpen = false;

    private void Start()
    {
        // Setup stats
        HealthSlider.maxValue = maxHealth;
        HealthSlider.value = maxHealth;

        HungerSlider.maxValue = maxHunger;
        HungerSlider.value = maxHunger;

        ThirstSlider.maxValue = maxThirst;
        ThirstSlider.value = maxThirst;

        StaminaSlider.maxValue = maxStamina;
        StaminaSlider.value = maxStamina;

        // Setup UI
        if (GameOverUI != null)
            GameOverUI.SetActive(false);

        if (inventoryPanel != null)
            inventoryPanel.SetActive(false);
    }

    private void Update()
    {
        if (isDead) return;

        if (Input.GetKeyDown(KeyCode.I))
        {
            inventoryOpen = !inventoryOpen;
            inventoryPanel.SetActive(inventoryOpen);

            Cursor.lockState = inventoryOpen ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = inventoryOpen;
            Time.timeScale = inventoryOpen ? 0f : 1f;
        }

        HandleVitals();
        HandleStamina();
    }

    private void HandleVitals()
    {
        if (HungerSlider.value <= 0 && ThirstSlider.value <= 0)
            HealthSlider.value -= Time.deltaTime / healFallRate * 2;
        else if (HungerSlider.value <= 0 || ThirstSlider.value <= 0)
            HealthSlider.value -= Time.deltaTime / healFallRate;
        else
        {
            HealthSlider.value += Time.deltaTime / healFallRate;
            HealthSlider.value = Mathf.Min(HealthSlider.value, maxHealth);
        }

        if (HealthSlider.value <= 0)
            CharacterDead();

        HungerSlider.value -= Time.deltaTime / hungerFallRate;
        HungerSlider.value = Mathf.Clamp(HungerSlider.value, 0, maxHunger);

        ThirstSlider.value -= Time.deltaTime / thirstFallRate;
        ThirstSlider.value = Mathf.Clamp(ThirstSlider.value, 0, maxThirst);
    }

    private void HandleStamina()
    {
        bool isSprinting = Input.GetKey(KeyCode.LeftShift) && StaminaSlider.value > 0;

        if (isSprinting)
            StaminaSlider.value -= staminaFallRatePerSecond * Time.deltaTime;
        else if (StaminaSlider.value < maxStamina)
            StaminaSlider.value += staminaRegainRatePerSecond * Time.deltaTime;

        StaminaSlider.value = Mathf.Clamp(StaminaSlider.value, 0, maxStamina);
    }

    public void TakeDamage(int damageAmount)
    {
        HealthSlider.value -= damageAmount;
        if (HealthSlider.value <= 0 && !isDead)
            CharacterDead();
    }

    public void CharacterDead()
    {
        Debug.Log("Player is dead!");
        isDead = true;

        foreach (Collider col in GetComponents<Collider>())
            col.enabled = false;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
            rb.isKinematic = true;

        if (GameOverUI != null)
            GameOverUI.SetActive(true);
    }

    public void AddItemToInventory(ItemData newItem)
    {
        foreach (InventorySlot slot in inventorySlots)
        {
            if (!slot.icon.enabled)
            {
                slot.SetItem(newItem);
                return;
            }
        }

        Debug.Log("Inventory ??y!");
    }
}
