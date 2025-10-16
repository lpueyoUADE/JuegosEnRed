using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PongUIManager : MonoBehaviour
{
    public TextMeshProUGUI pointsText;

    private void Start()
    {
        PongGameManager.Instance.OnScoreChanged += UpdateScoreUI;
    }

    void OnDestroy()
    {
        PongGameManager.Instance.OnScoreChanged -= UpdateScoreUI;
    }

    private void UpdateScoreUI(int left, int right)
    {
        pointsText.text = $"{left} | {right}";
    }
}
