using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Collections;

public class PlayersManager : SingletonMonoBehaviourPun<PlayersManager>
{
    [SerializeField] private List<PlayerModel> currentPlayers;

    [SerializeField] private float cameraEffectDuringTime;

    [Header("Rondas")]
    [SerializeField] private int totalRounds = 6;
    private int currentRound = 0;

    [Header("Niveles")]
    [SerializeField] private List<GameScenes> levels;
    private List<GameScenes> currentLevels;

    public List<PlayerModel> CurrentPlayers { get => currentPlayers; }

    public int TotalRounds { get => totalRounds; }
    public int CurrentRound { get => currentRound; }  


    void Awake()
    {
        CreateSingleton(true);
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
        GameScenes level = PickRandomLevel();

        if (currentLevels.Count == 0)
        {
            InitCurrentLevels();
        }

        return level;
    }

    private GameScenes PickRandomLevel()
    {
        int index = Random.Range(0, currentLevels.Count);
        GameScenes level = currentLevels[index];
        currentLevels.RemoveAt(index);

        return level;
    }


    public void UnregisterMeForMe(PlayerModel me)
    {
        if (currentPlayers.Contains(me))
        {
            currentPlayers.Remove(me);
        }
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
        currentRound = 0;
        InitCurrentLevels();
    }

    private IEnumerator WaitSomeSecondsToChangeScene()
    {
        yield return StartCoroutine(ZoomToPlayerBeforeSceneChange(cameraEffectDuringTime)); 

        yield return new WaitForSecondsRealtime(1.5f);

        currentPlayers.Clear();

        if (!PhotonNetworkManager.Instance.IsHost) yield break;

        // Si se fueron todos los players de la room ir directo a la escena "Podium"
        /*if (PhotonNetworkManager.Instance.GetCurrentPlayersCountInRoom() < 2)
        {
            photonView.RPC("RestartRaoundsAndLevelsForAll", RpcTarget.All);
            ScenesManager.Instance.LoadScene("Podium");
            yield break;
        }*/

        photonView.RPC("IncreaseCurrentRoundsForAll", RpcTarget.All);

        // Si ya jugamos todas las rondas ir directo a la escena "Podium"
        if (currentRound >= totalRounds)
        {
            photonView.RPC("RestartRaoundsAndLevelsForAll", RpcTarget.All);
            ScenesManager.Instance.LoadScene("Podium");
            yield break;
        }

        // Si llegamos hasta aca pasamos de nivel de forma random
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

    private void InitCurrentLevels()
    {
        currentLevels = new List<GameScenes>(levels);
    }
}
