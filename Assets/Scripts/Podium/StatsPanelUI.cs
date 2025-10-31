using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class StatsPanelUI : MonoBehaviour
{
    [Header("PlayerInformation:")]
    [SerializeField] private StatsPlayerSlot[] tatsPlayerSlots;


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
        foreach (var slot in tatsPlayerSlots)
        {
            slot.ClearPlayerInfoFromSlot();
        }

        // Asignar jugadores actuales
        Player[] players = PhotonNetwork.PlayerList;
        for (int i = 0; i < 4; i++)
        {
            if (i < players.Length)
            {
                tatsPlayerSlots[i].AssignPlayerInfoToSlot(players[i]);
            }
        }
    }
}
