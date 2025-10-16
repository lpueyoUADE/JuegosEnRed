using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PongLauncher : MonoBehaviourPunCallbacks
{
    public string gameplayScene;

    void Start()
    {
        Debug.Log("Conectando a Photon...");
        PhotonNetwork.ConnectUsingSettings(); // Usa los datos del PhotonServerSettings
        PhotonNetwork.SendRate = 60;
        PhotonNetwork.SerializationRate = 30;
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Conectado al servidor maestro.");
        PhotonNetwork.JoinLobby(); // Conecta al lobby general
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Unido al lobby.");
        PhotonNetwork.JoinOrCreateRoom("SalaPong", new RoomOptions { MaxPlayers = 4 }, TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Entró en la sala: " + PhotonNetwork.CurrentRoom.Name);
        // Cargar la escena del juego una vez que está dentro
        PhotonNetwork.LoadLevel(gameplayScene);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogWarning("Desconectado de Photon: " + cause);
    }
}