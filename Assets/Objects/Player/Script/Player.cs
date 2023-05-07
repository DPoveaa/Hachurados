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
    [SerializeField] private float skullSpeed;
    public bool pulo;
    private CharacterController ct;
    private Rigidbody rb;
    public GameObject skull;


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

        Vector3 SkullDistance = new Vector3(skull.transform.position.x - 2, 0, 0);

        if (rb.velocity.magnitude != 0)
        {
            skull.transform.position = Vector3.SmoothDamp(skull.transform.position, SkullDistance, ref movementInput, skullSpeed);
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

    /*
    private void LateUpdate()
    {

    // Calcula a direção e a velocidade em que a caveira deve se mover
    Vector3 targetPosition = skull.position - (skull.forward * 2f); // move a posição para trás do personagem
    Vector3 currentVelocity = (targetPosition - transform.position).normalized * maxSpeed;

    // Move a caveira suavemente para a nova posição
    transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref currentVelocity, smoothTime);
    }
    */
}
