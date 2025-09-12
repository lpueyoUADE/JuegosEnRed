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

    void Awake()
    {
        SuscribeToPhotonNetworkManagerEvents();
    }

    private void Start()
    {
        mainMenuPanel.SetActive(true);
        settingsPanel.SetActive(false);
        createRoomPanel.gameObject.SetActive(false);
        joinRoomPanel.gameObject.SetActive(false);

        AudioManager.Instance.PlayMusic(MusicTrack.MainMenu);

        HybridCursorManager.Instance.SetUIPointer();
    }
    void Update()
    {
        // Test para crear una room rapida automaticamente
        if (Input.GetKeyDown(KeyCode.T))
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
        mainMenuPanel.SetActive(false);
        settingsPanel.SetActive(false);
        createRoomPanel.gameObject.SetActive(true);
        joinRoomPanel.gameObject.SetActive(false);
    }

    public void ButtonShowJoinRoomPanel()
    {
        mainMenuPanel.SetActive(false);
        settingsPanel.SetActive(false);
        createRoomPanel.gameObject.SetActive(false);
        joinRoomPanel.gameObject.SetActive(true);
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
        mainMenuPanel.SetActive(false);
        settingsPanel.SetActive(true);
        createRoomPanel.gameObject.SetActive(false);
        joinRoomPanel.gameObject.SetActive(false);
    }

    public void ButtonExitGame()
    {
        StartCoroutine(ScenesManager.Instance.ExitGame());
    }


    private void SuscribeToPhotonNetworkManagerEvents()
    {
        PhotonNetworkManager.Instance.OnCreateRoomFailedEvent += OnShowErrorWhileCreatingRoom;
        PhotonNetworkManager.Instance.OnJoinRoomFailedEvent += OnShowErrorWhileJoiningRoom;
    }

    private void UnsuscribeToPhotonNetworkManagerEvents()
    {
        PhotonNetworkManager.Instance.OnCreateRoomFailedEvent -= OnShowErrorWhileCreatingRoom;
        PhotonNetworkManager.Instance.OnJoinRoomFailedEvent -= OnShowErrorWhileJoiningRoom;
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
            mainMenuPanel.SetActive(true);
            settingsPanel.SetActive(false);
            createRoomPanel.gameObject.SetActive(false);
            joinRoomPanel.gameObject.SetActive(false);

            createRoomPanel.CleanData();
            joinRoomPanel.CleanData();
        }
    }
}
