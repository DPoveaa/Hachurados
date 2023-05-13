using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxDestroy : MonoBehaviour
{
    public GameObject boxEffect;
    public GameObject Prize;
    private Pages page;
    public int PageNumber;

    public void Brake()
    {
        Instantiate(boxEffect, new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z), new Quaternion(0, 0, 0, 0));
        if (Prize != null)
        {
            if (PageNumber != null && PageNumber != 0)
            {
                page = Instantiate(Prize, new Vector3(gameObject.transform.position.x, gameObject.transform.position.y - 0.7f, gameObject.transform.position.z), new Quaternion(-90, -40, 0, 0)).GetComponent<Pages>();
                page.fragmentNumber = PageNumber;
            } else
            {
                Instantiate(Prize, new Vector3(gameObject.transform.position.x, gameObject.transform.position.y - 0.7f, gameObject.transform.position.z), new Quaternion(-90, -40, 0, 0));
            }
        }
        Destroy(gameObject);
    }

}
