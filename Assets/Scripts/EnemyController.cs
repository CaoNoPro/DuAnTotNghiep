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
    // private int damageToInflict = 1; // Bạn đã định nghĩa AttackDamage, nên có thể dùng nó thay vì damageToInflict này.

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        // Khởi tạo lastPosition cho PlayerState nếu nó được dùng trong Start()
        // lastPosition = playerBody.transform.position; // Đây là cho PlayerState, không phải EnemyController
    }

    void Update()
    {
        FindNearestPlayer();
        HandleMovementAndAttack();
    }

    void FindNearestPlayer()
    {
        // Có thể tối ưu hóa việc tìm kiếm player bằng cách dùng tags hoặc Singleton cho PlayerState
        // Nhưng với số lượng player ít thì cách này vẫn ổn.
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
            if (agent.enabled) // Đảm bảo agent đã được bật trước khi tương tác
            {
                agent.isStopped = true;
            }
            return;
        }

        float distance = Vector3.Distance(transform.position, targetPlayer.position);

        // Xoay mặt về phía player
        Vector3 directionToPlayer = (targetPlayer.position - transform.position).normalized;
        directionToPlayer.y = 0f;

        if (directionToPlayer != Vector3.zero)
        {
            // Làm mịn việc xoay để không quá giật
            Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
            // Có thể dùng Slerp để xoay mượt hơn:
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation * Quaternion.Euler(0f, 180f, 0f), Time.deltaTime * 5f); // Tùy chỉnh tốc độ xoay (5f)
        }

        if (distance <= attackRange)
        {
            if (agent.enabled)
            {
                agent.isStopped = true;
            }
            animator.SetBool("isMoving", false);
            animator.SetBool("isAttacking", true);

            // Kiểm tra thời gian hồi chiêu trước khi tấn công
            if (Time.time >= lastAttackTime + AttackCooldown)
            {
                Attack(); // Gọi hàm tấn công
                lastAttackTime = Time.time; // Cập nhật thời gian tấn công cuối cùng
            }
        }
        else
        {
            if (agent.enabled)
            {
                agent.isStopped = false;
                agent.SetDestination(targetPlayer.position);
            }
            animator.SetBool("isAttacking", false);
            animator.SetBool("isMoving", true);
        }
    }

    private void Attack()
    {
        // Kiểm tra xem PlayerState.Instance có tồn tại không
        if (PlayerState.Instance != null)
        {
            PlayerState.Instance.TakeDamage(AttackDamage); // Gây sát thương bằng AttackDamage đã định nghĩa
            Debug.Log($"Enemy attacked! Player took {AttackDamage} damage.");
        }
        else
        {
            Debug.LogWarning("PlayerState.Instance is null. Cannot inflict damage.");
        }
    }
}