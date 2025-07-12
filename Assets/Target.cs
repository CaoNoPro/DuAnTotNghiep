using UnityEngine;

public class Target : MonoBehaviour
{
    public float health = 50f;

    public void TakeDamage(float amount)
    {
        health -= amount;
        if (health <= 0f)
        {
            Die();
        }
        Debug.Log("Target " + gameObject.name + " took " + amount + " damage. Health now: " + health);
    }
    void Die()
    {
        Destroy(gameObject);
        Debug.Log("Target " + gameObject.name + " died.");
    }
}
