using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderboardUI : MonoBehaviour
{
    public LeaderboardPlayerStatsUI leaderboardPlayerStatsUIPrefab;
    public Transform container;

    public int MAX_ENTRIES = 10;

    public void RenderHighScores(List<(string name, int score)> results)
    {
        foreach (Transform child in container)
            Destroy(child.gameObject);

        int count = Mathf.Min(results.Count, MAX_ENTRIES);

        for (int i = 0; i < count; i++)
        {
            var p = results[i];

            var uiEntry = Instantiate(leaderboardPlayerStatsUIPrefab, container);
            uiEntry.SetPlayerStats($"{i + 1}. {p.name}", p.score.ToString());
        }
    }

    private void OnEnable()
    {
        if (LootLockerManager.Instance == null)
            return;

        StartCoroutine(LootLockerManager.Instance.FecthHighScoresRoutine(RenderHighScores));
    }
}