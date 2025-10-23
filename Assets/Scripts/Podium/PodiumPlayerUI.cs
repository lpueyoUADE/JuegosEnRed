using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PodiumPlayerUI : MonoBehaviour
{
    public TextMeshProUGUI playerName;
    public TextMeshProUGUI points;
    public TextMeshProUGUI deaths;

    public void SetName(string name)
    {
        playerName.text = name;
    }

    public void SetPoints(string points)
    {
        this.points.text = points;
    }

    public void SetDeaths(string deaths)
    {
        this.deaths.text = deaths;
    }

    public void SetColor(Color color)
    {
        playerName.color = color;
        points.color = color;
        deaths.color = color;
    }
}
