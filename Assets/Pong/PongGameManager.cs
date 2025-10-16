using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PongGameManager : MonoBehaviourPunCallbacks
{
    public static PongGameManager Instance { get; private set; }

    [Header("Prefabs y Spawn Points")]
    public GameObject paddlePrefab;
    public GameObject ballPrefab;
    public Transform spawnBallPoint;

    [Header("Posiciones de spawn por equipo")]
    public Transform leftSpawn1;
    public Transform leftSpawn2;
    public Transform rightSpawn1;
    public Transform rightSpawn2;

    [Header("Bounds del juego")]
    public BoxCollider2D bounds;

    [Header("Configuración del juego")]
    public int maxScoreToWin = 5;
    public string winSceneName;

    private PongBall ball;

    private int scoreLeft;
    private int scoreRight;

    public event Action<int, int> OnScoreChanged;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        SpawnPlayer();

        if (PhotonNetwork.IsMasterClient)
        {
            SpawnBall();
            bool toRight = UnityEngine.Random.value < 0.5f;
            ball.LaunchInDirection(toRight);
        }
    }
    [PunRPC]
    private void RPC_EndGame(string winner)
    {
        Debug.Log($"El equipo {winner} ganó la partida!");
        PhotonNetwork.LoadLevel(winSceneName);
        PlayerPrefs.SetString("WinnerTeam", winner);
        PlayerPrefs.SetString("Points", $"{scoreLeft} | {scoreRight}");
    }

    private void CheckForWinner()
    {
        if (scoreLeft >= maxScoreToWin)
        {
            photonView.RPC(nameof(RPC_EndGame), RpcTarget.All, "Left");
        }
        else if (scoreRight >= maxScoreToWin)
        {
            photonView.RPC(nameof(RPC_EndGame), RpcTarget.All, "Right");
        }
    }

    public void AddPointToLeft()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        scoreLeft++;
        Debug.Log("Puntaje Izquierdo: " + scoreLeft);
        BroadcastScore();
        CheckForWinner();
        ResetBall(toRight: false);
    }

    public void AddPointToRight()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        scoreRight++;
        Debug.Log("Puntaje Derecho: " + scoreRight);
        BroadcastScore();
        CheckForWinner();
        ResetBall(toRight: true);
    }


    public void ResetBall(bool toRight)
    {
        ball.SetPosition(spawnBallPoint.position);
        ball.LaunchInDirection(toRight);
    }

    void SpawnBall()
    {
        GameObject go = PhotonNetwork.Instantiate("Prefabs/Pong/" + ballPrefab.name, spawnBallPoint.position, Quaternion.identity);
        ball = go.GetComponent<PongBall>();
    }

    public void SpawnPlayer()
    {
        int actorIndex = PhotonNetwork.LocalPlayer.ActorNumber - 1;
        bool isLeftTeam = actorIndex % 2 == 0;

        Transform spawnPoint = GetSpawnPoint(actorIndex, isLeftTeam);
        if (spawnPoint == null)
        {
            Debug.LogWarning("No hay spawn point disponible para este jugador, usando posición (0,0)");
        }

        GameObject paddle = PhotonNetwork.Instantiate("Prefabs/Pong/" + paddlePrefab.name, spawnPoint.position, Quaternion.identity);
        paddle.GetComponent<PaddleController>().SetBounds(bounds);

        Debug.Log($"Jugador {PhotonNetwork.LocalPlayer.NickName} spawn en {(isLeftTeam ? "IZQUIERDA" : "DERECHA")}");
    }

    private Transform GetSpawnPoint(int index, bool isLeftTeam)
    {
        if (isLeftTeam)
        {
            return (index / 2) % 2 == 0 ? leftSpawn1 : leftSpawn2;
        }
        else
        {
            return (index / 2) % 2 == 0 ? rightSpawn1 : rightSpawn2;
        }
    }
    // Envía el puntaje actual a todos
    private void BroadcastScore()
    {
        photonView.RPC(nameof(RPC_UpdateScore), RpcTarget.All, scoreLeft, scoreRight);
    }

    // Recibe y aplica el puntaje
    [PunRPC]
    private void RPC_UpdateScore(int left, int right)
    {
        scoreLeft = left;
        scoreRight = right;
        OnScoreChanged?.Invoke(scoreLeft, scoreRight);
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);

        // Solo el Master envía el puntaje actual al nuevo jugador
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC(nameof(RPC_UpdateScore), newPlayer, scoreLeft, scoreRight);
        }
    }
}