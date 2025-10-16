using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EndGameUIManager : MonoBehaviour
{
    public TextMeshProUGUI pointsText;
    public TextMeshProUGUI winnersText;

    private void Start()
    {
        pointsText.text += " " + PlayerPrefs.GetString("Points");
        winnersText.text += " " + PlayerPrefs.GetString("WinnerTeam");
    }
}
