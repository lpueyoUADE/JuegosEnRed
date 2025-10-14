using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaddleController : MonoBehaviourPun
{
    public float speed = 5f;   
    private Collider2D bounds;

    public void SetBounds(Collider2D boundsCollider)
    {
        bounds = boundsCollider;
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