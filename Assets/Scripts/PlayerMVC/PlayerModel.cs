using UnityEngine;
using Photon.Pun;
using System;

public class PlayerModel : MonoBehaviourPun
{
    private Rigidbody2D rb;
    private SpriteRenderer sprite;

    private static event Action onInteract;

    [SerializeField] private float speed = 10f;

    public static Action OnInteract { get => onInteract; set => onInteract = value; }


    void Awake()
    {
        GetComponents();
        InitializeSkin();
    }

    void FixedUpdate()
    {
        Movement();
    }


    public void Interact()
    {
        if (photonView.IsMine)
        {
            onInteract?.Invoke();
        }
    }


    private void GetComponents()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
    }

    private void InitializeSkin()
    {
        if (photonView.Owner.CustomProperties.ContainsKey("SkinIndex"))
        {
            int skinIndex = (int)photonView.Owner.CustomProperties["SkinIndex"];
            sprite.color = PlayerSkinManager.Instance.PlayerSkins[skinIndex];
        }
    }

    private void Movement()
    {
        if (photonView.IsMine)
        {
            Vector2 move = new Vector2(PlayerInputsManager.Instance.GetMoveAxis().x, PlayerInputsManager.Instance.GetMoveAxis().y);
            rb.velocity = new Vector2(move.normalized.x * speed * Time.fixedDeltaTime, move.normalized.y * speed * Time.fixedDeltaTime);
        }
    }
}
