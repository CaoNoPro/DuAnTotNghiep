// EnemyHealth.cs
using UnityEngine;
using UnityEngine.UI; // Make sure to include this for UI elements

public class EnemyHealth : MonoBehaviour
{
    public float health = 100f;

    public float currentHealth;
    

    [Header("UI References")]
    public Slider healthSlider; // Kéo Slider UI vào đây trong Inspector

    void Start()
    {
        currentHealth = health;

        // Initialize the health slider
        if (healthSlider != null)
        {
            healthSlider.maxValue = health;
            healthSlider.value = currentHealth;
        }
    }

    public void TakeDamage(float amount)
    {
        health -= amount;
        if (health <= 0f)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log(gameObject.name + " has died!");
        // Add death animations, particle effects, loot drops, etc.
        Destroy(gameObject); // Remove the enemy from the scene
    }
}