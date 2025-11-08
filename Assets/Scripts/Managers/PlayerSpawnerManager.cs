using UnityEngine;
using System.Collections.Generic;
using Photon.Pun;

public class PlayerSpawnerManager : MonoBehaviour
{
    private List<Transform> spawnPositions = new List<Transform>();


    void Awake()
    {
        GetComponents();
        SpawnPlayer();
    }


    private void GetComponents()
    {
        foreach (Transform child in transform)
        {
            spawnPositions.Add(child);
        }
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
