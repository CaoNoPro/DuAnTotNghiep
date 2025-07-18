using UnityEngine;
using UnityEngine.AI; // Cần thiết cho NavMeshAgent
using System.Collections; // Cần thiết cho Coroutine

public class PlayerAIController : MonoBehaviour
{
    public enum AIState { Patrol, Chase, Attack, PlayerDirected } // C�c tr?ng th�i c?a AI

    [Header("References")]
    public Transform playerTarget; // K�o th? Player GameObject v�o ?�y trong Inspector
    private UnityEngine.AI.NavMeshAgent agent;
    private EnemyShooting enemyShooting;
    private PlayerDirectedMovement playerDirectedMovement; // Tham chi?u ??n script di chuy?n theo ng??i ch?i

    [Header("AI Settings")]
    public AIState currentState = AIState.Patrol; // Tr?ng th�i AI ban ??u
    public float sightRange = 15f; // Ph?m vi nh�n th?y ng??i ch?i
    public float attackRange = 10f; // Ph?m vi t?n c�ng
    public float patrolSpeed = 3.5f;
    public float chaseSpeed = 5.0f;

    [Header("Patrol Settings")]
    public Vector3[] patrolPoints; // C�c ?i?m tu?n tra
    private int currentPatrolIndex;
    public float minPatrolWaitTime = 1f;
    public float maxPatrolWaitTime = 3f;

    [Header("Player Directed Settings")]
    public float playerDirectedSpeed = 6.0f; // T?c ?? khi AI ???c ?i?u khi?n b?i ng??i ch?i

    void Awake()
    {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        enemyShooting = GetComponent<EnemyShooting>();
        playerDirectedMovement = GetComponent<PlayerDirectedMovement>();

        // ??m b?o agent v� c�c script kh�c t?n t?i
        if (agent == null) Debug.LogError("NavMeshAgent not found on " + gameObject.name);
        if (enemyShooting == null) Debug.LogWarning("EnemyShooting not found on " + gameObject.name + ". AI will not shoot.");
        if (playerDirectedMovement == null) Debug.LogWarning("PlayerDirectedMovement not found on " + gameObject.name + ". AI will not be player-directed.");

        // N?u PlayerDirectedMovement c�, ban ??u v� hi?u h�a n�
        if (playerDirectedMovement != null)
        {
            playerDirectedMovement.enabled = false;
        }
    }

    void Start()
    {
        // B?t ??u Coroutine ?? qu?n l� tr?ng th�i
        StartCoroutine(AILogicRoutine());
    }

    IEnumerator AILogicRoutine()
    {
        while (true) // V�ng l?p v� h?n ?? qu?n l� tr?ng th�i
        {
            switch (currentState)
            {
                case AIState.Patrol:
                    yield return StartCoroutine(PatrolState());
                    break;
                case AIState.Chase:
                    yield return StartCoroutine(ChaseState());
                    break;
                case AIState.Attack:
                    yield return StartCoroutine(AttackState());
                    break;
                case AIState.PlayerDirected:
                    yield return StartCoroutine(PlayerDirectedState());
                    break;
            }
            yield return null; // Ch? m?t frame tr??c khi ki?m tra l?i tr?ng th�i
        }
    }

    IEnumerator PatrolState()
    {
        Debug.Log("AI State: Patrol");
        agent.speed = patrolSpeed;
        enemyShooting?.SetTarget(null); // Kh�ng c� m?c ti�u ?? b?n khi tu?n tra
        playerDirectedMovement.enabled = false; // ??m b?o ch? ?? ?i?u khi?n b? t?t

        if (patrolPoints.Length == 0)
        {
            Debug.LogWarning("No patrol points defined. AI will idle.");
            agent.isStopped = true;
            yield break;
        }

        agent.isStopped = false; // ??m b?o agent ?ang di chuy?n

        // ?i ??n ?i?m tu?n tra hi?n t?i
        if (agent.destination != patrolPoints[currentPatrolIndex])
        {
            agent.SetDestination(patrolPoints[currentPatrolIndex]);
        }

        while (currentState == AIState.Patrol)
        {
            // Ki?m tra xem ng??i ch?i c� ? trong t?m nh�n kh�ng
            float distanceToPlayer = Vector3.Distance(transform.position, playerTarget.position);
            if (distanceToPlayer <= sightRange)
            {
                currentState = AIState.Chase; // Chuy?n sang tr?ng th�i Chase
                break;
            }

            // N?u ?� ??n ?i?m tu?n tra
            if (!agent.pathPending && agent.remainingDistance < agent.stoppingDistance + 0.1f)
            {
                Debug.Log("Reached patrol point. Waiting...");
                agent.isStopped = true; // D?ng l?i t?i ?i?m tu?n tra
                yield return new WaitForSeconds(Random.Range(minPatrolWaitTime, maxPatrolWaitTime)); // Ch?

                // Chuy?n sang ?i?m tu?n tra ti?p theo
                currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
                agent.SetDestination(patrolPoints[currentPatrolIndex]);
                agent.isStopped = false; // B?t ??u di chuy?n l?i
            }
            yield return null; // Ch? m?t frame
        }
    }

    IEnumerator ChaseState()
    {
        Debug.Log("AI State: Chase");
        agent.speed = chaseSpeed;
        enemyShooting?.SetTarget(null); // Ch? b?n khi ? tr?ng th�i Attack
        playerDirectedMovement.enabled = false; // ??m b?o ch? ?? ?i?u khi?n b? t?t

        agent.isStopped = false; // ??m b?o agent ?ang di chuy?n

        while (currentState == AIState.Chase)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, playerTarget.position);

            if (distanceToPlayer <= attackRange)
            {
                currentState = AIState.Attack; // Chuy?n sang tr?ng th�i Attack
                break;
            }
            else if (distanceToPlayer > sightRange + 5f) // Ng??i ch?i ?� ra kh?i t?m nh�n m?t kho?ng
            {
                currentState = AIState.Patrol; // Quay l?i tu?n tra
                break;
            }

            // Di chuy?n theo ng??i ch?i
            agent.SetDestination(playerTarget.position);
            yield return null; // Ch? m?t frame
        }
    }

    IEnumerator AttackState()
    {
        Debug.Log("AI State: Attack");
        agent.isStopped = true; // D?ng l?i ?? t?n c�ng
        enemyShooting?.SetTarget(playerTarget); // G�n m?c ti�u ?? b?n
        playerDirectedMovement.enabled = false; // ??m b?o ch? ?? ?i?u khi?n b? t?t


        while (currentState == AIState.Attack)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, playerTarget.position);

            if (distanceToPlayer > attackRange)
            {
                currentState = AIState.Chase; // Ng??i ch?i ra kh?i t?m t?n c�ng, chuy?n sang Chase
                break;
            }
            // Quay m?t v? ph�a ng??i ch?i
            Vector3 lookAtPlayer = playerTarget.position;
            lookAtPlayer.y = transform.position.y; // Gi? nguy�n tr?c Y c?a AI
            transform.LookAt(lookAtPlayer);

            // EnemyShooting script s? t? x? l� vi?c b?n khi c� target
            yield return null; // Ch? m?t frame
        }
    }

    IEnumerator PlayerDirectedState()
    {
        Debug.Log("AI State: Player Directed");
        agent.isStopped = true; // V� hi?u h�a NavMeshAgent
        agent.velocity = Vector3.zero; // ??m b?o kh�ng c�n v?n t?c t? NavMeshAgent
        enemyShooting?.SetTarget(null); // Kh�ng t?n c�ng khi ?ang ???c ?i?u khi?n

        playerDirectedMovement.enabled = true; // B?t script di chuy?n theo ng??i ch?i
        playerDirectedMovement.moveSpeed = playerDirectedSpeed; // C?p nh?t t?c ?? di chuy?n

        while (currentState == AIState.PlayerDirected)
        {
            // Logic player-directed s? ???c x? l� b?i PlayerDirectedMovement
            yield return null; // Ch? m?t frame
        }

        // Khi tho�t tr?ng th�i PlayerDirected, v� hi?u h�a PlayerDirectedMovement
        playerDirectedMovement.enabled = false;
        agent.isStopped = false; // K�ch ho?t l?i NavMeshAgent n?u c?n
    }

    // H�m public ?? ng??i ch?i c� th? chuy?n tr?ng th�i AI (v� d?: nh?n n�t)
    public void SetAIState(AIState newState)
    {
        if (currentState == newState) return; // Kh�ng l�m g� n?u tr?ng th�i kh�ng thay ??i

        currentState = newState;
        Debug.Log("AI State changed to: " + newState);

        // C� th? th�m logic reset ho?c transition ? ?�y n?u c?n
        if (newState != AIState.PlayerDirected && playerDirectedMovement != null)
        {
            playerDirectedMovement.enabled = false;
            //agent.isStopped = false; // ??m b?o NavMeshAgent ???c k�ch ho?t l?i
            //agent.SetDestination(transform.position); // Reset destination
        }
        else if (newState == AIState.PlayerDirected && playerDirectedMovement != null)
        {
            playerDirectedMovement.enabled = true;
            agent.isStopped = true; // T?t NavMeshAgent khi AI ???c ?i?u khi?n
        }
    }

    // V? t?m nh�n v� t?m t?n c�ng trong Scene view ?? d? debug
    void OnDrawGizmosSelected()
    {
        if (playerTarget != null)
        {
            // Sight Range
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, sightRange);

            // Attack Range
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);
        }

        // Patrol Points
        if (patrolPoints != null && patrolPoints.Length > 0)
        {
            Gizmos.color = Color.blue;
            for (int i = 0; i < patrolPoints.Length; i++)
            {
                Gizmos.DrawSphere(patrolPoints[i], 0.5f); // Draw a sphere at each point
                if (i < patrolPoints.Length - 1)
                {
                    Gizmos.DrawLine(patrolPoints[i], patrolPoints[i + 1]); // Draw line between points
                }
            }
            if (patrolPoints.Length > 1)
            {
                Gizmos.DrawLine(patrolPoints[patrolPoints.Length - 1], patrolPoints[0]); // Loop back to start
            }
        }
    }
}
