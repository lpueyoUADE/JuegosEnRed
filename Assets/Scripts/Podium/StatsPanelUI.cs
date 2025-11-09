using Photon.Pun;
using Photon.Realtime;
using System.Linq;
using UnityEngine;

public class StatsPanelUI : MonoBehaviour
{
    [Header("PlayerInformation:")]
    [SerializeField] private StatsPlayerSlot[] statsPlayerSlots;

    void Awake()
    {
        PhotonNetworkManager.Instance.OnPlayerLeftRoomEvent += RefreshSlots;
        PlayerModel.OnPointAcquired += RefreshSlots;
        RefreshSlots();
    }

    void OnDestroy()
    {
        PhotonNetworkManager.Instance.OnPlayerLeftRoomEvent -= RefreshSlots;
        PlayerModel.OnPointAcquired -= RefreshSlots;
    }

    private void RefreshSlots()
    {
        foreach (var slot in statsPlayerSlots)
        {
            slot.ClearPlayerInfoFromSlot();
        }

        // Asignar jugadores actuales
        Player[] players = PhotonNetwork.PlayerList;

        players = players.OrderByDescending(p => p.CustomProperties.ContainsKey("Score") ? (int)p.CustomProperties["Score"] : 0)
            .ThenBy(p => p.CustomProperties.ContainsKey("Deaths") ? (int)p.CustomProperties["Deaths"] : 0).ToArray();

        for (int i = 0; i < statsPlayerSlots.Length; i++)
        {
            if (i < players.Length)
            {
                statsPlayerSlots[i].AssignPlayerInfoToSlot(players[i]);
            }
        }
    }
}
