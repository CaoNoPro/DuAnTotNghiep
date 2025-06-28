using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class BossController : MonoBehaviour
{
    public Animator animator;
    public GameObject jumpExplosionVFX;
    public GameObject rageVFX;
    public Transform vfxSpawnPoint;

    public float attackRange = 2f;
    public float moveSpeed = 3.5f;

    public int attackDamage = 10;              // 👈 Gây sát thương
    public float attackCooldown = 2f;          // 👈 Thời gian hồi chiêu
    private float lastAttackTime;              // 👈 Theo dõi lần tấn công cuối

    private Transform targetPlayer;
    private NavMeshAgent agent;

    private bool isJumping = false;
    private bool isRaging = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        StartCoroutine(JumpRoutine());
        StartCoroutine(RageRoutine());
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
        if (targetPlayer == null || isJumping || isRaging)
        {
            animator.SetBool("isMoving", false);
            agent.isStopped = true;
            return;
        }

        float distance = Vector3.Distance(transform.position, targetPlayer.position);

        // Look at player
        Vector3 lookDirection = (targetPlayer.position - transform.position).normalized;
        lookDirection.y = 0;
        if (lookDirection != Vector3.zero)
            transform.forward = lookDirection;

        if (distance <= attackRange)
        {
            animator.SetBool("isMoving", false);
            agent.isStopped = true;
            animator.SetBool("isAttacking", true);

            // Gây sát thương nếu đến lúc được phép tấn công
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                lastAttackTime = Time.time;

                PlayerVitural player = targetPlayer.GetComponent<PlayerVitural>();
                if (player != null && !player.isdead)
                {
                    player.TakeDamage(attackDamage);
                }
            }
        }
        else
        {
            animator.SetBool("isAttacking", false);
            animator.SetBool("isMoving", true);
            agent.isStopped = false;
            agent.SetDestination(targetPlayer.position);
        }
    }

    IEnumerator JumpRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(10f);
            yield return StartCoroutine(DoJump());
        }
    }

    IEnumerator DoJump()
    {
        isJumping = true;
        agent.isStopped = true;
        animator.SetTrigger("doJump");

        yield return new WaitForSeconds(2f); // delay before VFX appears (after boss jumps)

        if (jumpExplosionVFX != null)
        {
            GameObject vfx = Instantiate(jumpExplosionVFX, vfxSpawnPoint.position, Quaternion.identity);
            Destroy(vfx, 2f);
        }

        yield return new WaitForSeconds(1.0f);
        isJumping = false;
        agent.isStopped = false;
    }

    IEnumerator RageRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(30f);
            yield return StartCoroutine(DoRage());
        }
    }

    IEnumerator DoRage()
    {
        isRaging = true;
        animator.SetBool("isRaging", true);
        agent.isStopped = true;

        if (rageVFX != null)
        {
            GameObject vfx = Instantiate(rageVFX, vfxSpawnPoint.position, Quaternion.identity);
            Destroy(vfx, 7f);
        }

        yield return new WaitForSeconds(3f);
        animator.SetBool("isRaging", false);
        isRaging = false;
        agent.isStopped = false;
    }
}

