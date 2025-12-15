using Photon.Pun;
using System;
using System.Collections.Generic;
using UnityEngine;

public enum BoomerangType
{
    Default, Fast, Returnable, Damageble
}

public class BoomerangModel : MonoBehaviourPun
{
    private PhotonTransformViewClassic ptvc;
    private Rigidbody2D rb;
    private CircleCollider2D circleCollider;

    private PlayerModel ownerPlayerModel;
    private BoxCollider2D ownerPlayerCollider;

    private PlayerModel auxiliarPlayerModel;

    private Dictionary<int, float> hitCooldowns = new Dictionary<int, float>();

    private static event Action<int> onDisableSprite;
    private static event Action<int, bool> onShowTrail;

    private Vector2 currentDir;

    [SerializeField] private BoomerangType boomerangType;

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
    public static Action<int, bool> OnShowTrail { get => onShowTrail; set => onShowTrail = value; }

    public BoomerangType BoomerangType { get => boomerangType; }


    void Awake()
    {
        SuscribeToUpdateManagerEvents();
        GetComponents();
    }

    // Simulacion de Update
    void UpdateBoomerangModel()
    {
        Rotation();
        ReturnBoomerangAutoAfterSecondsIfIsCollidingWithOtherPlayerOrOtherParentPlayerDies();
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
        OnCollisionEnterWithOtherBoomerangs(collision);
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
        onShowTrail?.Invoke(photonView.ViewID, false);
        transform.position = newPosition;
        onShowTrail?.Invoke(photonView.ViewID, true);
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
        onShowTrail?.Invoke(photonView.ViewID, true);
        transform.SetParent(null, true);
        Physics2D.IgnoreCollision(circleCollider, ownerPlayerCollider, true);
    }

    [PunRPC]
    public void ReturnBoomerang()
    {
        if (isReturning) return; // Esta linea fue agregada por el Boomerang Returnable

        AudioManager.Instance.PlaySound(SoundEffect.ThrowBack);
        rotationDirection = UnityEngine.Random.value < 0.5f ? 1 : -1;
        canRotate = true;
        isReturning = true;
        rb.simulated = true;
        circleCollider.enabled = true;
        circleCollider.isTrigger = true; // Esta linea fue agregada por el Boomerang Returnable
        rb.bodyType = RigidbodyType2D.Dynamic;
        onShowTrail?.Invoke(photonView.ViewID, true);
        transform.SetParent(null, true);
        auxiliarPlayerModel = null;
        Physics2D.IgnoreCollision(circleCollider, ownerPlayerCollider, false);
    }

    [PunRPC]
    public void DisableBoomerang()
    {
        onDisableSprite?.Invoke(photonView.ViewID);
        rb.bodyType = RigidbodyType2D.Kinematic;
        onShowTrail?.Invoke(photonView.ViewID, false);
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
        ptvc = GetComponent<PhotonTransformViewClassic>();
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

        /*if (rb.velocity.magnitude < 0.01f && photonView.IsMine)
        {
            photonView.RPC("SetInterpolation", RpcTarget.All, false);
        }

        else
        {
            photonView.RPC("SetInterpolation", RpcTarget.All, true);
        }*/
    }

    private void Rotation()
    {
        if (!photonView.IsMine) return;

        if (canRotate)
        {
            transform.Rotate(0f, 0f, rotationSpeed * rotationDirection * Time.deltaTime);
        }
    }

    private void ReturnBoomerangAutoAfterSecondsIfIsCollidingWithOtherPlayerOrOtherParentPlayerDies()
    {
        if (!photonView.IsMine) return;

        if (auxiliarPlayerHitActorNumber != null)
        {
            if (auxiliarPlayerModel == null) // Significa que el player el cual tenia pegado el boomerang lo mataron, entonces debe volver
            {
                photonView.RPC("ReturnBoomerang", RpcTarget.All);
                counterBoomerangComeBackAutomatically = 0f;
                return;
            }

            counterBoomerangComeBackAutomatically += Time.deltaTime;

            if (counterBoomerangComeBackAutomatically >= timeToGetBoomerangBackIfIsCollidingWithSomePlayer) // Significa que el boomerang esta pegado a un player hace mucho tiempo
            {
                photonView.RPC("ReturnBoomerang", RpcTarget.All);
                counterBoomerangComeBackAutomatically = 0f;
                return;
            }
        }

        else
        {
            counterBoomerangComeBackAutomatically = 0f;
        }
    }

    [PunRPC]
    private void SetInterpolation(bool enable)
    {
        ptvc.m_PositionModel.InterpolateOption = enable ?
            PhotonTransformViewPositionModel.InterpolateOptions.Lerp :
            PhotonTransformViewPositionModel.InterpolateOptions.Disabled;
    }

    [PunRPC]
    private void OnBoomerangCollisionEnterWithOtherPlayers(int hitPlayerActorNr, int playerModelViewID)
    {
        currentDir = Vector2.zero;
        rb.velocity = Vector2.zero;
        rb.simulated = false;
        rb.bodyType = RigidbodyType2D.Static;
        onShowTrail?.Invoke(photonView.ViewID, false);
        canRotate = false;
        circleCollider.isTrigger = true;

        PhotonView playerModelPV = PhotonView.Find(playerModelViewID);
        PlayerModel playerModel = playerModelPV.GetComponent<PlayerModel>();

        if (playerModel.photonView.OwnerActorNr == hitPlayerActorNr)
        {
            Debug.Log(playerModel.CurrentHealth);

            if (playerModel.CurrentHealth < playerModel.MinHealth)
            {
                photonView.RPC("ReturnBoomerang", RpcTarget.All); 
                return;
            }

            auxiliarPlayerHitActorNumber = hitPlayerActorNr;
            transform.SetParent(playerModel.transform, true);
        }
    }

    [PunRPC]
    private void OnBoomerangCollisionEnterWithScenary()
    {
        currentDir = Vector2.zero;
        rb.velocity = Vector2.zero;
        AudioManager.Instance.PlaySound(SoundEffect.BananaStick);
        rb.bodyType = RigidbodyType2D.Static;
        onShowTrail?.Invoke(photonView.ViewID, false);
        canRotate = false;
        circleCollider.isTrigger = true;
        Physics2D.IgnoreCollision(circleCollider, ownerPlayerCollider, false);
    }

    [PunRPC]
    private void OnBoomerangTriggerEnterWithOwnPlayer()
    {
        currentDir = Vector2.zero;
        rb.velocity = Vector2.zero;
        Vector3 rot = transform.eulerAngles;
        rot.z = 0f;
        transform.rotation = Quaternion.Euler(rot);
        onShowTrail?.Invoke(photonView.ViewID, false);
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
            auxiliarPlayerModel = collision.gameObject.GetComponent<PlayerModel>();
            PhotonView playerPV = collision.gameObject.GetComponent<PhotonView>();

            if (playerPV.OwnerActorNr != ownerActorNumber)
            {    
                playerPV.RPC("GetDamage", playerPV.Owner, damage, ownerActorNumber, boomerangType.ToString());
                photonView.RPC("OnBoomerangCollisionEnterWithOtherPlayers", RpcTarget.All, playerPV.OwnerActorNr, playerPV.ViewID);
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

    private void OnCollisionEnterWithOtherBoomerangs(Collision2D collision)
    {
        if (!photonView.IsMine) return;

        if (collision.gameObject.layer == LayerMask.NameToLayer("Boomerang") && !isReturning)
        {
            /// Se elimino la linea que hacia que el boomerang que colisiona tambien ejecute el RPC de "ReturnBoomerang"
            PhotonView boomerangPV = collision.gameObject.GetComponent<PhotonView>();
            photonView.RPC("ReturnBoomerang", RpcTarget.All);
        }
    }

    private void OnTriggerEnterWithOwnPlayer(Collider2D collider)
    {
        if (!photonView.IsMine) return;

        if (collider.gameObject.CompareTag("Player"))
        {
            PhotonView playerPV = collider.gameObject.GetComponent<PhotonView>();

            if (playerPV.OwnerActorNr == ownerActorNumber)
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
            PhotonView playerPV = collider.GetComponent<PhotonView>();
            int targetActorNr = playerPV.OwnerActorNr;

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
                playerPV.RPC("GetDamage", playerPV.Owner, damage, ownerActorNumber, boomerangType.ToString());

                hitCooldowns[targetActorNr] = Time.time + damageCooldown;
            }
        }
    }
}