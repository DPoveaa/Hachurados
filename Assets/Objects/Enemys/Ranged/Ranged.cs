using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;

public class Ranged : MonoBehaviour
{

    #region Projectile Audio
    public AudioSource shootingSFX;
    #endregion

    #region Damage

    private float health = 2;

    #endregion

    public NavMeshAgent agent;

    public Transform player;
    public Transform arrowSpawn;
    private Transform arrowRotation;

    public LayerMask whatIsGround, whatIsPlayer;

    public Animator animations;

    //Patroling
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    //Attacking
    public float timeBetweenAttacks;
    bool alreadyAttacked;
    public GameObject projectile;
    public float rotationSpeed = 5f;
    public float attackDelay;
    public float attackCountdown;

    //States
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    private void Awake()
    {
        player = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if (!GameManager.isPaused)
        {
            //Check for sight and attack range
            playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
            playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

            if (!playerInSightRange && !playerInAttackRange) Patroling();
            if (playerInSightRange && !playerInAttackRange) ChasePlayer();
            if (playerInAttackRange && playerInSightRange) AttackPlayer();

            if (attackCountdown > 0)
            {
                attackCountdown -= Time.deltaTime;
            }
            else if (attackCountdown <= 0)
            {
                alreadyAttacked = false;
            }

            if (!playerInAttackRange && !playerInSightRange)
            {
                animations.SetBool("Walking", false);
                animations.SetBool("InRange", false);
            }
            else if (!playerInAttackRange)
            {
                animations.SetBool("InRange", true);
                animations.SetBool("Walking", true);
            }
            else
            {
                animations.SetBool("Walking", false);
                animations.SetBool("InRange", false);
            }
        }
    }
    private void Patroling()
    {
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet)
            agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        //Walkpoint reached
        if (distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;
    }
    private void SearchWalkPoint()
    {
        //Calculate random point in range
        float randomZ = UnityEngine.Random.Range(-walkPointRange, walkPointRange);
        float randomX = UnityEngine.Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
            walkPointSet = true;
    }

    private void ChasePlayer()
    {
        agent.SetDestination(player.position);
    }

    private void AttackPlayer()
    {
        agent.SetDestination(transform.position);

        arrowSpawn.LookAt(player);

        Vector3 direction = (player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);

        if (!alreadyAttacked)
        {
            StartCoroutine(Attack());
        }
    }

    private IEnumerator Attack()
    {
        animations.SetBool("Shoot", true);
        alreadyAttacked = true;
        attackCountdown = attackDelay;
        yield return new WaitForSeconds(0.6f);
        Rigidbody rb = Instantiate(projectile, arrowSpawn.position, Quaternion.identity).GetComponent<Rigidbody>();
        rb.transform.rotation = Quaternion.LookRotation(arrowSpawn.forward, arrowSpawn.up) * Quaternion.Euler(-90f, 0f, 0f);
        rb.AddForce(arrowSpawn.forward * 32f, ForceMode.Impulse);
        rb.AddForce(arrowSpawn.up * 8f, ForceMode.Impulse);
        shootingSFX.Play();
        yield return new WaitForSeconds(0.1f);
        animations.SetBool("Shoot", false);
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
}
