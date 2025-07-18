// EnemyHealth.cs
using System;
using UnityEngine;
using UnityEngine.UI; // Make sure to include this for UI elements

public class EnemyHealth : MonoBehaviour
{
    public float maxHealth = 100;
    private float currentHealth;

    [Header("UI References")]
    public Slider healthSlider; // Kéo Slider UI vào đây trong Inspector

    void Start()
    {
        currentHealth = maxHealth;

        // Initialize the health slider
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
    }

    public void TakeDamage(float damageAmount)
    {
        currentHealth -= damageAmount;
        Debug.Log(gameObject.name + " took " + damageAmount + " damage. Current Health: " + currentHealth);

        // Update the health slider whenever damage is taken
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }

        if (currentHealth <= 0)
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