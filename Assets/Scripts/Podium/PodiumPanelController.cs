using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PodiumPanelController : MonoBehaviour
{
    public List<PodiumPlayerUI> podiumPlayers;
    private void OnEnable()
    {
        if (PhotonNetworkManager.Instance != null)
        {
            PhotonNetworkManager.Instance.OnPlayerEnteredRoomEvent += RefreshPlayers;
            PhotonNetworkManager.Instance.OnPlayerLeftRoomEvent += RefreshPlayers;
        }

        RefreshPlayers();
    }

    private void OnDisable()
    {
        if (PhotonNetworkManager.Instance != null)
        {
            PhotonNetworkManager.Instance.OnPlayerEnteredRoomEvent -= RefreshPlayers;
            PhotonNetworkManager.Instance.OnPlayerLeftRoomEvent -= RefreshPlayers;
        }
    }

    private void RefreshPlayers()
    {
        int playerCount = PhotonNetworkManager.Instance.GetCurrentPlayersCountInRoom();

        SetActivePlayersCount(playerCount);
        SortPlayers();
    }
    public void SetActivePlayersCount(int count)
    {
        count = Mathf.Clamp(count, 0, podiumPlayers.Count);

        for (int i = 0; i < podiumPlayers.Count; i++)
        {
            podiumPlayers[i].gameObject.SetActive(i < count);
        }
    }

    private void SortPlayers()
    {

    }
}
