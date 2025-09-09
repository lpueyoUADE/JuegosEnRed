using UnityEngine;
using Photon.Pun;

public class ProjectileModel : MonoBehaviourPun
{
    private Rigidbody2D rb;
    private CircleCollider2D circleCollider;

    private PlayerModel ownerPlayerModel;
    private BoxCollider2D ownerPlayerCollider;

    private Vector2 currentDir;

    [SerializeField] private int damage;

    [SerializeField] private float movementSpeed;
    [SerializeField] private float rotationSpeed;

    private int ownerActorNumber;
    private int? auxiliarPlayerHitActorNumber;

    private float counterBoomerangComeBackAutomatically = 0f;

    private bool canRotate = false;
    private bool isReturning = false;

    public Rigidbody2D Rb { get => rb; }
    public CircleCollider2D CircleCollider { get => circleCollider; }


    void Awake()
    {
        GetComponents();
    }

    void Update()
    {
        Rotation();
        photonView.RPC("ReturnBoomerangAutomaticalyAfterSeconds", RpcTarget.AllBuffered);
    }

    void FixedUpdate()
    {
        Movement();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        OnCollisionEnterWithOtherPlayers(collision);
        OnCollisionEnterWithScenary(collision);
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        OnTriggerEnterWithOwnPlayer(collider);
        OnTriggerEnterWithOtherPlayers(collider);
    }


    [PunRPC]
    public void Initialize(int owner)
    {
        ownerActorNumber = owner;

        foreach (PlayerModel playerModel in FindObjectsOfType<PlayerModel>())
        {
            if (playerModel.photonView.OwnerActorNr == ownerActorNumber)
            {
                ownerPlayerModel = playerModel;
                break;
            }
        }

        rb.simulated = false;
        circleCollider.enabled = false;
        transform.SetParent(ownerPlayerModel.transform);
        transform.position = ownerPlayerModel.AttackPosition.position;
        ownerPlayerCollider = ownerPlayerModel.GetComponent<BoxCollider2D>();
    }

    [PunRPC]
    public void ThrowBoomerang(Vector2 dir)
    {
        transform.SetParent(null);
        currentDir = dir;
        canRotate = true;
        isReturning = false;
        rb.simulated = true;
        circleCollider.enabled = true;
        rb.bodyType = RigidbodyType2D.Dynamic;
        Physics2D.IgnoreCollision(circleCollider, ownerPlayerCollider, true);
    }

    [PunRPC]
    public void ReturnBoomerang()
    {
        canRotate = true;
        isReturning = true;
        rb.simulated = true;
        circleCollider.enabled = true;
        rb.bodyType = RigidbodyType2D.Dynamic;
        Physics2D.IgnoreCollision(circleCollider, ownerPlayerCollider, false);
    }


    private void GetComponents()
    {
        rb = GetComponent<Rigidbody2D>();
        circleCollider = GetComponent<CircleCollider2D>();
    }

    private void Movement()
    {
        if (!photonView.IsMine) return;

        if (isReturning && ownerPlayerModel != null)
        {
            currentDir = ((Vector2)ownerPlayerModel.transform.position - (Vector2)transform.position).normalized;
        }

        if (rb.bodyType == RigidbodyType2D.Dynamic)
        {
            rb.velocity = currentDir.normalized * movementSpeed;
        }
    }

    private void Rotation()
    {
        if (!photonView.IsMine) return;

        if (canRotate)
        {
            transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
        }
    }

    [PunRPC]
    private void ReturnBoomerangAutomaticalyAfterSeconds()
    {
        if (auxiliarPlayerHitActorNumber != null) 
        {
            counterBoomerangComeBackAutomatically += Time.deltaTime;

            if (counterBoomerangComeBackAutomatically >= 3f)
            {
                photonView.RPC("ReturnBoomerang", RpcTarget.AllBuffered);
                counterBoomerangComeBackAutomatically = 0f;
            }
        }
    }

    [PunRPC]
    private void OnBoomerangCollisionEnterWithScenary()
    {
        rb.bodyType = RigidbodyType2D.Static;
        canRotate = false;
        circleCollider.isTrigger = true;
    }

    /// <summary>
    /// Testear aca que el boomerang se quede pegado al player cuando choca
    /// </summary>
    [PunRPC]
    private void OnBoomerangCollisionEnterWithOtherPlayers(int hitPlayerActorNr)
    {
        rb.simulated = false;
        rb.bodyType = RigidbodyType2D.Static;
        canRotate = false;
        circleCollider.isTrigger = true;

        foreach (PlayerModel playerModel in FindObjectsOfType<PlayerModel>())
        {
            if (playerModel.photonView.OwnerActorNr == hitPlayerActorNr)
            {
                auxiliarPlayerHitActorNumber = hitPlayerActorNr;
                transform.SetParent(playerModel.transform); 
                break;
            }
        }
    }

    [PunRPC]
    private void OnBoomerangTriggerEnterWithOwnPlayer()
    {
        auxiliarPlayerHitActorNumber = null;
        transform.SetParent(ownerPlayerModel.transform);
        transform.position = ownerPlayerModel.AttackPosition.position;
        circleCollider.isTrigger = false;
        canRotate = false;
        isReturning = false;
        rb.simulated = false;
        circleCollider.enabled = false;
    }

    private void OnCollisionEnterWithOtherPlayers(Collision2D collision)
    {
        if (!photonView.IsMine) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            PhotonView targetPV = collision.gameObject.GetComponent<PhotonView>();

            if (targetPV.OwnerActorNr != ownerActorNumber)
            {
                targetPV.RPC("GetDamage", targetPV.Owner, damage);
                photonView.RPC("OnBoomerangCollisionEnterWithOtherPlayers", RpcTarget.AllBuffered, targetPV.OwnerActorNr);
            }
        }
    }

    private void OnCollisionEnterWithScenary(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
        {
            photonView.RPC("OnBoomerangCollisionEnterWithScenary", RpcTarget.AllBuffered);
        }
    }

    private void OnTriggerEnterWithOwnPlayer(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            PhotonView targetPV = collider.gameObject.GetComponent<PhotonView>();

            if (targetPV.OwnerActorNr == ownerActorNumber)
            {
                photonView.RPC("OnBoomerangTriggerEnterWithOwnPlayer", RpcTarget.AllBuffered);
            }
        }
    }

    private void OnTriggerEnterWithOtherPlayers(Collider2D collider)
    {
        if (!photonView.IsMine) return;

        if (collider.gameObject.CompareTag("Player"))
        {
            PhotonView targetPV = collider.gameObject.GetComponent<PhotonView>();

            // Si no soy yo y no soy el que tiene el boomerang pegado no hacer daño
            if (targetPV.OwnerActorNr != ownerActorNumber && targetPV.OwnerActorNr != auxiliarPlayerHitActorNumber)
            {
                targetPV.RPC("GetDamage", targetPV.Owner, damage);
            }
        }
    }
}