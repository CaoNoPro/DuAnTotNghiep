using UnityEngine;
using UnityEngine.UI;

public class PlayerVirtual : MonoBehaviour
{
    public Slider HealthSlider;
    public int maxHealth;
    public float healFallRate; // Changed to float for Time.deltaTime division

    public Slider ThirstSlider;
    public int maxThirst;
    public float thirstFallRate; // Changed to float

    public Slider HungerSlider;
    public int maxHunger;
    public float hungerFallRate; // Changed to float

    public Slider StaminaSlider;
    public int maxStamina;
    public float staminaFallRatePerSecond = 10f; // Rate at which stamina falls per second when sprinting
    public float staminaRegainRatePerSecond = 5f; // Rate at which stamina regains per second when not sprinting

    public bool isDead = false;
    public GameObject GameOverUI;

    private CharacterController characterController; // You declared this but didn't initialize or use it. If you have a CharacterController for movement, you'll need to link it and modify its speed.

    private void Start()
    {
        HealthSlider.maxValue = maxHealth;
        HealthSlider.value = maxHealth;

        HungerSlider.maxValue = maxHunger;
        HungerSlider.value = maxHunger;

        ThirstSlider.maxValue = maxThirst;
        ThirstSlider.value = maxThirst;

        StaminaSlider.maxValue = maxStamina;
        StaminaSlider.value = maxStamina;

        if (GameOverUI != null)
        {
            GameOverUI.SetActive(false);
        }

        // It's good practice to get the CharacterController component if you're going to use it for player movement.
        // characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        if (isDead) return;

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
            // Health slowly regenerates if hunger and thirst are not critical
            HealthSlider.value += Time.deltaTime / healFallRate;
            // Clamp health to maxHealth
            HealthSlider.value = Mathf.Min(HealthSlider.value, maxHealth);
        }

        if (HealthSlider.value <= 0)
        {
            CharacterDead();
        }

        // --- Hunger Controller ---
        if (HungerSlider.value > 0)
        {
            HungerSlider.value -= Time.deltaTime / hungerFallRate;
        }
        else
        {
            HungerSlider.value = 0; // Ensure it doesn't go below zero
        }
        // Clamp hunger to maxHunger (useful if you have mechanics to increase hunger)
        HungerSlider.value = Mathf.Clamp(HungerSlider.value, 0, maxHunger);


        // --- Thirst Controller ---
        if (ThirstSlider.value > 0)
        {
            ThirstSlider.value -= Time.deltaTime / thirstFallRate;
        }
        else
        {
            ThirstSlider.value = 0; // Ensure it doesn't go below zero
        }
        // Clamp thirst to maxThirst
        ThirstSlider.value = Mathf.Clamp(ThirstSlider.value, 0, maxThirst);

        // --- Stamina Controller for Sprinting ---
        HandleStamina();
    }

    // --- Stamina Handling ---
    private void HandleStamina()
    {
        // Check if the Shift key is held down and if there's stamina to sprint
        bool isSprinting = Input.GetKey(KeyCode.LeftShift) && StaminaSlider.value > 0;

        if (isSprinting)
        {
            // Decrease stamina when sprinting
            StaminaSlider.value -= staminaFallRatePerSecond * Time.deltaTime;
            // You'll need to link this to your actual player movement script
            // For example, if you have a `PlayerMovement` script, you would call a method like:
            // PlayerMovement.instance.SetSprintState(true);
        }
        else
        {
            // Regenerate stamina when not sprinting and not at max stamina
            if (StaminaSlider.value < maxStamina)
            {
                StaminaSlider.value += staminaRegainRatePerSecond * Time.deltaTime;
            }
            // You'll need to link this to your actual player movement script
            // PlayerMovement.instance.SetSprintState(false);
        }

        // Clamp stamina to ensure it stays within min (0) and max bounds
        StaminaSlider.value = Mathf.Clamp(StaminaSlider.value, 0, maxStamina);

        // Debugging to see stamina values
        // Debug.Log("Current Stamina: " + StaminaSlider.value);
    }

    // --- Lost Health By Enemy ---
    public void TakeDamage(int DamageAmount)
    {
        HealthSlider.value -= DamageAmount;
        Debug.Log("Player took damage: " + DamageAmount + ", Current Health: " + HealthSlider.value);
        if (HealthSlider.value <= 0 && !isDead)
        {
            CharacterDead();
        }
    }

    public void CharacterDead()
    {
        Debug.Log("Player is dead!");
        isDead = true;
        Collider[] colliders = GetComponents<Collider>();
        foreach (Collider collider in colliders)
        {
            collider.enabled = false; // Disable all colliders
        }
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true; // Make the Rigidbody kinematic to stop physics interactions
        }
        if (GameOverUI != null)
        {
            GameOverUI.SetActive(true); // Show the Game Over UI
        }

        // Optionally, stop all movement and input handling here if the player dies
        // For example, if you have a PlayerInput script, you might disable it.
        // GetComponent<PlayerInput>().enabled = false;
    }
}