using Photon.Pun;
using System;

public class PlayerController : MonoBehaviourPun
{
    private PlayerModel playerModel;
    private PlayerView playerView;

    private bool acceptingInput;

    private static event Action onInteract;

    public static Action OnInteract { get => onInteract; set => onInteract = value; }


    void Awake()
    {
        SuscribeToUpdateManagerEvent();
        GetComponents();
        acceptingInput = true;
        GameUI.OnSetMainMenuState += SetInputState;
    }

    private void SetInputState(bool isAcceptingInput)
    {
        acceptingInput = isAcceptingInput;
    }

    private void OnDestroy()
    {
        GameUI.OnSetMainMenuState -= SetInputState;
    }

    // Simulacion de Update
    void UpdatePlayerController()
    {
        CheckInputs();
    }

    void OnDestroy()
    {
        UnsuscribeToUpdateManagerEvent();
    }


    private void SuscribeToUpdateManagerEvent()
    {
        UpdateManager.OnUpdate += UpdatePlayerController;
    }

    private void UnsuscribeToUpdateManagerEvent()
    {
        UpdateManager.OnUpdate -= UpdatePlayerController;
    }

    private void GetComponents()
    {
        playerModel = GetComponent<PlayerModel>();
        playerView = GetComponent<PlayerView>();
    }

    private void CheckInputs()
    {
        if (!photonView.IsMine) return;
        if (!acceptingInput) return;

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
