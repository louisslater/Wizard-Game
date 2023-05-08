using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public NavMeshAgent agent;

    public LayerMask whatIsGround, whatIsTarget;

    public string targetTag = "Player";

    public List<GameObject> targetList;

    public GameObject target;

    //Patrolling
    public Vector3 walkPoint;
    public bool walkPointSet;
    public float walkPointRange;
    public bool isResting;
    public float waitTimeMin;
    public float waitTimeMax;

    //Attacking
    public float timeBetweenAttacks;
    bool alreadyAttacked;

    //States
    public float sightRange, attackRange;
    public bool targetInSightRange, targetInAttackRange;

    public float speedSmoothTime = 0.1f;

    //Animation
    Animator animator;
    private float animationSpeedPercent;


    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();


    }

    Transform GetClosestTarget(Transform[] targets)
    {
        Transform bestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = transform.position;
        foreach (Transform potentialTarget in targets)
        {
            Vector3 directionToTarget = potentialTarget.position - currentPosition;
            float dSqrToTarget = directionToTarget.sqrMagnitude;
            if (dSqrToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToTarget;
                bestTarget = potentialTarget;
            }
        }

        return bestTarget;
    }

    private void Update()
    {

        target = GameObject.Find("player");

        //Check for sight and attack range
        targetInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsTarget);
        targetInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsTarget);

        float speed = agent.velocity.magnitude;

        animationSpeedPercent = (speed < 0.01?0:1);
        animator.SetFloat("SpeedPercent", animationSpeedPercent, speedSmoothTime, Time.deltaTime);


        if (!targetInSightRange && !targetInAttackRange) Patrolling();
        if (targetInSightRange && !targetInAttackRange) ChasePlayer();
        if (targetInSightRange && targetInAttackRange) AttackPlayer();
    }

    private void Patrolling()
    {
        if (!walkPointSet)
        {
            SearchWalkpoint();
        }

        if (walkPointSet)
        {
            agent.SetDestination(walkPoint);
        }

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        //Walkpoint reached
        if (distanceToWalkPoint.magnitude < 1f)
        {
            walkPointSet = false;
            StartCoroutine(WaitForSeconds());
        }
    }

    IEnumerator WaitForSeconds()
    {
        agent.isStopped = true;
        yield return new WaitForSeconds(Random.Range(waitTimeMin,waitTimeMax+1));
        agent.isStopped = false;
    }

    private void SearchWalkpoint()
    {
        //Calculate random point in range
        float randomX = Random.Range(-walkPointRange, walkPointRange+1);
        float randomZ = Random.Range(-walkPointRange, walkPointRange+1);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if(Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
        {
            walkPointSet = true;
        }
    }
    private void ChasePlayer()
    {
        StopCoroutine(WaitForSeconds());
        agent.isStopped = false;
        agent.SetDestination(target.transform.position);
    }
    private void AttackPlayer()
    {
        //Ensure enemy doesn't move
        agent.SetDestination(transform.position);

        transform.LookAt(target.transform);

        if (!alreadyAttacked)
        {
            //

            alreadyAttacked = true;
            StartCoroutine(ResetAttack());
        }
    }

    IEnumerator ResetAttack()
    {
        yield return new WaitForSeconds(2f);
        alreadyAttacked = false;
    }

}
