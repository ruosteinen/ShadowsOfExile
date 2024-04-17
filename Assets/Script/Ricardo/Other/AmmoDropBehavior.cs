using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AmmoDropBehavior : MonoBehaviour
{
    [SerializeField] Transform player;
    [SerializeField] private bool doRandomJump;
    [SerializeField] private float range;
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;

        Rigidbody rb = GetComponent<Rigidbody>();

        if (doRandomJump)
        {
            Vector2 pointEx = Random.insideUnitCircle;

            Vector3 point = new Vector3(pointEx.x, transform.position.y + 1.5f, pointEx.y);

            rb.AddForce(point*2, ForceMode.Impulse);

            point = new Vector3(point.x, Random.value, point.z);
            rb.AddTorque(point, ForceMode.Impulse);
        }
    }

    private void Update()
    {
        if (Vector3.Distance(transform.position, player.position) <= range)
        {
            transform.position = Vector3.MoveTowards(transform.position, player.position, 0.075f);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            
            Destroy(gameObject);
        }
    }
}
