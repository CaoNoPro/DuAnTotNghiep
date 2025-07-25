using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SpiderMoveState : StateMachineBehaviour
{
    float timer;
    public float MovingTime = 10f;

    Transform player;
    NavMeshAgent agent;

    public float detectionAreaRadius = 18f;
    public float MovingSpeed = 2f;

    List<Transform> wayPointList = new List<Transform>();
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //--Initialization--//
        player = GameObject.FindGameObjectWithTag("Player").transform;
        agent = animator.GetComponent<NavMeshAgent>();

        agent.speed = MovingSpeed;
        timer = 0;

        //--Get All Way Point and Move To The First Way Point--//
        GameObject wayPointCluster = animator.GetComponent<NpcWaypoint>().npcWayPoint;
        foreach (Transform t in wayPointCluster.transform)
        {
            wayPointList.Add(t);
        }
        Vector3 firstPosition = wayPointList[Random.Range(0, wayPointList.Count)].position;
        agent.SetDestination(firstPosition);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //--If agent arrived at waypoint, move to next waypoint --//
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            agent.SetDestination(wayPointList[Random.Range(0, wayPointList.Count)].position);
        }
        //--Transition to Idle State--//
        timer += Time.deltaTime;
        if (timer > MovingTime)
        {
            animator.SetBool("isMoving", false);
        }
        //--Transition to Chase State--//
        float distanceFromPlayer = Vector3.Distance(player.position, animator.transform.position);
        if (distanceFromPlayer < detectionAreaRadius)
        {
            animator.SetBool("isChasing", true);
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        agent.SetDestination(agent.transform.position);
    }
}
