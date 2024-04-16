using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        IHitable hitable = collision.gameObject.GetComponent<IHitable>();
        if (hitable != null)
        {
            hitable.Execute(transform);
        }
    }
}