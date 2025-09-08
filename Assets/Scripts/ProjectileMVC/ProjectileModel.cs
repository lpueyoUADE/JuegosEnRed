using UnityEngine;
using Photon.Pun;

public class ProjectileModel : MonoBehaviourPun//, IPunInstantiateMagicCallback
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

    private bool canRotate = false;
    private bool isReturning = false;

    public Rigidbody2D Rb { get => rb; }
    public CircleCollider2D CircleCollider { get => circleCollider; }


    /*public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        object[] data = info.photonView.InstantiationData;
        if (data != null && data.Length >= 2)
        {
            ownerActorNumber = (int)data[0];
            int ownerViewID = (int)data[1];

            PhotonView ownerPV = PhotonView.Find(ownerViewID);
            if (ownerPV != null)
            {
                PlayerModel owner = ownerPV.GetComponent<PlayerModel>();
                if (owner != null)
                {
                    Initialize(ownerActorNumber, owner);
                }
            }
        }
    }*/

    void Awake()
    {
        GetComponents();
    }

    void Update()
    {
        Rotation();
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


    public void Initialize(int owner, PlayerModel ownerPlayerModel)
    {
        ownerActorNumber = owner;
        this.ownerPlayerModel = ownerPlayerModel;
        rb.simulated = false;
        circleCollider.enabled = false;
        transform.SetParent(this.ownerPlayerModel.transform);
        transform.position = this.ownerPlayerModel.AttackPosition.position;
        ownerPlayerCollider = ownerPlayerModel.GetComponent<BoxCollider2D>();
    }

    public void Throw(Vector2 dir)
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

    public void Return()
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

    private void OnCollisionEnterWithOtherPlayers(Collision2D collision)
    {
        if (!photonView.IsMine) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            PhotonView targetPV = collision.gameObject.GetComponent<PhotonView>();

            if (targetPV.OwnerActorNr != ownerActorNumber)
            {
                targetPV.RPC("GetDamage", targetPV.Owner, damage);
            }
        }
    }

    private void OnCollisionEnterWithScenary(Collision2D collision)
    {
        if (!photonView.IsMine) return;

        if (!collision.gameObject.CompareTag("Player"))
        {
            rb.bodyType = RigidbodyType2D.Static;
            canRotate = false;
            circleCollider.isTrigger = true;
        }
    }

    private void OnTriggerEnterWithOwnPlayer(Collider2D collider)
    {
        if (!photonView.IsMine) return;

        if (collider.gameObject.CompareTag("Player"))
        {
            PhotonView targetPV = collider.gameObject.GetComponent<PhotonView>();

            if (targetPV.OwnerActorNr == ownerActorNumber)
            {
                transform.SetParent(ownerPlayerModel.transform);
                transform.position = ownerPlayerModel.AttackPosition.position;
                circleCollider.isTrigger = false;
                canRotate = false;
                isReturning = false;
                rb.simulated = false;
                circleCollider.enabled = false;
            }
        }
    }

    private void OnTriggerEnterWithOtherPlayers(Collider2D collider)
    {
        if (!photonView.IsMine) return;

        if (collider.gameObject.CompareTag("Player"))
        {
            PhotonView targetPV = collider.gameObject.GetComponent<PhotonView>();

            if (targetPV.OwnerActorNr != ownerActorNumber)
            {
                targetPV.RPC("GetDamage", targetPV.Owner, damage);
            }
        }
    }
}
