using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Collections;
using UnityEngine.SceneManagement;
using System;

public class PlayersManager : SingletonMonoBehaviourPun<PlayersManager>
{
    [SerializeField] private List<PlayerModel> currentPlayers;

    [SerializeField] private float cameraEffectDuringTime;

    [Serializable]
    public class PlayerStats
    {
        public string nickname;
        public int score;
        public int deaths;
        public Color color;

        public PlayerStats(string nickname, int score, int deaths, Color color)
        {
            this.nickname = nickname;
            this.score = score;
            this.deaths = deaths;
            this.color = color;
        }
    }

    public Dictionary<int, PlayerStats> PlayersPodium;
    public List<PlayerModel> CurrentPlayers { get => currentPlayers; }

    void Awake()
    {
        CreateSingleton(true);
        PlayersPodium = new();
    }

    void Start()
    {
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
                PlayersPodium.Add(pv.Owner.ActorNumber, new(playerModel.GetNickname(), 0, 0, playerModel.Color));
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
                PlayersPodium.Remove(pv.Owner.ActorNumber);
            }
        }
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
        photonView.RPC("ChangeNextLevel", RpcTarget.All);
    }

    [PunRPC]
    private void ChangeNextLevel()
    {
        if (currentPlayers.Count > 1) return;

        for (int i = 0; i < currentPlayers.Count; i++)
        {
            currentPlayers[i].AcceptingInput = false;
        }

        StartCoroutine(WaitSomeSecondsToChangeScene());
    }

    private IEnumerator WaitSomeSecondsToChangeScene()
    {
        yield return StartCoroutine(ZoomToPlayerBeforeSceneChange(cameraEffectDuringTime)); 


        yield return new WaitForSecondsRealtime(1.5f);

        currentPlayers.Clear();

        /*foreach (var player in Instance.currentPlayers.ToArray())
        {
            if (player.photonView.IsMine)
            {
                PhotonNetwork.Destroy(player.gameObject);
            }
        }*/

        string currentSceneName = SceneManager.GetActiveScene().name;

        if (Enum.TryParse(currentSceneName, out GameScenes currentSceneEnum))
        {
            int nextSceneValue = (int)currentSceneEnum + 1;

            // Si nos pasamos del enum, volvemos al primer nivel
            if (!Enum.IsDefined(typeof(GameScenes), nextSceneValue) || nextSceneValue < (int)GameScenes.Level1)
            {
                nextSceneValue = (int)GameScenes.Level1;
            }

            GameScenes nextSceneEnum = (GameScenes)nextSceneValue;
            ScenesManager.Instance.LoadScene(nextSceneEnum.ToString());
        }
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
