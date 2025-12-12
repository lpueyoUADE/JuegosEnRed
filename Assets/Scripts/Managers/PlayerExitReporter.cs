using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class PlayerExitReporter : SingletonMonoBehaviourPunCallbacks<PlayerExitReporter>
{
    private bool exitAlreadyReported = false;
    private void Awake()
    {
        CreateSingleton(true);
    }
    private void OnApplicationQuit()
    {
        TrySendForcedExit("ALTF4_QUIT");
    }

    // 2) Desconexión inesperada: internet cortado, ragequit, cierre abrupto
    public override void OnDisconnected(DisconnectCause cause)
    {
        if (cause == DisconnectCause.DisconnectByClientLogic)
            return; 

        string exitSource;

        if (Application.isPlaying == false)
        {
            exitSource = "ALTF4_QUIT";
        }
        else
        {
            exitSource = "NETWORK_DISCONNECT";
        }

        TrySendForcedExit(exitSource);
    }
    private void TrySendForcedExit(string exitSource)
    {
        if (exitAlreadyReported) return;

        exitAlreadyReported = true;

        Debug.Log("[ExitReporter] Sending exit event: " + exitSource);

        if (!PhotonNetwork.InRoom)
            return;

        string playerID = PhotonNetwork.LocalPlayer.UserId;
        int roundsPlayed = PlayersManager.Instance?.CurrentRound ?? 0;

        AnalyticsEvents.Instance.SendPlayerLeftMatch(playerID, roundsPlayed, exitSource);
    }
}
