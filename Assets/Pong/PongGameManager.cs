using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.VirtualTexturing.Debugging;

public class PongGameManager : MonoBehaviour
{
    public static PongGameManager Instance { get; private set; }

    [Header("Prefabs y Spawn Points")]
    public GameObject paddlePrefab;
    public Transform[] spawnPoints;

    [Header("Bounds del juego")]
    public BoxCollider2D bounds;

    void Awake()
    {
        // Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public string paddlePrefabName;
    void Start()
    {
        SpawnPlayer();
    }

    public void SpawnPlayer()
    {
        int index = PhotonNetwork.LocalPlayer.ActorNumber - 1;
        Vector3 spawnPos = spawnPoints[index % spawnPoints.Length].position;

        GameObject paddle = PhotonNetwork.Instantiate("Prefabs/Pong/" + paddlePrefab.name, spawnPos, Quaternion.identity);
        // Pasar bounds al paddle
        paddle.GetComponent<PaddleController>().SetBounds(bounds);
    }
}
