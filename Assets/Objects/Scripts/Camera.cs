using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{
    #region Vars

    #region Follow Target
    private Vector3 offset = new Vector3(3f, 12f, -42f);
    public float smoothTime = 0.25f;
    private Vector3 velocity = Vector3.zero;
    [SerializeField] private Transform target;
    #endregion

    #endregion
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        #region Following Target
        Vector3 targetPosition = target.position + offset;
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
        #endregion
    }
}
