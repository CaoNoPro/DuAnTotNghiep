using UnityEngine;
using UnityEngine.UI;

public class PlayerVirtual : MonoBehaviour
{

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

    public bool isDead = false;
    public GameObject GameOverUI;

    // private CharacterController characterController; // Bạn chưa sử dụng biến này, có thể bỏ qua hoặc khởi tạo nếu dùng

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

        Debug.Log("PlayerVirtual: Game started. Initial Health: " + HealthSlider.value);
    }

    private void Update()
    {
        if (isDead)
        {
            // Debug.Log("PlayerVirtual: Player is dead, Update loop skipped."); // Có thể bật nếu nghi ngờ vòng lặp Update bị bỏ qua khi player chết
            return;
        }

        // --- Health Regeneration/Degeneration ---
        if (HungerSlider.value <= 0 && ThirstSlider.value <= 0)
        {
            HealthSlider.value -= Time.deltaTime / healFallRate * 2;
            // Debug.Log("Health decreasing (severe): " + HealthSlider.value);
        }
        else if (HungerSlider.value <= 0 || ThirstSlider.value <= 0)
        {
            HealthSlider.value -= Time.deltaTime / healFallRate;
            // Debug.Log("Health decreasing (moderate): " + HealthSlider.value);
        }
        else
        {
            // Health slowly regenerates if hunger and thirst are not critical
            HealthSlider.value += Time.deltaTime / healFallRate;
            HealthSlider.value = Mathf.Min(HealthSlider.value, maxHealth); // Clamp health to maxHealth
            // Debug.Log("Health regenerating: " + HealthSlider.value);
        }

        // QUAN TRỌNG: Kiểm tra máu để kích hoạt hàm chết
        if (HealthSlider.value <= 0)
        {
            Debug.Log("PlayerVirtual: Health <= 0. Attempting to call CharacterDead(). Current Health: " + HealthSlider.value);
            CharacterDead();
        }
        else
        {
            // Chỉ log nếu không chết để tránh spam console khi máu đã 0
            // Debug.Log("PlayerVirtual: Current Health (Update): " + HealthSlider.value);
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

        // --- Stamina Controller for Sprinting ---
        HandleStamina();
    }

    private void HandleStamina()
    {
        bool isSprinting = Input.GetKey(KeyCode.LeftShift) && StaminaSlider.value > 0;

        if (isSprinting)
        {
            StaminaSlider.value -= staminaFallRatePerSecond * Time.deltaTime;
        }
        else
        {
            if (StaminaSlider.value < maxStamina)
            {
                StaminaSlider.value += staminaRegainRatePerSecond * Time.deltaTime;
            }
        }
        StaminaSlider.value = Mathf.Clamp(StaminaSlider.value, 0, maxStamina);
    }

    public void TakeDamage(int DamageAmount)
    {
        HealthSlider.value -= DamageAmount;
        Debug.Log("PlayerVirtual: Player took damage: " + DamageAmount + ", Current Health: " + HealthSlider.value);
        if (HealthSlider.value <= 0 && !isDead)
        {
            Debug.Log("PlayerVirtual: Health <= 0 from TakeDamage. Attempting to call CharacterDead().");
            CharacterDead();
        }
    }

    public void CharacterDead()
    {
        if (isDead) return; // Tránh gọi nhiều lần nếu đã chết

        Debug.Log("PlayerVirtual: CharacterDead() called. Player is now dead!");
        isDead = true;
        
        // Vô hiệu hóa collider để không tương tác vật lý nữa
        Collider[] colliders = GetComponents<Collider>();
        foreach (Collider collider in colliders)
        {
            collider.enabled = false;
        }
        
        // Vô hiệu hóa Rigidbody (nếu có) để dừng di chuyển vật lý
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true; // Dừng mọi tương tác vật lý
            // Tùy chọn: rb.velocity = Vector3.zero; để dừng chuyển động ngay lập tức
        }

        // Tùy chọn: Vô hiệu hóa các script liên quan đến di chuyển, input
        // Ví dụ: GetComponent<PlayerMovement>().enabled = false;
        // GetComponent<PlayerInput>().enabled = false;
        this.enabled = false; // Tắt chính script PlayerVirtual để Update() không chạy nữa

        if (GameOverUI != null)
        {
            GameOverUI.SetActive(true); // Hiển thị bảng Game Over
            Debug.Log("PlayerVirtual: GameOverUI set to active.");
        }
        else
        {
            Debug.LogError("PlayerVirtual: GameOverUI is NULL! Please assign it in the Inspector.");
        }

        // Tùy chọn: Dừng thời gian game
        // Time.timeScale = 0f;
    }
}