using UnityEngine;
using Photon.Pun;

public class Chest : MonoBehaviourPun
{
    private SpriteRenderer sprite;

    private bool canOpenChest = false;
    private bool isChestOpen = false;


    void Awake()
    {
        SuscribeToPlayerModelEvent();
        GetComponents();
    }

    void OnDestroy()
    {
        UnsuscribeToPlayerModelEvent();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        CheckCollisionEnterWithPlayer(collision);
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        CheckChestCollisionExitWithPlayer(collision);
    }


    private void SuscribeToPlayerModelEvent()
    {
        PlayerController.OnInteract += InteractWithChest;
    }

    private void UnsuscribeToPlayerModelEvent()
    {
        PlayerController.OnInteract -= InteractWithChest;
    }

    private void GetComponents()
    {
        sprite = GetComponent<SpriteRenderer>();
    }

    private void InteractWithChest()
    {
        if (canOpenChest && !isChestOpen)
        {
            photonView.RPC("OpenChestRPC", RpcTarget.All);
        }
    }

    [PunRPC]
    private void OpenChestRPC()
    {
        isChestOpen = true;
        sprite.color = Color.red;        
    }

    private void CheckCollisionEnterWithPlayer(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !isChestOpen)
        {
            PhotonView playerPv = collision.gameObject.GetComponent<PhotonView>();

            if (playerPv.IsMine)
            {
                canOpenChest = true;
                sprite.color = Color.green;
            }
        }
    }

    private void CheckChestCollisionExitWithPlayer(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !isChestOpen)
        {
            PhotonView playerPv = collision.gameObject.GetComponent<PhotonView>();

            if (playerPv.IsMine)
            {
                canOpenChest = false;
                sprite.color = Color.yellow;
            }
        }
    }
}
