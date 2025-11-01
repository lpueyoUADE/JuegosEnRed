using UnityEngine;
using System.Collections.Generic;
using Photon.Pun;

public class PlayerSpawnerManager : MonoBehaviour
{
    [SerializeField] private List<Transform> spawnPositions;


    void Awake()
    {
        SpawnPlayer();
    }


    private void SpawnPlayer()
    {
        var players = PhotonNetwork.PlayerList; // Jugadores ordenados por Join
        int index = System.Array.IndexOf(players, PhotonNetwork.LocalPlayer);

        GameObject go = PhotonNetwork.Instantiate("Prefabs/Player/Player", spawnPositions[index % spawnPositions.Count].position, Quaternion.identity);
        PlayerView playerView = go.GetComponent<PlayerView>();
        playerView.SpawnIndex = index;
    }
}
