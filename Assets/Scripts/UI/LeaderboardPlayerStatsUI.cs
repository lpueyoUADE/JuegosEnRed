using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LeaderboardPlayerStatsUI : MonoBehaviour
{
    public TextMeshProUGUI playerName;
    public TextMeshProUGUI playerScore;

    public void SetPlayerStats(string name, string score)
    {
        playerName.text = name;
        playerScore.text = score;
    }
}
