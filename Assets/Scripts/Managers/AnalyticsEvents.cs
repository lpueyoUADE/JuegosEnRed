using Photon.Pun;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Analytics;
using System.Collections.Generic;
using System.Threading.Tasks;

public class AnalyticsEvents : MonoBehaviour
{
    public static AnalyticsEvents Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            InitializeUnityServices();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private async void InitializeUnityServices()
    {
        try
        {
            await UnityServices.InitializeAsync();

            AnalyticsService.Instance.StartDataCollection();
            Debug.Log("[Analytics] Unity Gaming Services (UGS) Inicializado y coleccionando datos.");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[Analytics] Falla crítica en la inicialización: {e.Message}");
        }
    }

    private bool IsInitializedAndReady()
    {
        return UnityServices.State == ServicesInitializationState.Initialized;
    }

    private string GetMatchID()
    {

        if (PhotonNetwork.InRoom && PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("MatchID"))
        {
            return (string)PhotonNetwork.CurrentRoom.CustomProperties["MatchID"];
        }
        return "DEV_MATCH_" + System.Guid.NewGuid().ToString();
    }

    public void SendRoundEndedAnalytics(float roundDuration, int roundNumber, string levelName)
    {
        if (!IsInitializedAndReady()) return;

        if (!PhotonNetwork.IsMasterClient) return;

        string myMatchID = GetMatchID();

        try
        {
            var roundEvent = new CustomEvent("ROUND_ENDED");
            roundEvent.Add("MatchID", myMatchID);
            roundEvent.Add("RoundDuration", roundDuration); 
            roundEvent.Add("RoundNumber", roundNumber);
            roundEvent.Add("LevelName", levelName);

            AnalyticsService.Instance.RecordEvent(roundEvent);
            Debug.Log($"[Analytics] ROUND_ENDED. Duración: {roundDuration:F2}s, Ronda: {roundNumber}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[Analytics] Error al registrar ROUND_ENDED: {e.Message}");
        }
    }

    // Métrica 2: Efectividad de Power-Ups
    public void SendPlayerKilledAnalytics(string killerID, string victimID, string weaponUsed, bool killerHadPowerUp)
    {
        if (!IsInitializedAndReady()) return;

        string myMatchID = GetMatchID();

        try
        {
            var killEvent = new CustomEvent("PLAYER_KILLED");
            killEvent.Add("MatchID", myMatchID);
            killEvent.Add("KillerID", killerID);
            killEvent.Add("VictimID", victimID);
            killEvent.Add("WeaponUsed", weaponUsed);
            // CLAVE para la Métrica 2: True si se usó una Banana especial.
            killEvent.Add("KillsWithPowerUp", killerHadPowerUp);

            AnalyticsService.Instance.RecordEvent(killEvent);
            Debug.Log($"[Analytics] PLAYER_KILLED. Killer: {killerID}, Arma: {weaponUsed}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[Analytics] Error al registrar PLAYER_KILLED: {e.Message}");
        }
    }

    // Métrica 1 y 3: Recolección y Tiempo de Power-Ups
    public void SendPowerUpCollected(string playerID, string powerUpType, float timeToCollect)
    {
        if (!IsInitializedAndReady()) return;

        string myMatchID = GetMatchID();

        try
        {
            var collectEvent = new CustomEvent("POWERUP_COLLECTED");
            collectEvent.Add("MatchID", myMatchID);
            collectEvent.Add("PlayerID", playerID);
            collectEvent.Add("PowerUpType", powerUpType); 
            collectEvent.Add("TimeToCollect", timeToCollect); 

            AnalyticsService.Instance.RecordEvent(collectEvent);
            Debug.Log($"[Analytics] POWERUP_COLLECTED. Tipo: {powerUpType}, Tiempo: {timeToCollect:F2}s");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[Analytics] Error al registrar POWERUP_COLLECTED: {e.Message}");
        }
    }

    // Métrica 5 y 7: Abandono de Partida
    public void SendPlayerLeftMatch(string playerID, int roundsPlayed, string exitSource)
    {
        if (!IsInitializedAndReady()) return;

        string myMatchID = GetMatchID();

        try
        {
            var exitEvent = new CustomEvent("PLAYER_LEFT_MATCH");
            exitEvent.Add("MatchID", myMatchID);
            exitEvent.Add("PlayerID", playerID);
            exitEvent.Add("RoundsPlayedBeforeExit", roundsPlayed); // Métrica 5
            exitEvent.Add("ExitSource", exitSource);               // Métrica 7 (Ej: MENU_QUIT, ALTF4_QUIT)

            AnalyticsService.Instance.RecordEvent(exitEvent);
            Debug.Log($"[Analytics] PLAYER_LEFT_MATCH. Source: {exitSource}, Rondas: {roundsPlayed}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[Analytics] Error al registrar PLAYER_LEFT_MATCH: {e.Message}");
        }
    }

    // Puedes continuar añadiendo más eventos aquí (ej: BOX_SPAWNED, PLAYER_DASH_USED, etc.)
}