using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;
using System;
using System.Collections;

public class BoomerangModel : MonoBehaviourPun
{
    /// <summary>
    /// Agregar en un futuro que si alguien tiene un boomerang pegado y le tiran otro y lo mata, que el que estaba pegado vuelva a su dueño
    /// </summary>

    private Rigidbody2D rb;
    private CircleCollider2D circleCollider;

    private PlayerModel ownerPlayerModel;
    private BoxCollider2D ownerPlayerCollider;

    private Dictionary<int, float> hitCooldowns = new Dictionary<int, float>();

    private static event Action<int> onDisableSprite;

    private Vector2 currentDir;

    [SerializeField] private int damage;

    [SerializeField] private float movementSpeed;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float timeToGetBoomerangBackIfIsCollidingWithSomePlayer;
    [SerializeField] private float damageCooldown;

    private int ownerActorNumber;
    private int rotationDirection;
    private int? auxiliarPlayerHitActorNumber;

    private float counterBoomerangComeBackAutomatically = 0f;

    private bool canRotate = false;
    private bool isReturning = false;

    public Rigidbody2D Rb { get => rb; }
    public CircleCollider2D CircleCollider { get => circleCollider; }

    public static Action<int> OnDisableSprite { get => onDisableSprite; set => onDisableSprite = value; }


    void Awake()
    {
        SuscribeToUpdateManagerEvents();
        GetComponents();
    }

    // Simulacion de Update
    void UpdateBoomerangModel()
    {
        Rotation();
        ReturnBoomerangAutomaticalyAfterSeconds();
    }

    // Simulacion de FixedUpdate
    void FixedUpdateBoomerangModel()
    {
        Movement();
    }

    void OnDestroy()
    {
        UnsuscribeToUpdateManagerEvents();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        OnCollisionEnterWithOtherPlayers(collision);
        OnCollisionEnterWithScenary(collision);
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        OnTriggerEnterWithOwnPlayer(collider);
    }

    private void OnTriggerStay2D(Collider2D collider)
    {
        OnTriggerStayWithOtherPlayers(collider);
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
        transform.SetParent(ownerPlayerModel.transform, true);
        transform.position = ownerPlayerModel.BoomerangHandPosition.position;
        ownerPlayerCollider = ownerPlayerModel.GetComponent<BoxCollider2D>();
    }

    [PunRPC]
    public void Teleport(Vector3 newPosition)
    {
        transform.position = newPosition;
    }

    [PunRPC]
    public void ThrowBoomerang(Vector2 dir)
    {
        AudioManager.Instance.PlaySoundChoice(SoundEffect.Throw1, SoundEffect.Throw2, SoundEffect.Throw3);
        currentDir = dir;
        rotationDirection = UnityEngine.Random.value < 0.5f ? 1 : -1;
        canRotate = true;
        isReturning = false;
        rb.simulated = true;
        circleCollider.enabled = true;
        rb.bodyType = RigidbodyType2D.Dynamic;
        transform.SetParent(null, true);
        Physics2D.IgnoreCollision(circleCollider, ownerPlayerCollider, true);
    }

    [PunRPC]
    public void ReturnBoomerang()
    {
        AudioManager.Instance.PlaySound(SoundEffect.ThrowBack);
        rotationDirection = UnityEngine.Random.value < 0.5f ? 1 : -1;
        canRotate = true;
        isReturning = true;
        rb.simulated = true;
        circleCollider.enabled = true;
        rb.bodyType = RigidbodyType2D.Dynamic;
        transform.SetParent(null, true);
        Physics2D.IgnoreCollision(circleCollider, ownerPlayerCollider, false);
    }

    [PunRPC]
    public void DisableBoomerang()
    {
        onDisableSprite?.Invoke(photonView.ViewID);
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.simulated = false;
        circleCollider.enabled = false;
    }


    private void SuscribeToUpdateManagerEvents()
    {
        UpdateManager.OnUpdate += UpdateBoomerangModel;
        UpdateManager.OnFixedUpdate += FixedUpdateBoomerangModel;
    }

    private void UnsuscribeToUpdateManagerEvents()
    {
        UpdateManager.OnUpdate -= UpdateBoomerangModel;
        UpdateManager.OnFixedUpdate -= FixedUpdateBoomerangModel;
    }

    private void GetComponents()
    {
        rb = GetComponent<Rigidbody2D>();
        circleCollider = GetComponent<CircleCollider2D>();
    }

    private void Movement()
    {
        if (!photonView.IsMine) return;

        if (isReturning)
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
            transform.Rotate(0f, 0f, rotationSpeed * rotationDirection * Time.deltaTime);
        }
    }

    private void ReturnBoomerangAutomaticalyAfterSeconds()
    {
        if (!photonView.IsMine) return;

        if (auxiliarPlayerHitActorNumber != null) 
        {
            counterBoomerangComeBackAutomatically += Time.deltaTime;

            if (counterBoomerangComeBackAutomatically >= timeToGetBoomerangBackIfIsCollidingWithSomePlayer)
            {
                photonView.RPC("ReturnBoomerang", RpcTarget.All);
                counterBoomerangComeBackAutomatically = 0f;
            }
        }

        else
        {
            counterBoomerangComeBackAutomatically = 0f;
        }
    }

    [PunRPC]
    private void OnBoomerangCollisionEnterWithOtherPlayers(int hitPlayerActorNr, int playerModelViewID)
    {
        currentDir = Vector2.zero;
        rb.velocity = Vector2.zero;
        rb.simulated = false;
        rb.bodyType = RigidbodyType2D.Static;
        canRotate = false;
        circleCollider.isTrigger = true;

        PhotonView playerModelPV = PhotonView.Find(playerModelViewID);
        PlayerModel playerModel = playerModelPV.GetComponent<PlayerModel>();

        if (playerModel.photonView.OwnerActorNr == hitPlayerActorNr)
        {
            if (playerModel.CurrentHealth < playerModel.MinHealth)
            {
                photonView.RPC("ReturnBoomerang", RpcTarget.All); 
                return;
            }

            StartCoroutine(SetParentWithPlayerCollision(playerModel, hitPlayerActorNr));
        }
    }

    private IEnumerator SetParentWithPlayerCollision(PlayerModel playerModel, int hitPlayerActorNr)
    {
        for (int i = 0; i < 10; i++)
        {
            yield return null;  
        }

        auxiliarPlayerHitActorNumber = hitPlayerActorNr;
        if (playerModel  != null)
        transform.SetParent(playerModel.transform, true);
    }

    [PunRPC]
    private void OnBoomerangCollisionEnterWithScenary()
    {
        AudioManager.Instance.PlaySound(SoundEffect.BananaStick);
        rb.bodyType = RigidbodyType2D.Static;
        canRotate = false;
        circleCollider.isTrigger = true;
        Physics2D.IgnoreCollision(circleCollider, ownerPlayerCollider, false);
    }

    [PunRPC]
    private void OnBoomerangTriggerEnterWithOwnPlayer()
    {
        Vector3 rot = transform.eulerAngles;
        rot.z = 0f;
        transform.rotation = Quaternion.Euler(rot); 
        transform.position = ownerPlayerModel.BoomerangHandPosition.position;
        auxiliarPlayerHitActorNumber = null;
        circleCollider.isTrigger = false;
        canRotate = false;
        isReturning = false;
        rb.simulated = false;
        circleCollider.enabled = false;
        transform.SetParent(ownerPlayerModel.transform, true);
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
                photonView.RPC("OnBoomerangCollisionEnterWithOtherPlayers", RpcTarget.All, targetPV.OwnerActorNr, targetPV.ViewID);
            }
        }
    }

    private void OnCollisionEnterWithScenary(Collision2D collision)
    {
        if (!photonView.IsMine) return;

        if (!collision.gameObject.CompareTag("Player"))
        {
            photonView.RPC("OnBoomerangCollisionEnterWithScenary", RpcTarget.All);
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
                AudioManager.Instance.PlaySound(SoundEffect.HitOwnPlayer);
                photonView.RPC("OnBoomerangTriggerEnterWithOwnPlayer", RpcTarget.All);
            }
        }
    }

    private void OnTriggerStayWithOtherPlayers(Collider2D collider)
    {
        if (!photonView.IsMine) return;

        if (collider.gameObject.CompareTag("Player"))
        {
            PhotonView targetPV = collider.GetComponent<PhotonView>();
            int targetActorNr = targetPV.OwnerActorNr;

            if (targetActorNr == ownerActorNumber) return; // Si soy yo el que choca con el boomerang terminar
            if (isReturning && targetActorNr == auxiliarPlayerHitActorNumber) return; // Si esta volviendo y choca con el que lo tenia pegado terminar

            // Si no existe en el diccionario, lo inicializamos en 0
            if (!hitCooldowns.ContainsKey(targetActorNr))
            {
                hitCooldowns[targetActorNr] = 0f;
            }

            // Chequeamos si ya pasó suficiente tiempo desde el último daño
            if (Time.time >= hitCooldowns[targetActorNr])
            {
                targetPV.RPC("GetDamage", targetPV.Owner, damage);

                hitCooldowns[targetActorNr] = Time.time + damageCooldown;
            }
        }
    }
}