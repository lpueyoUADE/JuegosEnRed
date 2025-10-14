using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PongBall : MonoBehaviourPun
{
    public float speed = 5f;
    public float rotationSpeed = 360f; // grados por segundo

    private Vector2 direction;
    private BoxCollider2D bounds;

    private void Start()
    {
        bounds = PongGameManager.Instance.bounds;

        if (!PhotonNetwork.IsMasterClient) return;
        
        float angle = Random.Range(-45f, 45f);
        direction = Quaternion.Euler(0, 0, angle) * Vector2.right;

    }

    private void Update()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        // Movimiento base
        transform.position += (Vector3)(direction * speed * Time.deltaTime);

        // Rotación visual constante
        transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);

        CheckBoundsCollision();
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

        if (pos.x > max.x)
        {
            pos.x = max.x;
            direction.x *= -1;
        }
        else if (pos.x < min.x)
        {
            pos.x = min.x;
            direction.x *= -1;
        }

        transform.position = pos;
    }
}