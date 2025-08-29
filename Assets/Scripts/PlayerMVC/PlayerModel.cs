using UnityEngine;
using Photon.Pun;

public class PlayerModel : MonoBehaviourPun
{
    private Rigidbody2D rb;
    private BoxCollider2D boxCollider;
    private SpriteRenderer sprite;
    private Transform attackPosition;

    [SerializeField] private int startingHealth;

    [SerializeField] private float speed;
    [SerializeField] private float jumpForce;

    private int health;
    private int minHealth = 1;
    
    private bool isGrounded;


    void Awake()
    {
        GetComponents();
        InitializeSkin();
        InitializeHealth();
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
        GameObject projGO = PhotonNetwork.Instantiate("Prefabs/Projectiles/Projectile", attackPosition.position, Quaternion.identity);
        ProjectileModel proj = projGO.GetComponent<ProjectileModel>();

        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0;
        Vector2 dir = (mouseWorldPos - transform.position).normalized;
        proj.Initialize(dir, photonView.OwnerActorNr);
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
        sprite = GetComponent<SpriteRenderer>();
        attackPosition = transform.Find("AttackPosition");
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

    private void Movement()
    {
        if (photonView.IsMine)
        {
            Vector2 move = PlayerInputsManager.Instance.GetMoveAxis();
            rb.velocity = new Vector2(move.normalized.x * speed, rb.velocity.y);
        }
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
                transform.localScale = new Vector3(1, 1, 1);
            }

            else if (rb.velocity.x < -0.1)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
        }
    }
}
