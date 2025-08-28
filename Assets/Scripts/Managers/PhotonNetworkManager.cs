using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using System;
using ExitGames.Client.Photon;

public class PhotonNetworkManager : SingletonMonoBehaviourPunCallbacks<PhotonNetworkManager>
{
    private event Action onConnectedToMasterEvent;
    private event Action onJoinedRoomEvent;
    //private event Action onLeftRoomEvent;
    private event Action onPlayerEnteredRoomEvent;
    private event Action onPlayerLeftRoomEvent;
    private event Action<short> onCreateRoomFailedEvent;
    private event Action<short> onJoinRoomFailedEvent;
    private event Action<Player, Hashtable> onPlayerPropertiesUpdateEvent;

    public Action OnConnectedToMasterEvent { get => onConnectedToMasterEvent; set => onConnectedToMasterEvent = value; }
    public Action OnJoinedRoomEvent { get => onJoinedRoomEvent; set => onJoinedRoomEvent = value; }
    //public Action OnLeftRoomEvent { get => onLeftRoomEvent; set => onLeftRoomEvent = value; }
    public Action OnPlayerEnteredRoomEvent { get => onPlayerEnteredRoomEvent; set => onPlayerEnteredRoomEvent = value; }
    public Action OnPlayerLeftRoomEvent { get => onPlayerLeftRoomEvent; set => onPlayerLeftRoomEvent = value; }
    public Action<short> OnCreateRoomFailedEvent { get => onCreateRoomFailedEvent; set => onCreateRoomFailedEvent = value; }
    public Action<short> OnJoinRoomFailedEvent { get => onJoinRoomFailedEvent; set => onJoinRoomFailedEvent = value; }
    public Action<Player, Hashtable> OnPlayerPropertiesUpdateEvent { get => onPlayerPropertiesUpdateEvent; set => onPlayerPropertiesUpdateEvent = value; }

    public bool IsHost { get => PhotonNetwork.IsMasterClient; }


    void Awake()
    {
        CreateSingleton(true);
    }

    void Start()
    {
        InitializePhotonSettings();
    }


    // Se ejecute cuando se inicializa las settings en el awake
    public override void OnConnectedToMaster()
    {
        Debug.Log("Conectado al servidor de Photon");
        onConnectedToMasterEvent?.Invoke();

        PhotonNetwork.AutomaticallySyncScene = true;
    }

    // Se ejecute cuando se une alguien a una room porque la creo o porque se unio
    public override void OnJoinedRoom()
    {
        Debug.Log("Entro a una room");
        StartCoroutine(ExucuteOnJoinedRoomCallback());
    }

    // Se ejecute cuando alguien abandona una room
    public override void OnLeftRoom()
    {
        Debug.Log("Abandono la room");
        StartCoroutine(ExecuteOnLeftRoomCallback());
    }

    // Se ejecuta en todas las instancias cuando alguien se une a una room
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"El jugador {newPlayer.NickName} se unió a la room");
        onPlayerEnteredRoomEvent?.Invoke();
    }

    // Se ejecuta en todas las instancias cuando alguien abandona una room
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log($"El jugador {otherPlayer.NickName} se fue de la room");
        onPlayerLeftRoomEvent?.Invoke();
    }

    // Se ejecuta cuando no se puede crear una room
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("No se pudo crear la room");
        onCreateRoomFailedEvent?.Invoke(returnCode);
    }

    // Se ejecuta cuando no se puede unir a una room
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("No se pudo unir a la room");
        onJoinRoomFailedEvent?.Invoke(returnCode);
    }

    // Se ejecuta en todas las instancias cuando alguien modifica una propiedad personal
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        Debug.Log($"El jugador {targetPlayer.NickName} cambio sus propiedades");
        onPlayerPropertiesUpdateEvent?.Invoke(targetPlayer, changedProps);
    }


    public void SetNickName(string nickName)
    {
        PhotonNetwork.NickName = nickName;
    }

    public void CreateRoom(string roomName, string password)
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 4;

        string roomId = $"{roomName}_{password}";
        PhotonNetwork.CreateRoom(roomId, roomOptions);
    }

    public void JoinRoom(string roomName, string password)
    {
        string roomId = $"{roomName}_{password}";
        PhotonNetwork.JoinRoom(roomId);
    }

    public void CloseRoom()
    {
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.CurrentRoom.IsVisible = false;
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public int GetCurrentPlayersCountInRoom()
    {
        if (PhotonNetwork.CurrentRoom != null)
        {
            return PhotonNetwork.CurrentRoom.PlayerCount;
        }

        return 0;
    }


    private void InitializePhotonSettings()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    private System.Collections.IEnumerator ExucuteOnJoinedRoomCallback()
    {
        ScenesManager.Instance.LoadScene("Room");

        yield return new WaitForSecondsRealtime(1);

        onJoinedRoomEvent?.Invoke();
    }

    private System.Collections.IEnumerator ExecuteOnLeftRoomCallback()
    {
        ScenesManager.Instance.LoadScene("MainMenu");

        yield return new WaitForSecondsRealtime(1);

        //onLeftRoomEvent?.Invoke();
    }
}
