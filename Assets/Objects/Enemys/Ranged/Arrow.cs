using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    private bool groundHit = false;
    private float disapearCountdown = 2f;
    public Renderer arrow;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (groundHit)
        {
            disapearCountdown -= Time.deltaTime;
            if (disapearCountdown <= 0)
            {
                Destroy(gameObject);
            }
        }
    }

    public void OnCollisionEnter(Collision collision)
    {
        groundHit = true;
    }
}
