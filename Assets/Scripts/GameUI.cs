using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUI : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject panelBackToMainMenu;
    [SerializeField] private GameObject podiumPanel;

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
        if (ScenesManager.Instance.IsInLoadingScenePanel) return;
        ShowOrHidePanelToGoBackToMainMenu();
        ShowOrHidePodiumPanel();
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
                podiumPanel.SetActive(false);
                OnSetMainMenuState?.Invoke(false);
                HybridCursorManager.Instance.SetUIPointer();
            }
        }
    }

    private void ShowOrHidePodiumPanel()
    {
        if (panelBackToMainMenu.activeSelf) return;

        podiumPanel.SetActive(PlayerInputsManager.Instance.TabUI());
    }
}
