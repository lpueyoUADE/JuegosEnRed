using UnityEngine;
using System.Collections.Generic;
using Photon.Pun;

public class PlayerSpawnerManager : MonoBehaviour
{
    [SerializeField] private List<Transform> spawnPositions;


    void Awake()
    {
        InstantiatePlayerRandomSpawnPosition();
    }


    private void InstantiatePlayerRandomSpawnPosition()
    {
        PhotonNetwork.Instantiate("Prefabs/Player/Player", spawnPositions[0].transform.position, Quaternion.identity);
    }
}
