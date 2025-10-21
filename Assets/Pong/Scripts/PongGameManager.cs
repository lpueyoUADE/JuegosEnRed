using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
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
    }

    void Start()
    {
        SpawnPlayer();

        if (PhotonNetwork.IsMasterClient)
        {
            SpawnBall();
        }
    }
    [PunRPC]
    private void RPC_EndGame(string winner)
    {
        Debug.Log($"El equipo {winner} ganó la partida!");
        PlayerPrefs.SetString("WinnerTeam", winner);
        PlayerPrefs.SetString("Points", $"{scoreLeft} | {scoreRight}");
        PhotonNetwork.LoadLevel(winSceneName);
    }

    private void CheckForWinner()
    {
        if (scoreLeft >= maxScoreToWin)
            photonView.RPC(nameof(RPC_EndGame), RpcTarget.All, "Left");
        else if (scoreRight >= maxScoreToWin)
            photonView.RPC(nameof(RPC_EndGame), RpcTarget.All, "Right");
    }

    public void AddPointToLeft()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        scoreLeft++;
        BroadcastScore();
        CheckForWinner();
        ResetBall(toRight: false);
    }

    public void AddPointToRight()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        scoreRight++;
        BroadcastScore();
        CheckForWinner();
        ResetBall(toRight: true);
    }

    private void BroadcastScore()
    {
        photonView.RPC(nameof(RPC_UpdateScore), RpcTarget.All, scoreLeft, scoreRight);
    }

    [PunRPC]
    private void RPC_UpdateScore(int left, int right)
    {
        scoreLeft = left;
        scoreRight = right;
        OnScoreChanged?.Invoke(scoreLeft, scoreRight);
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
        bool toRight = UnityEngine.Random.value < 0.5f;
        ball.LaunchInDirection(toRight);
    }

    public void SpawnPlayer()
    {
        int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
        int actorIndex = actorNumber - 1;
        bool isLeftTeam = actorIndex % 2 == 0;

        Transform spawnPoint = GetSpawnPoint(actorIndex, isLeftTeam);
        if (spawnPoint == null)
            Debug.LogWarning("No hay spawn point disponible para este jugador, usando posición (0,0)");

        GameObject paddleObj = PhotonNetwork.Instantiate("Prefabs/Pong/" + paddlePrefab.name, spawnPoint.position, Quaternion.identity);
        PaddleController controller = paddleObj.GetComponent<PaddleController>();
        controller.SetBounds(bounds);

        Debug.Log($"Jugador {PhotonNetwork.LocalPlayer.NickName} spawn en {(isLeftTeam ? "IZQUIERDA" : "DERECHA")}");
    }

    private Transform GetSpawnPoint(int index, bool isLeftTeam)
    {
        if (isLeftTeam)
            return (index / 2) % 2 == 0 ? leftSpawn1 : leftSpawn2;
        else
            return (index / 2) % 2 == 0 ? rightSpawn1 : rightSpawn2;
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
