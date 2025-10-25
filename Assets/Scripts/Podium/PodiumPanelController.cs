using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PodiumPanelController : MonoBehaviour
{
    public List<PodiumPlayerUI> podiumPlayers;

    private void OnEnable()
    {
        if (PhotonNetworkManager.Instance != null)
        {
            PhotonNetworkManager.Instance.OnPlayerEnteredRoomEvent += RefreshPlayers;
            //PhotonNetworkManager.Instance.OnPlayerLeftRoomEvent += RefreshPlayers;
        }

        RefreshPlayers();
    }

    private void OnDisable()
    {
        if (PhotonNetworkManager.Instance != null)
        {
            PhotonNetworkManager.Instance.OnPlayerEnteredRoomEvent -= RefreshPlayers;
            //PhotonNetworkManager.Instance.OnPlayerLeftRoomEvent -= RefreshPlayers;
        }
    }

    private void RefreshPlayers()
    {
        var sortedPlayers = PlayersManager.Instance.PlayersPodium
            .OrderByDescending(p => p.Value.score)
            .Select(p => (p.Value.nickname, p.Value.score, p.Value.deaths, p.Value.color))
            .ToList();

        for (int i = 0; i < podiumPlayers.Count; i++)
        {
            podiumPlayers[i].gameObject.SetActive(i < sortedPlayers.Count);

            if(i < sortedPlayers.Count)
            {
                podiumPlayers[i].playerName.text = sortedPlayers[i].nickname;
                podiumPlayers[i].score.text = sortedPlayers[i].score.ToString();
                podiumPlayers[i].deaths.text = sortedPlayers[i].deaths.ToString();
                podiumPlayers[i].SetColor(sortedPlayers[i].color);
            }
        }
    }
}
