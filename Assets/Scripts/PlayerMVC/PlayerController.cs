using Photon.Pun;
using System;
using UnityEngine;

public class PlayerController : MonoBehaviourPun
{
    private PlayerModel playerModel;
    private PlayerView playerView;

    private static event Action onInteract;

    public PlayerModel PlayerModel { get => playerModel; }
    public PlayerView PlayerView { get => playerView; }

    public static Action OnInteract { get => onInteract; set => onInteract = value; }


    void Awake()
    {
        SuscribeToUpdateManagerEvent();
        GetComponents();
        playerModel.AcceptingInput = true;
        GameUI.OnSetMainMenuState += SetInputState;
    }

    // Simulacion de Update
    void UpdatePlayerController()
    {
        CheckInputs();
    }

    void OnDestroy()
    {
        GameUI.OnSetMainMenuState -= SetInputState;
        UnsuscribeToUpdateManagerEvent();
    }


    private void SetInputState(bool isAcceptingInput)
    {
        playerModel.AcceptingInput = isAcceptingInput;
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

        TestBoomerangs();
    }

    private void TestBoomerangs()
    {
        string namePrefab = Input.GetKeyDown(KeyCode.Alpha1) ? "BoomerangDefault" :
                        Input.GetKeyDown(KeyCode.Alpha2) ? "BoomerangFast" :
                        Input.GetKeyDown(KeyCode.Alpha3) ? "BoomerangReturnable" :
                        Input.GetKeyDown(KeyCode.Alpha4) ? "BoomerangDamagable" :
                        string.Empty;

        if (!string.IsNullOrEmpty(namePrefab))
        {
            GameObject boomerangGO = PhotonNetwork.Instantiate("Prefabs/Boomerangs/" + namePrefab, playerModel.BoomerangHandPosition.position, Quaternion.identity);
            BoomerangController newBoomerang = boomerangGO.GetComponent<BoomerangController>();
            newBoomerang.BoomerangModel.photonView.RPC("Initialize", RpcTarget.All, photonView.OwnerActorNr);
            playerModel.ChangeCurrentBoomerangToNewOne(newBoomerang);
        }   
    }
}
