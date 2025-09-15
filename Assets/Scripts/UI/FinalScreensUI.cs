using Photon.Pun;
using UnityEngine;

public class FinalScreensUI : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private GameObject winText;
    [SerializeField] private GameObject looseText;


    void Awake()
    {
        SuscribeToPlayerModelEvents();
        panel.SetActive(false);
    }

    void Update()
    {
        // Test
        /*if (Input.GetKeyDown(KeyCode.R))
        {
            ScenesManager.Instance.LoadScene("Game");
            TimeManager.Instance.photonView.RPC("RestartIsCountdownFinished", RpcTarget.All);
        }*/
    }

    void OnDestroy()
    {
        UnsuscribeToPlayerModelEvents();
    }


    private void SuscribeToPlayerModelEvents()
    {
        PlayerModel.OnPlayerWin += ShowWinPanel;
        PlayerModel.OnPlayerDeath += ShowLoosePanel;
    }

    private void UnsuscribeToPlayerModelEvents()
    {
        PlayerModel.OnPlayerWin -= ShowWinPanel;
        PlayerModel.OnPlayerDeath -= ShowLoosePanel;
    }

    private void ShowWinPanel()
    {
        PlayerModel[] playerModels = FindObjectsOfType<PlayerModel>();

        Debug.Log(playerModels.Length);

        if (playerModels.Length == 1)
        {
            panel.SetActive(true);
            winText.SetActive(true);
        }
    }

    private void ShowLoosePanel()
    {
        panel.SetActive(true);
        looseText.SetActive(true);
    }
}
