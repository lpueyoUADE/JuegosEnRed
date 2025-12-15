using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class PlayersManager : SingletonMonoBehaviourPun<PlayersManager>
{
    [SerializeField] private List<PlayerModel> currentPlayers;

    [SerializeField] private float cameraEffectDuringTime;

    [Header("Rondas")]
    [SerializeField] private int totalRounds;
    private int currentRound = 0;

    private float currentRoundTime = 0; 

    [Header("Niveles")]
    [SerializeField] private List<GameScenes> levels;
    private List<GameScenes> currentLevels;

    public List<PlayerModel> CurrentPlayers { get => currentPlayers; }

    public int TotalRounds { get => totalRounds; }
    public int CurrentRound { get => currentRound; }


    void Awake()
    {
        CreateSingleton(true);
        SuscribeToUpdateManagerEvent();
    }

    // Simulacion de Update
    void UpdatePlayersManager()
    {
        CountCurrentRoundTime();
    }

    void Start()
    {
        InitCurrentLevels();
        SuscribeToPlayerModelEvent();
    }


    [PunRPC]
    public void RegisterPlayerForAll(int viewID)
    {
        PhotonView pv = PhotonView.Find(viewID);
        if (pv != null)
        {
            PlayerModel playerModel = pv.GetComponent<PlayerModel>();
            if (playerModel != null && !currentPlayers.Contains(playerModel))
            {
                currentPlayers.Add(playerModel);
            }
        }
    }

    [PunRPC]
    public void UnregisterPlayerForAll(int viewID)
    {
        PhotonView pv = PhotonView.Find(viewID);
        if (pv != null)
        {
            PlayerModel playerModel = pv.GetComponent<PlayerModel>();
            if (playerModel != null && currentPlayers.Contains(playerModel))
            {
                currentPlayers.Remove(playerModel);
            }
        }
    }

    public GameScenes PickAndRemoveLevel()
    {
        if (currentLevels.Count == 0)
        {
            InitCurrentLevels();
        }

        GameScenes level = PickRandomLevel();

        return level;
    }

    public void UnregisterMeForMe(PlayerModel me)
    {
        if (currentPlayers.Contains(me))
        {
            currentPlayers.Remove(me);
        }
    }


    private void SuscribeToUpdateManagerEvent()
    {
        UpdateManager.OnUpdate += UpdatePlayersManager;
    }

    private void SuscribeToPlayerModelEvent()
    {
        PlayerModel.OnPlayerDeath += OnUpdateCurrentPlayer;
    }

    private void OnUpdateCurrentPlayer()
    {
        photonView.RPC("TryChangeNextLevel", RpcTarget.All);
    }

    [PunRPC]
    private void TryChangeNextLevel()
    {
        if (currentPlayers.Count > 1) return;

        StartCoroutine(WaitSomeSecondsToChangeScene());
    }

    [PunRPC]
    private void IncreaseCurrentRoundsForAll()
    {
        currentRound++;
    }

    [PunRPC]
    private void RestartRaoundsAndLevelsForAll()
    {
        currentRoundTime = 0f;
        currentRound = 0;
        InitCurrentLevels();
    }

    private void InitCurrentLevels()
    {
        currentLevels = new List<GameScenes>(levels);
        Enum.TryParse(SceneManager.GetActiveScene().name, out GameScenes currentScene);
        currentLevels.Remove(currentScene);
    }

    private GameScenes PickRandomLevel()
    {
        int index = Random.Range(0, currentLevels.Count);
        GameScenes level = currentLevels[index];
        currentLevels.RemoveAt(index);

        return level;
    }

    private void CountCurrentRoundTime()
    {
        currentRoundTime += Time.deltaTime;
    }

    private IEnumerator WaitSomeSecondsToChangeScene()
    {
        yield return StartCoroutine(ZoomToPlayerBeforeSceneChange(cameraEffectDuringTime));

        yield return new WaitForSecondsRealtime(1.5f);

        currentPlayers.Clear();

        if (!PhotonNetwork.IsMasterClient) yield break;

        photonView.RPC("IncreaseCurrentRoundsForAll", RpcTarget.All);

        if (currentRound >= totalRounds)
        {
            AnalyticsEventsManager.Instance.RoundEndedEvent(currentRound, currentRoundTime, PhotonNetwork.CurrentRoom.Name, PhotonNetwork.CurrentRoom.PlayerCount);
            photonView.RPC("RestartRaoundsAndLevelsForAll", RpcTarget.All);
            ScenesManager.Instance.LoadScene("Podium");
            yield break;
        }

        AnalyticsEventsManager.Instance.RoundEndedEvent(currentRound, currentRoundTime, PhotonNetwork.CurrentRoom.Name, PhotonNetwork.CurrentRoom.PlayerCount);
        ScenesManager.Instance.LoadScene(PickAndRemoveLevel().ToString());
    }

    private IEnumerator ZoomToPlayerBeforeSceneChange(float duration)
    {
        if (currentPlayers.Count == 0) yield break;

        Transform target = currentPlayers[0].transform; // el único player vivo
        Camera mainCam = Camera.main;

        if (mainCam == null) yield break;

        Vector3 startPos = mainCam.transform.position;
        Vector3 targetPos = new Vector3(target.position.x, target.position.y, startPos.z);

        float startSize = mainCam.orthographicSize;
        float targetSize = startSize / 2f;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            mainCam.transform.position = Vector3.Lerp(startPos, targetPos, t);
            mainCam.orthographicSize = Mathf.Lerp(startSize, targetSize, t);

            yield return null;
        }

        mainCam.transform.position = targetPos;
        mainCam.orthographicSize = targetSize;
    }
}