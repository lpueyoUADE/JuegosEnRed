using UnityEngine;
using Photon.Pun;

public class ProjectileModel : MonoBehaviourPun
{
    private Rigidbody2D rb;

    private Vector2 currentDir;

    [SerializeField] private int damage;

    [SerializeField] private float speed;

    private int ownerActorNumber;


    void Awake()
    {
        GetComponents();
    }

    void FixedUpdate()
    {
        Movement();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        CheckCollisionsEnter(collision);
    }


    public void Initialize(Vector2 dir, int owner)
    {
        currentDir = dir;
        ownerActorNumber = owner;
    }


    private void GetComponents()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Movement()
    {
        if (photonView.IsMine)
        {
            Vector2 dir = currentDir;
            rb.velocity = dir.normalized * speed;
        }
    }

    private void CheckCollisionsEnter(Collision2D collision)
    {
        if (!photonView.IsMine) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            PhotonView targetPV = collision.gameObject.GetComponent<PhotonView>();

            if (targetPV.OwnerActorNr != ownerActorNumber)
            {
                targetPV.RPC("GetDamage", targetPV.Owner, damage);
                PhotonNetwork.Destroy(gameObject);
            }
        }

        else
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }
}
