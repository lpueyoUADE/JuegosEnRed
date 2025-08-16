using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using UnityEngine;

public class PhotonNetworkManager : MonoBehaviourPunCallbacks
{
    private static PhotonNetworkManager instance;

    private string inputPassword;


    public static PhotonNetworkManager Instance { get => instance; }


    void Awake()
    {
        CreateSingleton();
        InitializePhoton();
    }


    // Se ejecute cuando se inicializa las settings en el awake
    public override void OnConnectedToMaster()
    {
        Debug.Log("Conectado al servidor de Photon");

        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Entró al lobby");
    }

    // Se ejecute cuando se une alguien a una room porque la creo o porque se unio
    public override void OnJoinedRoom()
    {
        Debug.Log("Entró a la sala");

        JoinRoomAsNoHost();
        JoinRoomAsHost();
    }


    public void CreateRoom(string roomName, string password)
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 4;

        Hashtable customProperties = new Hashtable();
        customProperties["password"] = password;
        roomOptions.CustomRoomProperties = customProperties;

        PhotonNetwork.CreateRoom(roomName, roomOptions);
    }

    public void JoinRoom(string roomName, string password)
    {
        inputPassword = password;
        PhotonNetwork.JoinRoom(roomName);
    }


    private void CreateSingleton()
    {
        if (instance == null)
        {
            instance = this;
        }

        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    private void InitializePhoton()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    private void JoinRoomAsNoHost()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("password", out object actualPassword))
            {
                if ((string)actualPassword != inputPassword)
                {
                    Debug.Log("Contraseña incorrecta, saliendo...");
                    PhotonNetwork.LeaveRoom();
                    return;
                }
            }
        }
    }

    private void JoinRoomAsHost()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("Room");
        }
    }
}
