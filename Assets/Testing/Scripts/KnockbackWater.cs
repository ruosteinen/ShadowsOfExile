using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnockbackWater : MonoBehaviour, IHitable
{
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Execute(Transform executionSource)
    {
        KnockbackEntity(executionSource);
    }

    private void KnockbackEntity(Transform executionSource)
    {
        Vector3 dir = (transform.position - executionSource.transform.position).normalized;
        float knockbackForce = 10.0f;
        rb.AddForce(dir * knockbackForce, ForceMode.Impulse);
    }
}

public interface IHitable
{
    public void Execute(Transform executionSource);
}