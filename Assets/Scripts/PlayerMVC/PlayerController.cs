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
        SuscribeToUpdateManagerEvent();
        GetComponents();
        playerModel.AcceptingInput = true;
        GameUI.OnSetMainMenuState += SetInputState;
    }

    private void SetInputState(bool isAcceptingInput)
    {
        playerModel.AcceptingInput = isAcceptingInput;
    }

    private void OnDestroy()
    {
        GameUI.OnSetMainMenuState -= SetInputState;
        UnsuscribeToUpdateManagerEvent();
    }

    // Simulacion de Update
    void UpdatePlayerController()
    {
        CheckInputs();
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
        if (!playerModel.AcceptingInput) return;

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
