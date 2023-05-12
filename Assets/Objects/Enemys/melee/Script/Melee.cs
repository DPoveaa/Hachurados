using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Melee : MonoBehaviour
{
    public NavMeshAgent agent;

    public Transform player;
    public Player playerScript;

    public LayerMask whatIsGround, whatIsPlayer;

    #region Damage

    private float health = 3;
    public float blinkDuration = 0.2f;
    public Material material;
    private Material oldColor;

    #endregion

    #region Attack
    private Collider attackCheck;
    #endregion

    //Patroling
    public Vector3 walkPoint;
    public bool walkPointSet;
    private float walkPointRange;

    //Attacking
    public float attackDelay;
    public float attackCountdown;
    private bool alreadyAttacked;

    //States
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    private void Start()
    {
        oldColor.color = material.color;
    }

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        attackCheck = GetComponent<BoxCollider>();
        playerScript = FindObjectOfType<Player>();
    }

    private void Update()
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
        } else if (attackCountdown <= 0)
        {
            alreadyAttacked = false;
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
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
            walkPointSet = true;
    }

    private void ChasePlayer()
    {
        agent.SetDestination(player.position);
    }

    public void OnCollisionEnter(Collision collision)
    {

    }

    public void OnTriggerEnter(Collider other)
    {
        #region Trigger From Melee Enemy
        if (attackCheck)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                playerScript.DamageTaken(10, null, other, 40, 5);
            }
        }

        #endregion
    }

    private void AttackPlayer()
    {
        //Make sure enemy doesn't move
        agent.SetDestination(transform.position);

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
        attackCheck.enabled = true;
        yield return new WaitForSeconds(0.1f);
        attackCheck.enabled = false;
        alreadyAttacked = true;
        attackCountdown = attackDelay;
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Destroy(gameObject);
        } else
        {
            StartCoroutine(Blink());
        }
    }

    private IEnumerator Blink()
    {
        float timer = 0f;

        while (timer < blinkDuration)
        {
            material.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            material.color = oldColor.color;
            yield return new WaitForSeconds(0.1f);
            timer += 0.2f;
        }
    }
    private void DestroyEnemy()
    {
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
}
