using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUI : MonoBehaviour
{
    [SerializeField] private GameObject panelBackToMainMenu;

    public static event Action<bool> OnSetMainMenuState;
    private void Start()
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
        PhotonNetworkManager.Instance.LeaveRoom();
        //TimeManager.Instance.RestartIsCountdownFinished();
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
                OnSetMainMenuState?.Invoke(false);
                HybridCursorManager.Instance.SetUIPointer();
            }
        }
    }
}
