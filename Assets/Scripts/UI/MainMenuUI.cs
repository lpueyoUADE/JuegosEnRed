using UnityEngine;
using TMPro;
using Photon.Realtime;
using System.Collections.Generic;

public class MainMenuUI : MonoBehaviour
{
    [Header("Main Menu")]
    [SerializeField] private GameObject mainMenuPanel;

    [Header("Create Room")]
    [SerializeField] private ConnectionToRoomPanel createRoomPanel;

    [Header("Join Room")]
    [SerializeField] private ConnectionToRoomPanel joinRoomPanel;

    [Header("Settings")]
    [SerializeField] private GameObject settingsPanel;

    [Header("Credits")]
    [SerializeField] private GameObject creditsPanel;
    void Awake()
    {
        SuscribeToPhotonNetworkManagerEvents();
    }

    public enum UIPanel {
        None,
        Main,
        CreateRoom,
        JoinRoom,
        Settings,
        Credits
    }



    private void SetPanels(UIPanel panelToActivate)
    {
        mainMenuPanel.SetActive(false);
        createRoomPanel.gameObject.SetActive(false);
        joinRoomPanel.gameObject.SetActive(false);
        settingsPanel.SetActive(false);
        creditsPanel.SetActive(false);

        switch (panelToActivate)
        {
            case UIPanel.Main:
                mainMenuPanel.SetActive(true);
                break;

            case UIPanel.CreateRoom:
                createRoomPanel.gameObject.SetActive(true);
                break;

            case UIPanel.JoinRoom:
                joinRoomPanel.gameObject.SetActive(true);
                break;

            case UIPanel.Settings:
                settingsPanel.SetActive(true);
                break;

            case UIPanel.Credits:
                creditsPanel.SetActive(true);
                break;
        }
    }

    private void Start()
    {
        SetPanels(UIPanel.None);

        AudioManager.Instance.PlayMusic(MusicTrack.MainMenu);

        HybridCursorManager.Instance.SetUIPointer();
    }
    void Update()
    {
        // Test para crear una room rapida automaticamente
        if (Input.GetKeyDown(KeyCode.T) && !createRoomPanel.gameObject.activeSelf)
        {
            string nickName = "Kong777";
            string roomName = "asd";
            string roomPassword = "asd";

            PhotonNetworkManager.Instance.CreateRoom(roomName, roomPassword);
            PhotonNetworkManager.Instance.SetNickName(nickName);
        }

        // Test para unirse una room rapida automaticamente
        if (Input.GetKeyDown(KeyCode.Y))
        {
            string nickName = "WUKong1991";
            string roomName = "asd";
            string roomPassword = "asd";

            PhotonNetworkManager.Instance.JoinRoom(roomName, roomPassword);
            PhotonNetworkManager.Instance.SetNickName(nickName);
        }

        CleanAllInformation();
    }

    void OnDestroy()
    {
        UnsuscribeToPhotonNetworkManagerEvents();
    }

    // Funciones asignadas a botones de la UI
    public void ButtonShowCreateRoomPanel()
    {
        SetPanels(UIPanel.CreateRoom);
    }

    public void ButtonShowJoinRoomPanel()
    {
        SetPanels(UIPanel.JoinRoom);
    }

    public void ButtonCreateRoom()
    {
        bool result = createRoomPanel.ValidateConnectionRoom();

        if (!result)
        {
            return;
        }

        PhotonNetworkManager.Instance.CreateRoom(createRoomPanel.roomName.text, createRoomPanel.password.text);
        PhotonNetworkManager.Instance.SetNickName(createRoomPanel.nickname.text);
    }

    public void ButtonJoinRoom()
    {
        bool result = joinRoomPanel.ValidateConnectionRoom();

        if (!result)
        {
            return;
        }

        PhotonNetworkManager.Instance.JoinRoom(joinRoomPanel.roomName.text, joinRoomPanel.password.text);
        PhotonNetworkManager.Instance.SetNickName(joinRoomPanel.nickname.text);
    }

    public void ButtonSettings()
    {
        SetPanels(UIPanel.Settings);
    }

    public void ButtonCredits()
    {
        SetPanels(UIPanel.Credits);
    }

    public void ButtonExitGame()
    {
        StartCoroutine(ScenesManager.Instance.ExitGame());
    }


    private void SuscribeToPhotonNetworkManagerEvents()
    {
        PhotonNetworkManager.Instance.OnConnectedToMasterEvent += OnConnectedToMasterEvent;
        PhotonNetworkManager.Instance.OnCreateRoomFailedEvent += OnShowErrorWhileCreatingRoom;
        PhotonNetworkManager.Instance.OnJoinRoomFailedEvent += OnShowErrorWhileJoiningRoom;
    }

    private void UnsuscribeToPhotonNetworkManagerEvents()
    {
        PhotonNetworkManager.Instance.OnConnectedToMasterEvent -= OnConnectedToMasterEvent;
        PhotonNetworkManager.Instance.OnCreateRoomFailedEvent -= OnShowErrorWhileCreatingRoom;
        PhotonNetworkManager.Instance.OnJoinRoomFailedEvent -= OnShowErrorWhileJoiningRoom;
    }

    private void OnConnectedToMasterEvent()
    {
        SetPanels(UIPanel.Main);
    }

    private void OnShowErrorWhileCreatingRoom(short returnCode)
    {
        if (returnCode == ErrorCode.GameIdAlreadyExists)
        {
            createRoomPanel.errorMessage.text = "There is already a room with that name.";
        }
    }

    private void OnShowErrorWhileJoiningRoom(short returnCode)
    {
        switch (returnCode)
        {
            case ErrorCode.GameDoesNotExist:
                joinRoomPanel.errorMessage.text = "The room name or password are incorrect";
                    break;

            case ErrorCode.GameFull:
                joinRoomPanel.errorMessage.text = "The room is full";
                break;
        }
    }

    private void CleanAllInformation()
    {
        if (PlayerInputsManager.Instance.BackUI())
        {
            SetPanels(UIPanel.Main);

            createRoomPanel.CleanData();
            joinRoomPanel.CleanData();
        }
    }
}
