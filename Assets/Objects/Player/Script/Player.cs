using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.EventSystems.EventTrigger;

public class Player : MonoBehaviour
{
    #region vars

    #region Sounds

    public AudioSource hurtedSFX;

    #endregion

    #region

    public Animator animations;

    #endregion

    #region Another Scripts

    public GameManager gameManager;

    #endregion

    #region Components
    [Header("Components")]
    private CharacterController ct;
    private Rigidbody rb;
    #endregion

    #region Skull
    [Header("Skull Mechanics")]
    public Transform skull;
    public Animator skullFollow;
    #endregion

    #region Speed boost
    [Header("Speed Hability")]
    private float oldSpeed;
    private float effectSpeed = 10f;
    #endregion

    #region Movement
    [Header("Movement Mechanic")]
    private float rotationSpeed = 5f;
    private float horizontalInput;
    private float speed = 5f;

    #region Jump
    [Header("Jump Mechanic")]
    public float jumpForce;
    private float rayLength = 1.3f;
    private bool canJump;
    RaycastHit hit;
    #endregion

    #endregion

    #region Power
    [Header("Power Mechanics")]
    private bool powerState = false;
    public float staminaPenality;
    public float healthPenality;
    public float lightPenality;
    public Light worldLight;
    #endregion

    #region Heal && Stamina Bars
    [Header("Healt And Stamina Mechanics")]
    private bool resting;
    public float restingDelay;
    private float restCountdown;

    #endregion

    #region Receiving Damage
    [Header("Receiving Damage")]
    public float blinkDuration = 0.5f;
    private bool canReceive = true;
    public Material material;
    public Material material2;

    #endregion

    #region Attacking
    [SerializeField, Header("Attacking Values")]
    public float attackDelay;
    public Transform attackPoint;
    public float attackCountdown = 0;
    private Collider attackCheck;
    public LayerMask enemyLayers;
    private Melee melee;
    private Ranged ranged;
    private BoxDestroy box;
    public bool attacked;

    #endregion

    #endregion

    #region Void Start
    void Start()
    {
        #region GetComponent at start

        #region Movement
        ct = GetComponent<CharacterController>();
        rb = GetComponent<Rigidbody>();
        #endregion

        #region Attack

        attackCheck = GetComponent<BoxCollider>();

        #endregion

        #endregion

        #region GetValues at start

        #region Speed
        oldSpeed = speed;
        #endregion

        #region Player Color
        material.color = Color.white;
        material2.color = Color.white;
        #endregion

        #region

        worldLight.intensity = 1;

        #endregion

        #endregion

        #region InvokeRepeating

        #region Power Verification
        InvokeRepeating("PowerPenality", 0f, 0.2f);
        #endregion

        #region Rest Mode
        InvokeRepeating("RestMode", 0.2f, 0.2f);
        #endregion

        #endregion

    }
    #endregion

    #region Void FixedUpdate
    void FixedUpdate()
    {
        if (!GameManager.isPaused)
        {
            #region Movement Mechanic

            horizontalInput = Input.GetAxis("Horizontal");
            rb.velocity = new Vector2(horizontalInput * speed, rb.velocity.y);

            #region Rotation
            Vector3 movementInput = new Vector3(0, 0, horizontalInput);
            Vector3 movementDirection = movementInput.normalized;

            if (movementDirection != Vector3.zero)
            {
                Quaternion playerRotation = Quaternion.LookRotation(movementDirection, Vector3.up);
                animations.SetBool("Walking", true);
                transform.rotation = Quaternion.Slerp(transform.rotation, playerRotation, rotationSpeed * Time.deltaTime);
            } else
            {
                animations.SetBool("Walking", false);
            }

            #endregion

            #region Skull Following
            skullFollow.SetBool("Follow", horizontalInput != 0);
            #endregion

            #region Jump Mechanic
            if (Physics.Raycast(transform.position, Vector3.down, out hit, rayLength))
            {
                if (!hit.collider.CompareTag("Trap"))
                {
                    canJump = true;
                    animations.SetBool("Jump", false);
                }
            }
            else
            {
                canJump = false;
                animations.SetBool("Jump", false);
            }

            if (Input.GetButton("Jump") && canJump == true)
            {
                animations.SetBool("Jump",true);
                rb.AddForce(Vector3.up * jumpForce * 10);
            }
            #endregion

            #endregion
        }
    }
    #endregion

    #region Void Update
    private void Update()
    {
        if (!GameManager.isPaused)
        {
            #region Power Mechanic

            if (Input.GetMouseButtonDown(1))
            {
                if (!powerState)
                {
                    speed = effectSpeed;
                    powerState = true;
                    //attackSpeed = attackSpeed * 2;
                }
                else if (powerState)
                {
                    speed = oldSpeed;
                    powerState = false;
                }
            }
            #endregion

            #region Rest Check
            if (!powerState)
            {
                if (restCountdown < 5)
                {
                    restCountdown += Time.deltaTime;
                    resting = false;
                }
                if (restCountdown >= restingDelay && !resting)
                {
                    resting = true;
                    RestMode();
                }
            }
            else
            {
                restCountdown = 0f;
                resting = false;
            }
            #endregion

            #region Attack Mechanic
            if (attackCountdown > 0)
            {
                attackCountdown -= Time.deltaTime;
            }
            else if (Input.GetMouseButtonDown(0) && attackCountdown <= 0)
            {
                UnityEngine.Debug.Log("Attacked");
                attackCountdown = attackDelay;
                StartCoroutine(Attacking());
            }

            if (attackCountdown <= 0)
            {
                attacked = false;
            }
            #endregion

        }
    }
    #endregion

    #region On Collision
    public void OnCollisionEnter(Collision collision)
    {
        #region Collision With Arrow
        if (collision.gameObject.CompareTag("Arrow"))
        {
            DamageTaken(10, collision, null, 80, 10);
            Destroy(collision.gameObject);
        }

        if (collision.gameObject.CompareTag("Trap"))
        {
            DamageTaken(50, collision, null, 20, 0);
        }

        if (collision.gameObject.CompareTag("Limit"))
        {
            DamageTaken(110, collision, null, 0, 0);
        }
        #endregion
    }

    #endregion

    #region On Trigger

    public void OnTriggerEnter(Collider other)
    {

    }

    #endregion

    #region Functions

    #region Power Code Functions
    public void PowerPenality()
    {
        if (powerState)
        {
            if (gameManager.staminaAmount <= 50)
            {
                gameManager.TakeStamina(staminaPenality);
                if (worldLight.intensity >= 0.01f)
                {
                    worldLight.intensity = worldLight.intensity - lightPenality;
                }
            }
            if (gameManager.staminaAmount > 50)
            {
                gameManager.TakeStamina(staminaPenality);
            }
            if (gameManager.staminaAmount <= 0)
            {
                gameManager.TakeDamage(healthPenality);
            }
        }


    }
    #endregion

    #region Resting Functions

    public void RestMode()
    {
        if (resting && !powerState)
        {
            if (gameManager.staminaAmount >= 80 && worldLight.intensity < 1 && !GameManager.bossDead)
            {
                worldLight.intensity = worldLight.intensity + lightPenality;
                gameManager.Rest(5);
            }
            else if (gameManager.healthAmount < 100)
            {
                gameManager.Heal(1);
            }
            else if (gameManager.healthAmount >= 100)
            {
                gameManager.Rest(5);
            }

        }
    }

    #endregion

    #region Damage Functions

    #region Damage Taken
    public void DamageTaken(int damageAmount, Collision collision, Collider trigger, float knockbackForce, float upKnockBackForce)
    {
        if (canReceive)
        {
            canReceive = false;
            animations.SetBool("Hitted", true);
            gameManager.TakeDamage(damageAmount);
            hurtedSFX.Play();
            restCountdown = 0f;
            if (collision != null)
            {
                Vector3 damageDirection = (transform.position - collision.transform.position).normalized;
                Vector3 knockbackDirection = new Vector3(damageDirection.x, 0f, damageDirection.z) * 2;
                rb.AddForce(transform.up * upKnockBackForce, ForceMode.Impulse);
                rb.AddForce(knockbackDirection * knockbackForce, ForceMode.Impulse);
            }
            else if (trigger != null)
            {
                Vector3 damageDirection = (transform.position - trigger.transform.position).normalized;
                Vector3 knockbackDirection = new Vector3(damageDirection.x, 0f, damageDirection.z) * 2;
                rb.AddForce(transform.up * upKnockBackForce, ForceMode.Impulse);
                rb.AddForce(knockbackDirection * knockbackForce, ForceMode.Impulse);
            }

            StartCoroutine(Blink());
        }
    }
    #endregion

    #region Attack Delay

    private IEnumerator Attacking()
    {
        UnityEngine.Debug.Log("Coroutine");
        animations.SetBool("Attack", true);;
        yield return new WaitForSeconds(0.5f);
        Attack();
        yield return new WaitForSeconds(0.2f);
        animations.SetBool("Attack", false);
    }

    #endregion

    #region Blink
    private IEnumerator Blink()
    {
        float timer = 0f;

        while (timer < blinkDuration)
        {
            material.color = Color.red;
            material2.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            material.color = Color.white;
            material2.color = Color.white;
            yield return new WaitForSeconds(0.1f);
            timer += 0.2f;
        }
        animations.SetBool("Hitted", false);
        canReceive = true;
    }

    #endregion

    #endregion

    #region Attack Functions

    public void Attack()
    {
        if (!attacked)
        {
            attacked = true;
            UnityEngine.Debug.Log("Coroutine");
            Collider[] hitCollider = Physics.OverlapBox(attackPoint.position, new Vector3(3f, 2f, 2f), new Quaternion(0, 0, 0, 0), enemyLayers);

            foreach (Collider objects in hitCollider)
            {
                if (objects.GetComponent<Melee>() != null)
                {
                    Melee melee = objects.GetComponent<Melee>();
                    melee.TakeDamage(1);
                }

                if (objects.GetComponent<Ranged>() != null)
                {
                    Ranged ranged = objects.GetComponent<Ranged>();
                    ranged.TakeDamage(1);
                }

                if (objects.GetComponent<General>() != null)
                {
                    General general = objects.GetComponent<General>();
                    general.TakeDamage(1);
                }

                if (objects.GetComponent<BoxDestroy>() != null)
                {
                    BoxDestroy box = objects.GetComponent<BoxDestroy>();
                    box.Brake();
                }
            }
        }
    }

    #endregion

    #endregion

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(attackPoint.position, new Vector3(3f, 2f, 2f));
    }
}
