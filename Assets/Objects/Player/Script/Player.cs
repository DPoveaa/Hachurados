using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    #region vars

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
    public float effectSpeed;
    #endregion

    #region Movement
    [Header("Movement Mechanic")]
    [SerializeField] private float rotationSpeed;
    private float horizontalInput;
    public float speed;

    #region Jump
    [Header("Jump Mechanic")]
    public float jumpForce;
    public bool pulo;
    private float rayLength = 1.2f;
    [SerializeField] private bool canJump;
    #endregion

    #endregion

    #region Power
    [Header("Power Mechanics")]
    public bool powerState = false;
    public float staminaPenality;
    public float healthPenality;
    public float lightPenality;
    public Light worldLight;
    #endregion

    #region Heal && Stamina Bars
    [Header("Healt And Stamina Mechanics")]
    public bool resting;
    public float restingDelay;
    public float restCountdown;

    #endregion

    #region Receiving Damage
    [Header("Receiving Damage")]
    public float knockbackForce = 80f;
    public float upKnockBackForce = 20f;
    public float blinkDuration = 0.5f;
    private Vector3 damageDirection;
    public Material material;
    public Material material2;

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

        #endregion

        #region GetValues at start

        #region Speed
        oldSpeed = speed;
        #endregion

        #region Player Color
        material.color = Color.white;
        material2.color = Color.white;
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
        #region Movement Mechanic

        horizontalInput = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(horizontalInput * speed, rb.velocity.y);

        #region Rotation
        Vector3 movementInput = new Vector3(0, 0, horizontalInput);
        Vector3 movementDirection = movementInput.normalized;

        if (movementDirection != Vector3.zero)
        {
            Quaternion playerRotation = Quaternion.LookRotation(movementDirection, Vector3.up);

            transform.rotation = Quaternion.Slerp(transform.rotation, playerRotation, rotationSpeed * Time.deltaTime);
        }

        #endregion

        #region Skull Following
        skullFollow.SetBool("Follow", horizontalInput != 0);
        #endregion

        #region Jump Mechanic
        if (Physics.Raycast(transform.position, Vector3.down, rayLength))
        {
            canJump = true;
        }
        else
        {
            canJump = false;
        }

        if (Input.GetButton("Jump") && canJump == true)
        {
            rb.AddForce(Vector3.up * jumpForce * 10);
        }
        #endregion

        #endregion
    }
    #endregion

    #region Void Update
    private void Update()
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
    }
    #endregion

    #region On Collision
    public void OnCollisionEnter(Collision collision)
    {
        #region Collision With Arrow
        if (collision.gameObject.CompareTag("Arrow"))
        {
            DamageTaken(10, collision);
            Destroy(collision.gameObject);
        }
        #endregion
    }

    #endregion

    #region Functions

    #region Power Code Functions
    public void PowerPenality()
    {
        if (powerState)
        {
            if (gameManager.staminaAmount > 0)
            {
                gameManager.TakeStamina(staminaPenality);
            }
            else if (gameManager.staminaAmount <= 0)
            {
                gameManager.TakeDamage(healthPenality);
                if (worldLight.intensity >= 0.01f)
                {
                    worldLight.intensity = worldLight.intensity - lightPenality;
                }
            }
        }


    }
    #endregion

    #region Resting Function

    public void RestMode()
    {
        if (resting && !powerState)
        {
            if (gameManager.staminaAmount >= 100 && worldLight.intensity < 1)
            {
                worldLight.intensity = worldLight.intensity + lightPenality;
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
    public void DamageTaken(int damageAmount, Collision collision)
    {
        gameManager.TakeDamage(damageAmount);
        restCountdown = 0f;
        Vector3 damageDirection = (transform.position - collision.transform.position).normalized;
        Vector3 knockbackDirection = new Vector3(damageDirection.x, 0f, 0f);
        rb.AddForce(transform.up * upKnockBackForce, ForceMode.Impulse);
        rb.AddForce(knockbackDirection * knockbackForce, ForceMode.Impulse);

        StartCoroutine(Blink());
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
    }

    #endregion

    #endregion

    #endregion

}
