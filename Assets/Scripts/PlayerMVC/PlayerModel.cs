using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class PlayerModel : MonoBehaviourPun
{
    private BoomerangController boomerangController;
    private Rigidbody2D rb;
    private BoxCollider2D boxCollider;
    private SpriteRenderer sprite;
    private Animator animator;
    private Slider healthBar;
    private Image fillImage;
    private Transform boomerangHandPosition;

    [SerializeField] private Color mySliderColorView;
    [SerializeField] private Color othrsSliderColorView;

    [SerializeField] private int startingHealth;

    [SerializeField] private float speed;
    [SerializeField] private float jumpForce;

    private int health;
    private int minHealth = 1;
    
    private bool isGrounded;
    private bool acceptingInput;

    public Transform BoomerangHandPosition { get => boomerangHandPosition; }

    public bool AcceptingInput { get => acceptingInput; set => acceptingInput = value; }


    void Awake()
    {
        SuscribeToUpdateManagerEvents();
        GetComponents();
        InitializeSkin();
        InitializeHealthAndHealthBar();
        InitializeBoomerang();
    }

    // Simulacion de Update
    void UpdatePlayerModel()
    {
        RotatePlayer();
    }

    // Simulacion de FixedUpdate
    void FixedUpdatePlayerModel()
    {
        Movement();
        CheckIsOnFloor();
    }

    void OnDestroy()
    {
        UnsuscribeToUpdateManagerEvents();
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
            AudioManager.Instance.PlaySound(SoundEffect.Jump);
            rb.velocity = new Vector2(rb.velocity.x, 0f);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }

    [PunRPC]
    public void GetDamage(int damage)
    {
        if (!photonView.IsMine) return;

        health -= damage;

        if (health < minHealth)
        {
            AudioManager.Instance.PlaySound(SoundEffect.Death);
            fillImage.gameObject.SetActive(false);
            PhotonNetwork.Destroy(boomerangController.gameObject);
            PhotonNetwork.Destroy(gameObject);
        }

        photonView.RPC("UpdateHealthBar", RpcTarget.All, health);
    }

    [PunRPC]
    public void UpdateHealthBar(int newHealth)
    {
        health = newHealth;
        healthBar.value = health;
    }


    private void SuscribeToUpdateManagerEvents()
    {
        UpdateManager.OnUpdate += UpdatePlayerModel;
        UpdateManager.OnFixedUpdate += FixedUpdatePlayerModel;
    }

    private void UnsuscribeToUpdateManagerEvents()
    {
        UpdateManager.OnUpdate -= UpdatePlayerModel;
        UpdateManager.OnFixedUpdate -= FixedUpdatePlayerModel;
    }

    private void GetComponents()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        sprite = GetComponentInChildren<SpriteRenderer>();
        animator = GetComponentInChildren<Animator>();
        healthBar = GetComponentInChildren<Slider>();
        fillImage = healthBar.fillRect.GetComponent<Image>();
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

    private void InitializeHealthAndHealthBar()
    {
        health = startingHealth;
        healthBar.maxValue = startingHealth;
        healthBar.value = startingHealth;

        if (photonView.IsMine)
        {
            fillImage.color = mySliderColorView;
        }

        else
        {
            fillImage.color = othrsSliderColorView;
        }
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
            if (!acceptingInput) return;

            Vector2 move = PlayerInputsManager.Instance.GetMoveAxis();
            rb.velocity = new Vector2(move.normalized.x * speed, rb.velocity.y);
        }

        animator.SetFloat("velocity", Mathf.Abs(rb.velocity.x));
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
