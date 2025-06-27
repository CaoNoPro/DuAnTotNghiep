using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public Animator animator;
    public float attackRange = 2f;

    private Transform targetPlayer;
    private NavMeshAgent agent;

    public int AttackDamage = 10; // Sát thương khi tấn công
    public float AttackCooldown = 1f; // Thời gian hồi chiêu giữa các đòn tấn công
    private float lastAttackTime = 0f;


    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        FindNearestPlayer();
        HandleMovementAndAttack();
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

        // Xoay m?t v? phía player
        Vector3 directionToPlayer = (targetPlayer.position - transform.position).normalized;
        directionToPlayer.y = 0f;

        if (directionToPlayer != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
            transform.rotation = targetRotation * Quaternion.Euler(0f, 180f, 0f); // Xoay n?u model quay ng??c
        }

        if (distance <= attackRange)
        {
            agent.isStopped = true;
            animator.SetBool("isMoving", false);
            animator.SetBool("isAttacking", true);
            // TODO: thêm logic gây sát th??ng n?u mu?n
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
        if (collision.gameObject.CompareTag("Player"))
        {
            if (Time.time >= lastAttackTime + AttackCooldown)
            {
                PlayerVitural player = collision.gameObject.GetComponent<PlayerVitural>();
                if (player != null)
                {
                    player.TakeDamage(AttackDamage); // Giảm máu của người chơi
                    lastAttackTime = Time.time; // Cập nhật thời gian tấn công cuối cùng
                }
                else
                {
                    Debug.LogWarning("PlayerVitural component not found on the player object.");
                }

            }
        }else
        {
            Debug.Log("Collision with non-player object: " + collision.gameObject.tag);
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerVitural player = other.GetComponent<PlayerVitural>();
            if (player != null && Time.time >= lastAttackTime + AttackCooldown)
            {
                player.TakeDamage(AttackDamage);
                lastAttackTime = Time.time;
            }
        }
    }
} 
