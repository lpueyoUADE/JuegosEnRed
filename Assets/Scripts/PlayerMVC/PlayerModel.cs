using UnityEngine;
using Photon.Pun;

public class PlayerModel : MonoBehaviourPun
{
    private Rigidbody2D rb;
    private SpriteRenderer sprite;

    [SerializeField] private float speed = 10f;

    void Awake()
    {
        GetComponents();
        InitializeSkin();
    }

    void FixedUpdate()
    {
        Movement();
    }

    void Update()
    {

    }


    private void GetComponents()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
    }

    private void InitializeSkin()
    {
        sprite.color = PlayerSkinManager.Instance.CurrentSelectedSkin;
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
