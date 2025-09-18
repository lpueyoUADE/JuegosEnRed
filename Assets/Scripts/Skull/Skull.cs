using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skull : MonoBehaviour
{
    private Rigidbody2D rb;
    private ParticleSystem ps;
    private ParticleSystem.EmissionModule emission;

    private float minSpeed = 0.01f;

    void Awake()
    {
        ps = GetComponentInChildren<ParticleSystem>();
        emission = ps.emission;

        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        bool isMoving = rb.velocity.magnitude > minSpeed;
        emission.enabled = isMoving;
    }
}
