using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LootLocker.Requests;

public class LootLockerManager : SingletonMonoBehaviourPun<LootLockerManager>
{
    /* How to:
     * https://www.youtube.com/watch?v=u8llsk7FoYg
    */

    const string leaderboardKey = "bananazleaderboard";
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
    private void Start()
    {
        StartCoroutine(LoginRoutine());


        StartCoroutine(SubmitScoreRoutine(10));
    }
}
