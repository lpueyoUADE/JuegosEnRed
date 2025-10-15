using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PongGameManager : MonoBehaviour
{
    public static PongGameManager Instance { get; private set; }

    [Header("Prefabs y Spawn Points")]
    public GameObject paddlePrefab;
    public Transform spawnBallPoint;
    public Transform[] spawnPoints;
    public GameObject ballPrefab;

    [Header("Bounds del juego")]
    public BoxCollider2D bounds;

    private PongBall ball;

    private int scoreLeft;
    private int scoreRight;

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
    public void AddPointToLeft()
    {
        scoreLeft++;
        Debug.Log("Puntaje Izquierdo: " + scoreLeft);
        ResetBall(toRight: false);
    }

    public void AddPointToRight()
    {
        scoreRight++;
        Debug.Log("Puntaje Derecho: " + scoreRight);
        ResetBall(toRight: true);
    }

    public void ResetBall(bool toRight)
    {
        ball.SetPosition(spawnBallPoint.position);
        ball.LaunchInDirection(toRight);
    }
    void Start()
    {
        SpawnPlayer();
        SpawnBall();
        if (PhotonNetwork.IsMasterClient)
        {
            bool toRight = Random.value < 0.5f; // true o false aleatorio
            ball.LaunchInDirection(toRight);
        }
    }

    void SpawnBall()
    {
        GameObject go = PhotonNetwork.Instantiate("Prefabs/Pong/" + ballPrefab.name, spawnBallPoint.position, Quaternion.identity);
        ball = go.GetComponent<PongBall>();
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
