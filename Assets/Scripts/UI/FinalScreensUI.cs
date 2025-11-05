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

    void OnDestroy()
    {
        UnsuscribeToPlayerModelEvents();
    }


    private void SuscribeToPlayerModelEvents()
    {
        PlayerModel.OnPlayerDeath += OnShowLoosePanel;
        PlayerModel.OnPlayerWinCurrentRound += OnShowWinPanel;
    }

    private void UnsuscribeToPlayerModelEvents()
    {
        PlayerModel.OnPlayerDeath -= OnShowLoosePanel;
        PlayerModel.OnPlayerWinCurrentRound -= OnShowWinPanel;
    }

    private void OnShowLoosePanel()
    {
        panel.SetActive(true);
        looseText.SetActive(true);
    }

    private void OnShowWinPanel()
    {
        panel.SetActive(true);
        winText.SetActive(true);
    }
}
