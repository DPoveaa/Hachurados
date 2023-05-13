using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class General : MonoBehaviour
{
    public NavMeshAgent agent;

    public Transform player;
    public Player playerScript;

    public LayerMask whatIsGround, whatIsPlayer;

    public Animator animations;

    #region Damage

    public float health;

    #endregion

    #region Attack
    private Collider attackCheck;
    public ParticleSystem attackEffect;
    private float punchDelay;
    private float punchDisableDelay;
    #endregion

    //Patroling
    public Vector3 walkPoint;
    public bool walkPointSet;
    private float walkPointRange;

    //Attacking
    public float attackDelay = 1f;
    public float attackCountdown;
    public bool alreadyAttacked;
    public bool gonnaPunch;

    //States
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    private void Start()
    {

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
                playerScript.DamageTaken(50, null, other, 40, 5);
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

        gonnaPunch = true;
        StartCoroutine(Attack());
    }

    private IEnumerator Attack()
    {
        if (!alreadyAttacked)
        {
            yield return new WaitForSeconds(0.1f);
            attackCountdown = attackDelay;
            alreadyAttacked = true;
            animations.SetBool("Punch", true);
            yield return new WaitForSeconds(1f);
            attackCheck.enabled = true;
            attackEffect.Stop();
            attackEffect.Play();
            yield return new WaitForSeconds(0.1f);
            attackCheck.enabled = false;
            animations.SetBool("Punch", false);
        }
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
