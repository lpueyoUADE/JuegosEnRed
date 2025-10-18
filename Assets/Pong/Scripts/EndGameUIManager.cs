using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGameUIManager : MonoBehaviour
{
    public TextMeshProUGUI pointsText;
    public TextMeshProUGUI winnersText;

    public string MainMenuSceneName;

    private void Start()
    {
        pointsText.text += " " + PlayerPrefs.GetString("Points");
        winnersText.text += " " + PlayerPrefs.GetString("WinnerTeam");
    }

    public void OnBackToMenuClicked()
    {
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
        }
        PhotonNetwork.Disconnect();
        SceneManager.LoadScene(MainMenuSceneName);
    }
}
