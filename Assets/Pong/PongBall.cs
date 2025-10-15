using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PongBall : MonoBehaviourPun
{
    [Header("Movimiento")]
    public float speed = 5f;
    public float rotationSpeed = 360f;

    [Header("Trail")]
    public TrailRenderer trailRenderer;
    
    private Vector2 direction;
    private BoxCollider2D bounds;

    private void Start()
    {
        bounds = PongGameManager.Instance.bounds;
    }

    private void Update()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        Move();
        CheckBoundsCollision();
    }

    private void Move()
    {
        transform.position += (Vector3)(direction * speed * Time.deltaTime);
        transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime * Mathf.Sign(direction.x));
    }

    private void CheckBoundsCollision()
    {
        Vector2 min = bounds.bounds.min;
        Vector2 max = bounds.bounds.max;
        Vector2 pos = transform.position;

        if (pos.y > max.y)
        {
            pos.y = max.y;
            direction.y *= -1;
        }
        else if (pos.y < min.y)
        {
            pos.y = min.y;
            direction.y *= -1;
        }

        // Goles / límites horizontales
        if (pos.x > max.x)
        {
            PongGameManager.Instance.AddPointToLeft();
            return;
        }
        else if (pos.x < min.x)
        {
            PongGameManager.Instance.AddPointToRight();
            return;
        }

        transform.position = pos;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        if (other.CompareTag("Paddle"))
        {
            direction.x *= -1;

            // Modificar ángulo según punto de impacto
            float offset = transform.position.y - other.transform.position.y;
            float normalizedOffset = offset / other.bounds.extents.y;
            direction.y += normalizedOffset * 0.5f;
            direction.Normalize();

            speed *= 1.02f; // leve aceleración
        }
    }
    public void SetPosition(Vector3 position)
    {
        trailRenderer.emitting = false;
        transform.position = position;
        trailRenderer.Clear();
        trailRenderer.emitting = true;
    }
    public void LaunchInDirection(bool toRight)
    {
        float angle = Random.Range(-45f, 45f);
        Vector2 baseDir = toRight ? Vector2.right : Vector2.left;
        direction = Quaternion.Euler(0, 0, angle) * baseDir;
    }
}