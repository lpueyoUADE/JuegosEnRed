using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Analytics;
using System;
using System.Collections.Generic;

public class AnalyticsEventsManager : SingletonMonoBehaviour<AnalyticsEventsManager>
{
    private Dictionary<string, double> powerUpSpawnTimes = new();


    void Awake()
    {
        CreateSingleton(true);
    }

    void Start()
    {
        InitializeUnityServices();
    }


    // POWER-UP SPAWNED
    /*public void TrackPowerUpSpawned(string powerUpId, int powerUpType, int currentRound, string matchId)
    {
        double spawnTime = Time.time;
        powerUpSpawnTimes[powerUpId] = spawnTime;

        var evt = new CustomEvent("powerup_spawned")
        {
            { "match_id", matchId },
            { "powerup_id", powerUpId },
            { "powerup_type", powerUpType },
            { "current_round", currentRound },
            { "spawn_time", spawnTime }
        };

        AnalyticsService.Instance.RecordEvent(evt);
    }*/

    // POWER-UP COLLECTED
    public void PowerUpColledtedEvent(string playerId, string powerUpId, float timeToCollectPowerUpAfterSpawn)
    {
        var evt = new CustomEvent("powerup_collected")
        {
            { "PLAYER_ID", playerId },
            { "POWERUP_ID", powerUpId },
            { "TIME_TO_COLLECT_POWERUP_AFTER_SPAWN", timeToCollectPowerUpAfterSpawn },
        };

        AnalyticsService.Instance.RecordEvent(evt);
    }

    // PLAYER KILLED
    public void PlayerKilledEvent(string boomerangType, string matchId)
    {
        var evt = new CustomEvent("PLAYER_KILLED")
        {
            { "MATCH_ID", matchId },
            { "BOOMERANG_TYPE", boomerangType },
        };

        AnalyticsService.Instance.RecordEvent(evt);
    }

    // ROUND ENDED
    public void RoundEndedEvent(int currentRound, float roundDuration, string matchId, int amountOfPlayerInRound)
    {
        var evt = new CustomEvent("ROUND_ENDED")
        {
            { "MATCH_ID", matchId },
            { "CURRENT_EOUND", currentRound },
            { "ROUND_DURATION", roundDuration },
            { "AMOUNT_OF_PLAYERS_IN_ROUND", amountOfPlayerInRound }
        };

        AnalyticsService.Instance.RecordEvent(evt);
    }

    // PLAYER LEFT MATCH
    public void PlayerLeftMatchEvent(string playerId, string matchId, int roundsPlayed, string exitSource)
    {
        var evt = new CustomEvent("PLAYER_LEFT_MATCH")
        {
            { "MATCH_ID", matchId },
            { "PLAYER_ID", playerId },
            { "ROUNDS_PLAYED", roundsPlayed },
            { "EXIT_SOURCE", exitSource }
        };

        AnalyticsService.Instance.RecordEvent(evt);
    }

    // PLAYER MODIFIED SOUND
    public void PlayerModifiedSoundEvent(string playerId)
    {
        var evt = new CustomEvent("PLAYER_MODIFIED_SOUND")
        {
            { "PLAYER_ID", playerId },
        };

        AnalyticsService.Instance.RecordEvent(evt);
    }

    // SESSION STARTED
    /*public void SessionStartedEvent()
    {
        var evt = new CustomEvent("SESSION_STARTED")
        {

        };

        AnalyticsService.Instance.RecordEvent(evt);
    }

    // SESSION ENDED    
    public void SessionEnded()
    {
        var evt = new CustomEvent("SESSION_ENDED")
        {

        };

        AnalyticsService.Instance.RecordEvent(evt);
    }*/


    private async void InitializeUnityServices()
    {
        try
        {
            await UnityServices.InitializeAsync();
            AnalyticsService.Instance.StartDataCollection();
            Debug.Log("[Analytics] Unity Gaming Services (UGS) Inicializado y coleccionando datos.");
        }

        catch (Exception e)
        {
            Debug.LogError($"[Analytics] Falla crítica en la inicialización: {e.Message}");
        }
    }
}

public static class PlayerAnalyticsId
{
    private const string PlayerIdKey = "PLAYER_ANALYTICS_ID";

    public static string GetOrCreateId()
    {
        if (PlayerPrefs.HasKey(PlayerIdKey))
        {
            Debug.Log("PlayerId ya guardado en memoria: " + PlayerIdKey);
            return PlayerPrefs.GetString(PlayerIdKey);
        }

        string newId = Guid.NewGuid().ToString();
        PlayerPrefs.SetString(PlayerIdKey, newId);
        PlayerPrefs.Save();

        Debug.Log("PlayerId creado por primera vez: " + PlayerIdKey);
        return newId;
    }
}