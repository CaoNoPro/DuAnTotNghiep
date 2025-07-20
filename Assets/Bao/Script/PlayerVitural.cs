using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerVirtual : MonoBehaviour
{
    public TextMeshProUGUI healthCounter; // You had this, but it's not currently used to update the text display from HealthSlider.value
    public GameObject playerState; // This seems redundant as this script is likely the "player state"

    //player heal
    [Header("player heal")]
    public Slider HealthSlider;
    public int maxHealth;
    public float healFallRate; // Rate at which health recovers/falls due to hunger/thirst

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

    // --- Added: Item Data reference ---
    // You'll need a scriptable object or class to define your item properties
    // For example:
    // public class ItemData : ScriptableObject {
    //     public string itemName;
    //     public ItemType itemType; // Enum: Food, Drink, Healing, etc.
    //     public float healAmount;
    //     public float hungerRestore;
    //     public float thirstRestore;
    // }
    // public enum ItemType { Healing, Food, Drink, Other }


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

        // Inventory Toggle
        if (Input.GetKeyDown(KeyCode.I))
        {
            inventoryOpen = !inventoryOpen;
            inventoryPanel.SetActive(inventoryOpen);

            Cursor.lockState = inventoryOpen ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = inventoryOpen;
            Time.timeScale = inventoryOpen ? 0f : 1f;
        }

        // Handle Vitals (Health, Hunger, Thirst)
        HandleVitals();

        // Update health counter text
        // Ensure healthCounter is assigned in the Inspector and you have a PlayerState reference if needed
        if (healthCounter != null)
        {
            healthCounter.text = Mathf.CeilToInt(HealthSlider.value).ToString() + "/" + maxHealth.ToString();
        }
    }

    private void HandleVitals()
    {
        // --- Health Regeneration/Degeneration ---
        // Player loses health faster if both hungry AND thirsty
        if (HungerSlider.value <= 0 && ThirstSlider.value <= 0)
        {
            HealthSlider.value -= Time.deltaTime / healFallRate * 2;
        }
        // Player loses health if either hungry OR thirsty
        else if (HungerSlider.value <= 0 || ThirstSlider.value <= 0)
        {
            HealthSlider.value -= Time.deltaTime / healFallRate;
        }
        // Player regenerates health if neither hungry nor thirsty (and not at max health)
        else
        {
            HealthSlider.value += Time.deltaTime / healFallRate;
        }
        // Clamp health value to ensure it stays within bounds
        HealthSlider.value = Mathf.Clamp(HealthSlider.value, 0, maxHealth);


        // Check for death after health updates
        if (HealthSlider.value <= 0)
        {
            Debug.Log("PlayerVirtual: Health <= 0. Attempting to call CharacterDead(). Current Health: " + HealthSlider.value);
            CharacterDead();
        }

        // --- Hunger Controller ---
        // Hunger constantly decreases
        HungerSlider.value -= Time.deltaTime / hungerFallRate;
        // Ensure hunger doesn't go below 0 or above max
        HungerSlider.value = Mathf.Clamp(HungerSlider.value, 0, maxHunger);

        // --- Thirst Controller ---
        // Thirst constantly decreases
        ThirstSlider.value -= Time.deltaTime / thirstFallRate;
        // Ensure thirst doesn't go below 0 or above max
        ThirstSlider.value = Mathf.Clamp(ThirstSlider.value, 0, maxThirst);
    }

    public void TakeDamage(float damageAmount) // Changed to float for consistency with Time.deltaTime
    {
        if (isDead) return; // Prevent damage if already dead

        HealthSlider.value -= damageAmount;
        // Health clamping is handled in HandleVitals, but it's good to re-check after an immediate damage event
        HealthSlider.value = Mathf.Clamp(HealthSlider.value, 0, maxHealth);

        if (HealthSlider.value <= 0 && !isDead)
            CharacterDead();
    }

    // --- NEW: Method to use an item ---
    public void UseItem(ItemData itemToUse)
    {
        if (isDead) return;

        Debug.Log("Using item: " + itemToUse.itemName);

        switch (itemToUse.itemType)
        {
            case ItemType.Healing:
                HealthSlider.value += itemToUse.healAmount;
                HealthSlider.value = Mathf.Min(HealthSlider.value, maxHealth);
                Debug.Log($"Healed for {itemToUse.healAmount}. Current Health: {HealthSlider.value}");
                break;
            case ItemType.Food:
                HungerSlider.value += itemToUse.hungerRestore;
                HungerSlider.value = Mathf.Min(HungerSlider.value, maxHunger);
                Debug.Log($"Ate food, restored {itemToUse.hungerRestore} hunger. Current Hunger: {HungerSlider.value}");
                break;
            case ItemType.Drink:
                ThirstSlider.value += itemToUse.thirstRestore;
                ThirstSlider.value = Mathf.Min(ThirstSlider.value, maxThirst);
                Debug.Log($"Drank, restored {itemToUse.thirstRestore} thirst. Current Thirst: {ThirstSlider.value}");
                break;
            default:
                Debug.LogWarning("Attempted to use an item with an unhandled type: " + itemToUse.itemType);
                break;
        }
    }

    public void CharacterDead()
    {
        if (isDead) return; // Prevent calling multiple times

        Debug.Log("Player is dead!");
        isDead = true;

        // Disable colliders to prevent further interactions
        foreach (Collider col in GetComponents<Collider>())
            col.enabled = false;

        // If you have a Rigidbody, make it kinematic to stop physics interactions
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
            rb.isKinematic = true;

        // Stop time if the game is paused
        if (inventoryOpen) // If inventory was open, time was scaled to 0, reset it
        {
            Time.timeScale = 1f;
            inventoryOpen = false; // Close inventory visually too
            if (inventoryPanel != null)
                inventoryPanel.SetActive(false);
        }

        // Show game over UI
        if (GameOverUI != null)
            GameOverUI.SetActive(true);

        // Optional: Disable player movement/input scripts here
        // For example: GetComponent<PlayerMovement>().enabled = false;
    }

    public void AddItemToInventory(ItemData newItem)
    {
        foreach (InventorySlot slot in inventorySlots)
        {
            if (!slot.icon.enabled) // Assuming disabled icon means empty slot
            {
                slot.SetItem(newItem);
                Debug.Log("Added " + newItem.itemName + " to inventory.");
                return;
            }
        }
        Debug.Log("Inventory is full!");
    }
}