using Photon.Pun;
using System;
using System.Collections;
using System.Reflection;
using UnityEngine;
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

    private Coroutine damageFlashCoroutine;

    private static event Action<int> onDisableNicknameText;
    private static event Action onPlayerDeath;
    private static event Action onPlayerWin;

    [SerializeField] private int startingHealth;

    [SerializeField] private float speed;
    [SerializeField] private float jumpForce;

    private int currentHealth;
    private int minHealth = 1;
    
    private bool isGrounded;
    private bool acceptingInput;

    public Transform BoomerangHandPosition { get => boomerangHandPosition; }

    public static Action<int> OnDisableNicknameText { get => onDisableNicknameText; set => onDisableNicknameText = value; }
    public static Action OnPlayerDeath { get => onPlayerDeath; set => onPlayerDeath = value; }
    public static Action OnPlayerWin { get => onPlayerWin; set => onPlayerWin = value; }

    public int CurrentHealth { get => currentHealth; }
    public int MinHealth { get => minHealth; }

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
            boomerangController.BoomerangModel.photonView.RPC("ThrowBoomerang", RpcTarget.All, dir);
            animator.SetTrigger("attack");
            return;
        }

        // Si el boomerang esta pegado a algun objeto del escenario
        else if (boomerangController.BoomerangModel.Rb.velocity.sqrMagnitude == 0)
        {
            boomerangController.BoomerangModel.photonView.RPC("ReturnBoomerang", RpcTarget.All);
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
            animator.SetTrigger("jump");
        }
    }

    [PunRPC]
    public void Teleport(Vector3 newPosition)
    {
        transform.position = newPosition;
    }

    [PunRPC]
    public void GetDamage(int damage)
    {
        if (!photonView.IsMine) return;

        currentHealth -= damage;
        photonView.RPC("PlaySound", RpcTarget.All, SoundEffect.HitOtherPlayers);
        photonView.RPC("UpdateHealthBar", RpcTarget.All, currentHealth);

        if (currentHealth > minHealth - 1)
        {
            photonView.RPC("DamageBlinkEffect", RpcTarget.All);
        }

        if (currentHealth < minHealth)
        {
            photonView.RPC("PlaySound", RpcTarget.All, SoundEffect.Death1);
            photonView.RPC("DisablePlayer", RpcTarget.All);
            boomerangController.BoomerangModel.photonView.RPC("DisableBoomerang", RpcTarget.All);
            StartCoroutine(DestroyPlayerAndHisBoomerang());
            PhotonNetwork.Instantiate("Prefabs/Skull/Skull",transform.position, Quaternion.identity);
        }
    }


    [PunRPC]
    private void UpdateHealthBar(int newHealth)
    {
        currentHealth = newHealth;
        healthBar.value = currentHealth;
    }

    [PunRPC]
    private void DamageBlinkEffect()
    {
        if (damageFlashCoroutine != null)
        {
            StopCoroutine(damageFlashCoroutine);
        }

        damageFlashCoroutine = StartCoroutine(BlinkEffect());
    }

    [PunRPC]
    private void PlaySound(SoundEffect soundType)
    {
        AudioManager.Instance.PlaySound(soundType);
    }

    [PunRPC]
    private void PlaySoundChoice(SoundEffect[] soundsToChoose)
    {
        AudioManager.Instance.PlaySoundChoice(soundsToChoose);
    }

    [PunRPC]
    private void DisablePlayer()
    {
        acceptingInput = false;
        onDisableNicknameText?.Invoke(photonView.ViewID); 
        fillImage?.gameObject.SetActive(false);
        healthBar?.gameObject.SetActive(false);
        sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 0f); // invisible
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.simulated = false;
        boxCollider.enabled = false;
    }

    private IEnumerator DestroyPlayerAndHisBoomerang()
    {
        yield return null;

        PhotonNetwork.Destroy(boomerangController.gameObject);
        PhotonNetwork.Destroy(gameObject);
        onPlayerDeath?.Invoke();
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
        currentHealth = startingHealth;
        healthBar.maxValue = startingHealth;
        healthBar.value = startingHealth;

        if (photonView.Owner.CustomProperties.ContainsKey("SkinIndex"))
        {
            int skinIndex = (int)photonView.Owner.CustomProperties["SkinIndex"];
            fillImage.color = PlayerSkinManager.Instance.PlayerSkins[skinIndex];
        }
    }

    private void InitializeBoomerang()
    {
        if (photonView.IsMine)
        {
            GameObject projGO = PhotonNetwork.Instantiate("Prefabs/Boomerangs/Boomerang", boomerangHandPosition.position, Quaternion.identity);
            boomerangController = projGO.GetComponent<BoomerangController>();
            boomerangController.BoomerangModel.photonView.RPC("Initialize", RpcTarget.All, photonView.OwnerActorNr);
        }
    }

    private void Movement()
    {
        if (photonView.IsMine)
        {
            if (!acceptingInput)
            {
                rb.velocity = Vector2.zero;
                return;
            }

            Vector2 move = PlayerInputsManager.Instance.GetMoveAxis();
            rb.velocity = new Vector2(move.normalized.x * speed, rb.velocity.y);
        }

        animator.SetFloat("velocity", Mathf.Abs(rb.velocity.x));
    }

    private void CheckIsOnFloor()
    {
        if (photonView.IsMine)
        {
            float extraHeight = 0.1f;
            RaycastHit2D hit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size * new Vector2(0.9f, 1f), 0f, Vector2.down, extraHeight, LayerMask.GetMask("Floor"));

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

    private IEnumerator BlinkEffect()
    {
        int executeTimesBlinkEffect = 4;

        for (int i = 0; i < executeTimesBlinkEffect; i++)
        {
            sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 0f); // invisible
            yield return new WaitForSeconds(0.1f);
            sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 1f); // visible
            yield return new WaitForSeconds(0.1f);
        }
    }
}
