using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

public class PongMainMenuManager : MonoBehaviourPunCallbacks
{
    public GameObject mainMenu;
    public GameObject createRoomMenu;
    public GameObject joinRoomMenu;

    public string gameplayScene;

    [Header("Create Room")]
    public TMP_InputField createRoomInput;
    public TMP_InputField createPlayerNameInput;

    [Header("Join Room")]
    public TMP_InputField joinRoomInput;
    public TMP_InputField joinPlayerNameInput;

    [Header("Loading Messages")]
    public TextMeshProUGUI loadingText;

    private enum Menu { Main, Create, Join }
    private bool isCreating = false;

    void Start()
    {
        SetActiveMenu(Menu.Main);
        PhotonNetwork.SendRate = 60;
        PhotonNetwork.SerializationRate = 30;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            ShowMainMenu();
    }

    void SetActiveMenu(Menu menu)
    {
        mainMenu.SetActive(menu == Menu.Main);
        createRoomMenu.SetActive(menu == Menu.Create);
        joinRoomMenu.SetActive(menu == Menu.Join);
    }

    public void ShowMainMenu() => SetActiveMenu(Menu.Main);
    public void CreateRoomButton() => SetActiveMenu(Menu.Create);
    public void JoinRoomButton() => SetActiveMenu(Menu.Join);

    public void CreateRoomAction()
    {
        if (string.IsNullOrEmpty(createRoomInput.text))
        {
            loadingText.text = "Ingrese un nombre de sala.";
            return;
        }

        isCreating = true;
        PhotonNetwork.NickName = createPlayerNameInput.text;
        loadingText.text = "Conectando a Photon...";

        if (!PhotonNetwork.IsConnected)
            PhotonNetwork.ConnectUsingSettings();
        else
            OnConnectedToMaster();
    }

    public void JoinRoomAction()
    {
        if (string.IsNullOrEmpty(joinRoomInput.text))
        {
            loadingText.text = "Ingrese el nombre de la sala.";
            return;
        }

        isCreating = false;
        PhotonNetwork.NickName = joinPlayerNameInput.text;
        loadingText.text = "Conectando a Photon...";

        if (!PhotonNetwork.IsConnected)
            PhotonNetwork.ConnectUsingSettings();
        else
            OnConnectedToMaster();
    }


    public override void OnConnectedToMaster()
    {
        loadingText.text = "Conectado al servidor maestro.";
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        loadingText.text = "Unido al lobby.";

        if (isCreating)
        {
            PhotonNetwork.CreateRoom(
                createRoomInput.text,
                new RoomOptions { MaxPlayers = 4 },
                TypedLobby.Default
            );
        }
        else
        {
            PhotonNetwork.JoinRoom(joinRoomInput.text);
        }
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        loadingText.text = "No se pudo crear la sala: " + message;
        SetActiveMenu(Menu.Main);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        loadingText.text = "No se pudo unir a la sala: " + message;
        SetActiveMenu(Menu.Main);
    }

    public override void OnJoinedRoom()
    {
        loadingText.text = "Entró en la sala: " + PhotonNetwork.CurrentRoom.Name;
        PhotonNetwork.LoadLevel(gameplayScene);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        loadingText.text = "Desconectado de Photon: " + cause;
    }

    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }
}
