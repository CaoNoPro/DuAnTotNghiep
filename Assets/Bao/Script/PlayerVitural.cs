using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerVirtual : MonoBehaviour
{
    public TextMeshProUGUI healthCounter;
    public GameObject playerState;
    //player heal
    [Header("player heal")]
    public Slider HealthSlider;
    public int maxHealth;
    public float healFallRate;

    //player thirst
    [Header("player thirst")]
    public Slider ThirstSlider;
    public int maxThirst;
    public float thirstFallRate;

    //player hunger
    [Header("player hunger")]
    public Slider HungerSlider;
    public int maxHunger;
    public float hungerFallRate;

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

        // --- Health Regeneration/Degeneration ---
        if (HungerSlider.value <= 0 && ThirstSlider.value <= 0)
        {
            HealthSlider.value -= Time.deltaTime / healFallRate * 2;
        }
        else if (HungerSlider.value <= 0 || ThirstSlider.value <= 0)
        {
            HealthSlider.value -= Time.deltaTime / healFallRate;
        }
        else
        {
            HealthSlider.value += Time.deltaTime / healFallRate;
            HealthSlider.value = Mathf.Min(HealthSlider.value, maxHealth); 
        }

        if (HealthSlider.value <= 0)
        {
            Debug.Log("PlayerVirtual: Health <= 0. Attempting to call CharacterDead(). Current Health: " + HealthSlider.value);
            CharacterDead();
        }
        else
        {
        }

        // --- Hunger Controller ---
        if (HungerSlider.value > 0)
        {
            HungerSlider.value -= Time.deltaTime / hungerFallRate;
        }
        else
        {
            HungerSlider.value = 0;
        }
        HungerSlider.value = Mathf.Clamp(HungerSlider.value, 0, maxHunger);
        // Debug.Log("Current Hunger: " + HungerSlider.value);


        // --- Thirst Controller ---
        if (ThirstSlider.value > 0)
        {
            ThirstSlider.value -= Time.deltaTime / thirstFallRate;
        }
        else
        {
            ThirstSlider.value = 0;
        }
        ThirstSlider.value = Mathf.Clamp(ThirstSlider.value, 0, maxThirst);
        // Debug.Log("Current Thirst: " + ThirstSlider.value);

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
