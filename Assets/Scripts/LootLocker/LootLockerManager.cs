using LootLocker.Requests;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootLockerManager : SingletonMonoBehaviourPun<LootLockerManager>
{
    /* How to:
     * https://www.youtube.com/watch?v=u8llsk7FoYg
    */

    const string leaderboardKey = "bananazleaderboard";


    public static event Action OnLoginCompleted;
    void Awake()
    {
        CreateSingleton(true);
    }

    IEnumerator LoginRoutine()
    {
        bool done = false;
        LootLockerSDKManager.StartGuestSession((response) => 
        {
            if (response.success)
            {
                Debug.Log("Player logged in to LootLocker.");
                PlayerPrefs.SetString("PlayerID", response.player_id.ToString());
            }
            else
            {
                Debug.Log("Could not start session");
            }

            done = true;
        });

        yield return new WaitWhile(()=> done == false);

        OnLoginCompleted?.Invoke();
    }

    public IEnumerator SetPlayerNameRoutine(string name)
    {
        bool done = false;

        LootLockerSDKManager.SetPlayerName(name, (response) =>
        {
            if(response.success)
            {
                Debug.Log("Player Name set Successfully.");
            } else
            {
                Debug.Log("Could not set player name " + response.errorData);
            }

            done = true;
        });

        yield return new WaitWhile(() => done == false);
    }

    public IEnumerator SubmitScoreRoutine(int scoreToUpload)
    {
        // TODO: Llamar a esto cuando termina la partida para cargar los puntos de los players.
        bool done = false;
        string playerID = PlayerPrefs.GetString("PlayerID");
        LootLockerSDKManager.SubmitScore(playerID, scoreToUpload, leaderboardKey, (response) => {
            if (response.success)
            {
                Debug.Log("Succesfully Uploaded Score");
            } else
            {
                Debug.Log("Error when uploading Score: " + response.errorData);
            }
                done = true;
        });

        yield return new WaitWhile(()=> done == false);
    }

    public IEnumerator FecthHighScoresRoutine(Action<List<(string name, int score)>> onResult)
    {
        bool done = false;
        List<(string name, int score)> results = new List<(string name, int score)>();

        LootLockerSDKManager.GetScoreList(leaderboardKey, 10, 0, (response) => {
            if (response.success)
            {
                foreach (var member in response.items)
                {
                    string name = string.IsNullOrEmpty(member.player.name) ?
                                  member.player.id.ToString() :
                                  member.player.name;

                    int score = member.score;

                    results.Add((name, score));
                }
            }
            else
            {
                Debug.Log("Failed " + response.errorData);
            }

            done = true;
        });

        yield return new WaitWhile( ()=> done == false);

        // Devolvemos los resultados
        onResult?.Invoke(results);
    }
    private IEnumerator flow()
    {
        yield return StartCoroutine(LoginRoutine());         // Espera a que termine el login
        yield return StartCoroutine(SubmitScoreRoutine(20)); // Luego envía el score
    }


    private void Start()
    {
        StartCoroutine(flow());
    }
    private void OnApplicationQuit()
    {
        LootLockerSDKManager.EndSession((response) =>
        {
            if (response.success)
                Debug.Log("LootLocker session ended cleanly.");
            else
                Debug.Log("Error ending LootLocker session: " + response.errorData);
        });
    }
}
