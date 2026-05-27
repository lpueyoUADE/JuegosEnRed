using UnityEngine;
using Photon.Pun;

public class PlayerExitReporter : MonoBehaviour
{
    void OnApplicationQuit()
    {
        string playerId = PlayerAnalyticsId.GetOrCreateId();
        //AnalyticsEventsManager.Instance.PlayerLeftMatchEvent(playerId, PhotonNetwork.CurrentRoom.Name, PlayersManager.Instance.CurrentRound + 1, "ViaAltF4OrClosingWindow");
    }
}
