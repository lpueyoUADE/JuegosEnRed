using UnityEngine;
using TMPro;
using Photon.Realtime;
using System.Collections.Generic;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private List<GameObject> buttonsMainMenu;
    [SerializeField] private TMP_InputField nickNameInputField;

    [Header("CreateRoomInformation:")]
    [SerializeField] private GameObject createRoomPanel;
    [SerializeField] private TMP_InputField createRoomNameInputField;
    [SerializeField] private TMP_InputField createRoomPasswordInputField;
    [SerializeField] private TMP_Text errorTextCreateRoom;

    [Header("JoinRoomInformation:")]
    [SerializeField] private GameObject joinRoomPanel;
    [SerializeField] private TMP_InputField joinRoomNameInputFiel;
    [SerializeField] private TMP_InputField joinRoomPasswordInputField;
    [SerializeField] private TMP_Text errorTextJoinRoom;


    void Awake()
    {
        SuscribeToPhotonNetworkManagerEvents();
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
        foreach (var button in buttonsMainMenu)
        {
            button.SetActive(false);
        }
        createRoomPanel.SetActive(true);
        nickNameInputField.gameObject.SetActive(true);
    }

    public void ButtonShowJoinRoomPanel()
    {
        foreach (var button in buttonsMainMenu)
        {
            button.SetActive(false);
        }
        joinRoomPanel.SetActive(true);
        nickNameInputField.gameObject.SetActive(true);
    }

    public void ButtonCreateRoom()
    {
        string nickName = nickNameInputField.text;
        string roomName = createRoomNameInputField.text;
        string roomPassword = createRoomPasswordInputField.text;

        if (nickName.Length < 3 || nickName.Length > 12)
        {
            errorTextCreateRoom.text = "The nick name must be between 3 and 12 characters.";
            return;
        }

        if (roomName.Length < 3 || roomName.Length > 12)
        {
            errorTextCreateRoom.text = "The room name must be between 3 and 12 characters.";
            return;
        }

        if (roomPassword.Length < 3 || roomPassword.Length > 12)
        {
            errorTextCreateRoom.text = "The password must be between 3 and 12 characters.";
            return;
        }

        errorTextCreateRoom.text = string.Empty;

        PhotonNetworkManager.Instance.CreateRoom(roomName, roomPassword);
        PhotonNetworkManager.Instance.SetNickName(nickName);
    }

    public void ButtonJoinRoom()
    {
        string nickName = nickNameInputField.text;
        string roomName = joinRoomNameInputFiel.text;
        string roomPassword = joinRoomPasswordInputField.text;

        if (nickName.Length < 3 || nickName.Length > 12)
        {
            errorTextCreateRoom.text = "The nick name must be between 3 and 12 characters.";
            return;
        }

        if (roomName.Length < 3 || roomName.Length > 12)
        {
            errorTextJoinRoom.text = "The room name must be between 3 and 12 characters.";
            return;
        }

        if (roomPassword.Length < 3 || roomPassword.Length > 12)
        {
            errorTextJoinRoom.text = "The password must be between 3 and 12 characters.";
            return;
        }

        errorTextJoinRoom.text = string.Empty;

        PhotonNetworkManager.Instance.JoinRoom(roomName, roomPassword);
        PhotonNetworkManager.Instance.SetNickName(nickName);
    }

    public void ButtonSettings()
    {

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
            errorTextCreateRoom.text = "There is already a room with that name.";
        }
    }

    private void OnShowErrorWhileJoiningRoom(short returnCode)
    {
        switch (returnCode)
        {
            case ErrorCode.GameDoesNotExist:
                errorTextJoinRoom.text = "The room name or password are incorrect";
                    break;

            case ErrorCode.GameFull:
                errorTextJoinRoom.text = "The room is full";
                break;
        }
    }

    private void InitializeInputFieldsCharactersLimits()
    {
        createRoomNameInputField.characterLimit = 12;
        createRoomPasswordInputField.characterLimit = 12;

        joinRoomNameInputFiel.characterLimit = 12;
        joinRoomPasswordInputField.characterLimit = 12;
    }

    private void CleanAllInformation()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.JoystickButton2))
        {
            foreach (var button in buttonsMainMenu)
            {
                button.SetActive(true);
            }

            nickNameInputField.text = string.Empty;
            nickNameInputField.gameObject.SetActive(false);

            createRoomPanel.SetActive(false);
            createRoomNameInputField.text = string.Empty;
            createRoomPasswordInputField.text = string.Empty;
            errorTextCreateRoom.text = string.Empty;

            joinRoomPanel.SetActive(false);
            joinRoomNameInputFiel.text = string.Empty;
            joinRoomPasswordInputField.text = string.Empty;
            errorTextJoinRoom.text = string.Empty;
        }
    }
}
