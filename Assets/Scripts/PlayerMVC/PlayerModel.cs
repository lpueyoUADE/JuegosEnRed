using UnityEngine;
using Photon.Pun;

public class PlayerModel : MonoBehaviourPun
{
    private BoomerangController boomerangController;
    private Rigidbody2D rb;
    private BoxCollider2D boxCollider;
    private Transform boomerangHandPosition;

    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private Animator animator;

    [SerializeField] private int startingHealth;

    [SerializeField] private float speed;
    [SerializeField] private float jumpForce;

    private int health;
    private int minHealth = 1;
    
    private bool isGrounded;

    public Transform BoomerangHandPosition { get => boomerangHandPosition; }


    void Awake()
    {
        GetComponents();
        InitializeSkin();
        InitializeHealth();
        InitializeBoomerang();
    }

    void Update()
    {
        RotatePlayer();
    }

    void FixedUpdate()
    {
        Movement();
        CheckIsOnFloor();
    }


    public void Attack()
    {
        // Si tiene el boomerang en la mano
        if (boomerangController.BoomerangModel.CircleCollider.enabled == false)
        {
            Vector2 cursorScreenPos = HybridCursorManager.Instance.GetCursorPosition();
            Vector3 cursorWorldPos = Camera.main.ScreenToWorldPoint(cursorScreenPos);
            cursorWorldPos.z = 0f;

            Vector2 dir = (cursorWorldPos - boomerangHandPosition.position).normalized;
            boomerangController.BoomerangModel.photonView.RPC("ThrowBoomerang", RpcTarget.AllBuffered, dir);
            return;
        }

        // Si el boomerang esta pegado a algun objeto del escenario
        else if (boomerangController.BoomerangModel.Rb.velocity.sqrMagnitude == 0)
        {
            Vector2 dir = (transform.position - boomerangController.transform.position).normalized;
            boomerangController.BoomerangModel.photonView.RPC("ReturnBoomerang", RpcTarget.AllBuffered);
            return;            
        }
    }

    public void Jump()
    {
        if (isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0f);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }

    [PunRPC]
    public void GetDamage(int damage)
    {
        health -= damage;

        if (health < minHealth)
        {
            PhotonNetwork.Destroy(gameObject);  
        }
    }


    private void GetComponents()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        boomerangHandPosition = transform.Find("BoomerangHandPosition");
    }

    private void InitializeSkin()
    {
        if (photonView.Owner.CustomProperties.ContainsKey("SkinIndex"))
        {
            int skinIndex = (int)photonView.Owner.CustomProperties["SkinIndex"];
            sprite.color = PlayerSkinManager.Instance.PlayerSkins[skinIndex];
        }
    }

    private void InitializeHealth()
    {
        health = startingHealth;
    }

    private void InitializeBoomerang()
    {
        if (photonView.IsMine)
        {
            GameObject projGO = PhotonNetwork.Instantiate("Prefabs/Boomerangs/Boomerang", boomerangHandPosition.position, Quaternion.identity);
            boomerangController = projGO.GetComponent<BoomerangController>();
            boomerangController.BoomerangModel.photonView.RPC("Initialize", RpcTarget.AllBuffered, photonView.OwnerActorNr);
        }
    }

    private void Movement()
    {
        if (photonView.IsMine)
        {
            Vector2 move = PlayerInputsManager.Instance.GetMoveAxis();
            rb.velocity = new Vector2(move.normalized.x * speed, rb.velocity.y);
        }
        float vel = new Vector3(rb.velocity.x, 0).magnitude;
        animator.SetFloat("velocity", vel);
    }

    private void CheckIsOnFloor()
    {
        if (photonView.IsMine)
        {
            Vector2 origin = (Vector2)transform.position + boxCollider.offset;
            float extraHeight = 0.05f;

            RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, (boxCollider.size.y / 2f) + extraHeight, LayerMask.GetMask("Floor"));

            isGrounded = hit.collider != null;
        }
    }

    private void RotatePlayer()
    {
        if (photonView.IsMine)
        {
            if (rb.velocity.x > 0.1f)
            {
                //sprite.flipX = false;
                transform.localScale = new Vector3(1, 1, 1);
            }

            else if (rb.velocity.x < -0.1)
            {
                //sprite.flipX = true;
                transform.localScale = new Vector3(-1, 1, 1);
            }
        }
    }
}
