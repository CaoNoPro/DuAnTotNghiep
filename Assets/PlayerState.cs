using UnityEngine;

public class PlayerState : MonoBehaviour
{
    public static PlayerState Instance { get; set; }
    

    // Health
    public float currentHealth;
    public float maxHealth;

    //Thirst
    public float currentThirst;
    public float maxThirst;
    //Hunger
    public float currentHunger;
    public float maxHunger;


    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }


    void Start()
    {
        currentHealth = maxHealth;
        currentThirst = maxThirst;
        currentHunger = maxHunger;
    }



    void Update()
    {
        
    }
}
