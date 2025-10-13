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
        int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
        int index = (actorNumber - 1) % spawnPositions.Count;

        PhotonNetwork.Instantiate("Prefabs/Player/Player", spawnPositions[index].position, Quaternion.identity);
    }
}
