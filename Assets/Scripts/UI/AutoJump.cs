using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoJump : MonoBehaviour
{
    [Header("Salto")]
    public float jumpForce = 60f;       // Altura del salto en pixeles
    public float gravity = -300f;       // Fuerza de caída

    [Header("Tiempo entre saltos")]
    public float minWaitTime = 1f;
    public float maxWaitTime = 3f;

    private float groundY;
    private float verticalVelocity = 0f;
    private float nextJumpTime = 0f;
    private RectTransform rect;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    void Start()
    {
        // Se guarda la posición inicial como piso
        groundY = rect.anchoredPosition.y;
        ScheduleNextJump();
    }

    void Update()
    {
        // Saltar cuando toque el tiempo
        if (Time.time >= nextJumpTime && IsGrounded())
        {
            verticalVelocity = jumpForce;
            ScheduleNextJump();
        }

        // Aplicar gravedad
        verticalVelocity += gravity * Time.deltaTime;

        // Movimiento
        Vector2 pos = rect.anchoredPosition;
        pos.y += verticalVelocity * Time.deltaTime;

        // Si toca el suelo, reiniciar
        if (pos.y <= groundY)
        {
            pos.y = groundY;
            verticalVelocity = 0f;
        }

        rect.anchoredPosition = pos;
    }

    void ScheduleNextJump()
    {
        nextJumpTime = Time.time + Random.Range(minWaitTime, maxWaitTime);
    }

    bool IsGrounded()
    {
        return Mathf.Approximately(rect.anchoredPosition.y, groundY);
    }
}