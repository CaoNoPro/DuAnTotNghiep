using NUnit.Framework.Interfaces;
using Unity.VisualScripting;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int DamageAmount = 10;

    public float lifeTime = 3;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }
    void OnTriggerEnter(Collider other)
    {
        EnemyHealth enemyHealth = other.GetComponent<EnemyHealth>();

        if (enemyHealth != null)
        {
            enemyHealth.TakeDamage(DamageAmount);
        }
        Destroy(gameObject);
    }
    void OnCollisionEnter(Collision collision)
    {
        EnemyHealth enemyHealth = collision.gameObject.GetComponent<EnemyHealth>();
        if (enemyHealth != null)
        {
            enemyHealth.TakeDamage(DamageAmount);
        }
        Destroy(gameObject);
    }
}
