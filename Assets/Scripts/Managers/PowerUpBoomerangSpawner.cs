using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpBoomerangSpawner : MonoBehaviour
{
    private List<GameObject> powerUpPrefabs = new List<GameObject>();
    private List<Transform> spawnPositions = new List<Transform>();

    [SerializeField] private float timeToSpawnNewBoomerang;

    private float counterSpawnBoomerang;


    void Awake()
    {
        GetComponents();
    }

    void Update()
    {
        SpawnRandomPowerUp();
    }


    private void GetComponents()
    {
        GameObject[] loadedPrefabs = Resources.LoadAll<GameObject>("Prefabs/PowerUpBoomerangs");
        powerUpPrefabs.AddRange(loadedPrefabs);

        foreach (Transform child in transform)
        {
            spawnPositions.Add(child);
        }
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
            //timeToSpawnNewBoomerang *= 1.5f;
        }
    }
}
