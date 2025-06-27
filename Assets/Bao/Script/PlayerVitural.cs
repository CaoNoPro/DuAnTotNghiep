using UnityEngine;
using UnityEngine.UI;

public class PlayerVitural : MonoBehaviour
{
    public Slider HealthSlider;
    public int maxHealth;
    public int healFallRate;

    public Slider ThirstSlider;
    public int maxThirst;
    public int thirstFallRate;

    public Slider HungerSlider;
    public int maxHunger;
    public int hungerFallRate;

    public Slider StaminaSlider;
    public int maxStamina;
    private int staminaFallRate;
    public int staminaFallMult;
    private int StaminaRegainRate;
    public int StaminaRegainMult;

    private CharacterController characterController;


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

        staminaFallRate = 1; // Set the rate at which stamina falls
        StaminaRegainRate = 1; // Set the rate at which stamina regains
    }
    private void Update()
    {
        if(HungerSlider.value <= 0 && (ThirstSlider.value <= 0))
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
        }
        
        if (HealthSlider.value <= 0)
        {
            CharacterDead();
        }

        //HungerController
        if (HungerSlider.value >= 0)
        {
            HungerSlider.value -= Time.deltaTime / hungerFallRate;
        }

        else if(HungerSlider.value <= 0)
        {
            HungerSlider.value = 0;
        }

        else if(HungerSlider.value >= maxHunger)
        {
            HungerSlider.value = maxHunger;
        }

        //ThirstController
        if (ThirstSlider.value >= 0)
        {
            ThirstSlider.value -= Time.deltaTime / thirstFallRate;
        }
        else if (ThirstSlider.value <= 0)
        {
            ThirstSlider.value = 0;
        }
        else if (ThirstSlider.value >= maxThirst)
        {
            ThirstSlider.value = maxThirst;
        }

        
    }
    public void CharacterDead()
    {
        //
    }
}
