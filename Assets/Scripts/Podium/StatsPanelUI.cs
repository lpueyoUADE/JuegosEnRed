using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class StatsPanelUI : MonoBehaviour
{
    [Header("PlayerInformation:")]
    [SerializeField] private StatsPlayerSlot[] statsPlayerSlots;


    void Awake()
    {
        SuscribeToPhotonNetworkManagerEvent();
        RefreshSlots();
    }

    void OnDestroy()
    {
        UnsuscribeToPhotonNetworkManagerEvent();
    }


    private void SuscribeToPhotonNetworkManagerEvent()
    {
        PhotonNetworkManager.Instance.OnPlayerLeftRoomEvent += RefreshSlots;
    }

    private void UnsuscribeToPhotonNetworkManagerEvent()
    {
        PhotonNetworkManager.Instance.OnPlayerLeftRoomEvent -= RefreshSlots;
    }

    private void RefreshSlots()
    {
        foreach (var slot in statsPlayerSlots)
        {
            slot.ClearPlayerInfoFromSlot();
        }

        // Asignar jugadores actuales
        Player[] players = PhotonNetwork.PlayerList;
        for (int i = 0; i < 4; i++)
        {
            if (i < players.Length)
            {
                statsPlayerSlots[i].AssignPlayerInfoToSlot(players[i]);
            }
        }
    }
}
