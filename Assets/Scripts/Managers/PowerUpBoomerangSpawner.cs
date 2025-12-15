using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PowerUpBoomerangSpawner : MonoBehaviour
{
    private List<GameObject> powerUpPrefabs = new List<GameObject>();
    private List<Transform> spawnPositions = new List<Transform>();

    [SerializeField] private float timeToSpawnNewBoomerang;

    private float counterSpawnBoomerang;


    void Awake()
    {
        SuscribeToUpdateManagerEvent();
        GetComponents();
    }

    // Simulacion de Update
    void UpdatePowerUpBoomerangSpawner()
    {
        SpawnRandomPowerUp();
    }

    void OnDestroy()
    {
        UnsuscribeToUpdateManagerEvent();
    }


    private void SuscribeToUpdateManagerEvent()
    {
        UpdateManager.OnUpdate += UpdatePowerUpBoomerangSpawner;
    }

    private void UnsuscribeToUpdateManagerEvent()
    {
        UpdateManager.OnUpdate -= UpdatePowerUpBoomerangSpawner;
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

        if (!TestManager.Instance.UseTestSystem)
        {
            if (PlayersManager.Instance.CurrentPlayers.Count == 1) return; // Esto evitar que se instancien powerUps si finalizo la ronda
        }

        counterSpawnBoomerang += Time.deltaTime;

        if (counterSpawnBoomerang >= timeToSpawnNewBoomerang)
        {
            int randomSpawnPosition = UnityEngine.Random.Range(0, spawnPositions.Count);
            int randomPowerUp = UnityEngine.Random.Range(0, powerUpPrefabs.Count);

            string powerUpId = Guid.NewGuid().ToString();

            GameObject powerUpGo = PhotonNetwork.InstantiateRoomObject("Prefabs/PowerUpBoomerangs/" + powerUpPrefabs[randomPowerUp].name, spawnPositions[randomSpawnPosition].position, Quaternion.identity);
            PowerUpBoomerang powerUp = powerUpGo.GetComponent<PowerUpBoomerang>();
            powerUp.Initialize(powerUpId);
            counterSpawnBoomerang = 0;
        }
    }
}
