using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    #region vars
    private float horizontalInput;
    public float rayLength;
    public float jumpForce;
    public float speed;
    [SerializeField] private bool canJump;
    [SerializeField] private float rotationSpeed;
    public bool pulo;
    private CharacterController ct;
    private Rigidbody rb;


    #endregion
    void Start()
    {
        ct = GetComponent<CharacterController>();
        rb = GetComponent<Rigidbody>();  
    }

    void FixedUpdate()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(horizontalInput * speed, rb.velocity.y);

        Vector3 movementInput = new Vector3(0, 0, horizontalInput);
        Vector3 movementDirection = movementInput.normalized;

        if (movementDirection != Vector3.zero)
        {
            Quaternion playerRotation = Quaternion.LookRotation(movementDirection, Vector3.up);

            transform.rotation = Quaternion.Slerp(transform.rotation, playerRotation, rotationSpeed * Time.deltaTime);
        }

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
    }
}
