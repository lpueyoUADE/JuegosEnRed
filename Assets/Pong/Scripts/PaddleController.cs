using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaddleController : MonoBehaviourPun
{
    public float speed = 5f;
    public SpriteRenderer spriteRenderer;

    private Collider2D bounds;

    public static readonly Color[] paddleColors = 
    {
        Color.red,
        Color.green,
        Color.blue,
        Color.yellow,
        Color.cyan,
        Color.magenta
    };

    public static Color GetRandomColor()
    {
        return paddleColors[Random.Range(0, paddleColors.Length)];
    }
    public void SetBounds(Collider2D boundsCollider)
    {
        bounds = boundsCollider;
    }

    private void Start()
    {
        spriteRenderer.material = new Material(spriteRenderer.material); 
    }

    [PunRPC]
    public void RPC_SetColor(float r, float g, float b)
    {
        SetColor(new Color(r, g, b));
    }

    public void SetColor(Color color)
    {
        spriteRenderer.material.SetColor("_SolidOutline", color);
    }

    void Update()
    {
        if (!photonView.IsMine) return;

        float move = Input.GetAxisRaw("Vertical") * speed * Time.deltaTime;

        float minY = bounds.bounds.min.y + transform.localScale.y / 2;
        float maxY = bounds.bounds.max.y - transform.localScale.y / 2;

        float newY = Mathf.Clamp(transform.position.y + move, minY, maxY);
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
}