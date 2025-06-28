using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public Animator animator;
    public float attackRange = 2f;
    public int AttackDamage = 10;
    public float AttackCooldown = 1f;
    public float MaxHealth = 100f;

    public GameObject deathEffectPrefab;

    private float currentHealth;
    private float lastAttackTime = 0f;
    private Transform targetPlayer;
    private NavMeshAgent agent;
    private EnemySpawner spawner;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        currentHealth = MaxHealth;
    }

    void Update()
    {
        FindNearestPlayer();
        HandleMovementAndAttack();
    }

    public void SetSpawner(EnemySpawner s)
    {
        spawner = s;
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    void Die()
    {
        animator.SetTrigger("die");

        if (deathEffectPrefab)
            Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);

        if (spawner != null)
        {
            spawner.RespawnAfterDelay(5f); // KHÔNG truyền position nữa
        }

        Destroy(gameObject);
    }

    void FindNearestPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        float closestDistance = Mathf.Infinity;
        Transform closestPlayer = null;

        foreach (GameObject p in players)
        {
            float dist = Vector3.Distance(transform.position, p.transform.position);
            if (dist < closestDistance)
            {
                closestDistance = dist;
                closestPlayer = p.transform;
            }
        }

        targetPlayer = closestPlayer;
    }

    void HandleMovementAndAttack()
    {
        if (targetPlayer == null)
        {
            animator.SetBool("isMoving", false);
            animator.SetBool("isAttacking", false);
            agent.isStopped = true;
            return;
        }

        float distance = Vector3.Distance(transform.position, targetPlayer.position);

        Vector3 directionToPlayer = (targetPlayer.position - transform.position).normalized;
        directionToPlayer.y = 0f;

        if (directionToPlayer != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
            transform.rotation = targetRotation * Quaternion.Euler(0f, 180f, 0f);
        }

        if (distance <= attackRange)
        {
            agent.isStopped = true;
            animator.SetBool("isMoving", false);
            animator.SetBool("isAttacking", true);
        }
        else
        {
            agent.isStopped = false;
            agent.SetDestination(targetPlayer.position);
            animator.SetBool("isAttacking", false);
            animator.SetBool("isMoving", true);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && Time.time >= lastAttackTime + AttackCooldown)
        {
            PlayerVitural player = collision.gameObject.GetComponent<PlayerVitural>();
            if (player != null)
            {
                player.TakeDamage(AttackDamage);
                lastAttackTime = Time.time;
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && Time.time >= lastAttackTime + AttackCooldown)
        {
            PlayerVitural player = other.GetComponent<PlayerVitural>();
            if (player != null)
            {
                player.TakeDamage(AttackDamage);
                lastAttackTime = Time.time;
            }
        }
    }
}

