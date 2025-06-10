using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bulletpref : MonoBehaviour
{
    private Rigidbody bulletRigidbody;

    private void Awake()
    {
        bulletRigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        float speed = 10f;
        bulletRigidbody.linearVelocity = transform.forward * speed; // Set the bullet's initial velocity

    }

    private void OnTriggerEnter(Collider other)
    {
        Destroy(gameObject); // Destroy the bullet on collision
    }
}
