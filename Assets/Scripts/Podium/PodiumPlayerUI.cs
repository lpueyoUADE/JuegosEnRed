using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PodiumPlayerUI : MonoBehaviour
{
    public TextMeshProUGUI playerName;
    public TextMeshProUGUI score;
    public TextMeshProUGUI deaths;

    public void SetName(string name)
    {
        playerName.text = name;
    }

    public void SetScore(string score)
    {
        this.score.text = score;
    }

    public void SetDeaths(string deaths)
    {
        this.deaths.text = deaths;
    }

    public void SetColor(Color color)
    {
        playerName.color = color;
        score.color = color;
        deaths.color = color;
    }
}
