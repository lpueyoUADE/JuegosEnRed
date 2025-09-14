using UnityEngine;

public class FinalScreensUI : MonoBehaviour
{
    [SerializeField] private GameObject winText;
    [SerializeField] private GameObject looseText;


    void Awake()
    {
        SuscribeToPlayerModelEvents();
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
        winText.SetActive(true);
    }

    private void ShowLoosePanel()
    {
        looseText.SetActive(true);
    }
}
