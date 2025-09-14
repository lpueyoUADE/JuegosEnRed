using UnityEngine;
using Photon.Pun;

public class Teleport2D : MonoBehaviour
{
    [SerializeField] private Transform teleportPosition;


    void OnTriggerEnter2D(Collider2D collider)
    {
        OnTriggerEnter2DWithPlayerOrBoomerang(collider);
    }


    private void OnTriggerEnter2DWithPlayerOrBoomerang(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Player") || collider.gameObject.CompareTag("Boomerang"))
        {
            PhotonView pv = collider.GetComponent<PhotonView>();

            if (pv != null && pv.IsMine)
            {
                pv.RPC("Teleport", RpcTarget.AllBuffered, teleportPosition.position);
            }
        }
    }
}
