using Photon.Pun;
using UnityEngine;

public class FinalScreensUI : MonoBehaviour
{
    [SerializeField] private GameObject winText;
    [SerializeField] private GameObject looseText;


    void Awake()
    {
        SuscribeToPlayerModelEvents();
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
            winText.SetActive(true);
        }
    }

    private void ShowLoosePanel()
    {
        looseText.SetActive(true);
    }
}
