using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetHighscoresManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Player localPlayer = PhotonNetwork.LocalPlayer;

        int score = 0;

        if (localPlayer.CustomProperties.ContainsKey("Score"))
        {
            score = (int)localPlayer.CustomProperties["Score"];
        }

        StartCoroutine(LootLockerManager.Instance.SubmitScoreRoutine(score));
    }
}
