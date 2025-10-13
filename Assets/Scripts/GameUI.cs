using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUI : MonoBehaviour
{
    [SerializeField] private GameObject panelBackToMainMenu;

    private static event Action onPlayerLeaveRoomFrameEarlier;
    public static event Action<bool> OnSetMainMenuState;

    public static Action OnPlayerLeaveRoomFrameEarlier { get => onPlayerLeaveRoomFrameEarlier; set => onPlayerLeaveRoomFrameEarlier = value; }


    void Start()
    {
        AudioManager.Instance.PlayMusic(MusicTrack.Gameplay);
        HybridCursorManager.Instance.SetBattlePointer();
    }

    void Update()
    {
        ShowOrHidePanelToGoBackToMainMenu();
    }


    public void ButtonYes()
    {
        onPlayerLeaveRoomFrameEarlier?.Invoke();
        PhotonNetworkManager.Instance.LeaveRoom();
    }

    public void ButtonNo()
    {
        panelBackToMainMenu.SetActive(false);
        OnSetMainMenuState?.Invoke(true);
        HybridCursorManager.Instance.SetBattlePointer();
    }


    private void ShowOrHidePanelToGoBackToMainMenu()
    {
        if (ScenesManager.Instance.IsInLoadingScenePanel) return;

        if (PlayerInputsManager.Instance.BackUI())
        {
            if (panelBackToMainMenu.activeSelf)
            {
                panelBackToMainMenu.SetActive(false);
                OnSetMainMenuState?.Invoke(true);
                HybridCursorManager.Instance.SetBattlePointer();
            }
            
            else
            {
                panelBackToMainMenu.SetActive(true);
                OnSetMainMenuState?.Invoke(false);
                HybridCursorManager.Instance.SetUIPointer();
            }
        }
    }
}
