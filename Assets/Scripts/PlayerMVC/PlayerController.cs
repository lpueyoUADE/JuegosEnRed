using Photon.Pun;
using System;

public class PlayerController : MonoBehaviourPun
{
    private PlayerModel playerModel;
    private PlayerView playerView;

    private static event Action onInteract;

    public static Action OnInteract { get => onInteract; set => onInteract = value; }


    void Awake()
    {
        GetComponents();
    }

    void Update()
    {
        CheckInputs();
    }


    private void GetComponents()
    {
        playerModel = GetComponent<PlayerModel>();
        playerView = GetComponent<PlayerView>();
    }

    private void CheckInputs()
    {
        if (!photonView.IsMine) return;

        
        if (PlayerInputsManager.Instance.Interact())
        {
            onInteract?.Invoke();
        }

        if (PlayerInputsManager.Instance.Attack())
        {
            playerModel.Attack();
        }

        if (PlayerInputsManager.Instance.Jump())
        {
            playerModel.Jump();
        }
    }
}
