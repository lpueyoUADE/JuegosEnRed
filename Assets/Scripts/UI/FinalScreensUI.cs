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
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (PhotonNetworkManager.Instance.IsHost)
            {
                ScenesManager.Instance.LoadScene("Level2");   
            }
        }
    }

    void OnDestroy()
    {
        UnsuscribeToPlayerModelEvents();
    }


    private void SuscribeToPlayerModelEvents()
    {
        PlayerModel.OnPlayerWin += OnShowWinPanel;
        PlayerModel.OnPlayerDeath += OnShowLoosePanel;
    }

    private void UnsuscribeToPlayerModelEvents()
    {
        PlayerModel.OnPlayerWin -= OnShowWinPanel;
        PlayerModel.OnPlayerDeath -= OnShowLoosePanel;
    }

    private void OnShowWinPanel()
    {
        PlayerModel[] playerModels = FindObjectsOfType<PlayerModel>();

        Debug.Log(playerModels.Length);

        if (playerModels.Length == 1)
        {
            panel.SetActive(true);
            winText.SetActive(true);
        }
    }

    private void OnShowLoosePanel()
    {
        panel.SetActive(true);
        looseText.SetActive(true);
    }
}
