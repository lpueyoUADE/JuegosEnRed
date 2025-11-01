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
    }

    private void UnsuscribeToPlayerModelEvents()
    {
        PlayerModel.OnPlayerDeath -= OnShowLoosePanel;
    }

    private void OnShowLoosePanel()
    {
        panel.SetActive(true);
        looseText.SetActive(true);
    }
}
