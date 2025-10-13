using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpBoomerangSpawner : MonoBehaviour
{
    [SerializeField] private List<GameObject> powerUpPrefabs;
    [SerializeField] private List<Transform> spawnPositions;

    [SerializeField] private float timeToSpawnNewBoomerang;

    private float counterSpawnBoomerang;


    void Update()
    {
        SpawnRandomPowerUp();
    }


    private void SpawnRandomPowerUp()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        counterSpawnBoomerang += Time.deltaTime;

        if (counterSpawnBoomerang >= timeToSpawnNewBoomerang)
        {
            int randomSpawnPosition = Random.Range(0, spawnPositions.Count);
            int randomPowerUp = Random.Range(0, powerUpPrefabs.Count);

            PhotonNetwork.Instantiate("Prefabs/PowerUpBoomerangs/" + powerUpPrefabs[randomPowerUp].name, spawnPositions[randomSpawnPosition].position, Quaternion.identity);
            counterSpawnBoomerang = 0;
            timeToSpawnNewBoomerang *= 1.5f;
        }
    }
}
