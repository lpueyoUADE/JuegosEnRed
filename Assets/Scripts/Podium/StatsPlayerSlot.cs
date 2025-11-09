using TMPro;
using UnityEngine;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class StatsPlayerSlot : MonoBehaviour
{
    private Player assignedPlayer;

    [SerializeField] private TMP_Text playerNameText;
    [SerializeField] private TMP_Text playerCurrentScore;
    [SerializeField] private TMP_Text playerCurrentDeaths;


    void Awake()
    {
        SuscribeToPhotonNetworkManagerEvent();
    }

    void OnDestroy()
    {
        UnsuscribeToPhotonNetworkManagerEvent();
    }


    public void AssignPlayerInfoToSlot(Player player)
    {
        gameObject.SetActive(true);

        assignedPlayer = player;
        playerNameText.text = player.NickName;
        playerCurrentScore.text = player.CustomProperties["Score"].ToString();
        playerCurrentDeaths.text = player.CustomProperties["Deaths"].ToString();

        if (player.IsLocal)
        {
            playerNameText.color = new Color32(255, 165, 0, 255); // Naranja
        }
    }

    public void ClearPlayerInfoFromSlot()
    {
        assignedPlayer = null;
        playerNameText.text = string.Empty;
        playerCurrentScore.text = string.Empty;
        playerCurrentDeaths.text = string.Empty;
        gameObject.SetActive(false);
    }


    private void SuscribeToPhotonNetworkManagerEvent()
    {
        PhotonNetworkManager.Instance.OnPlayerPropertiesUpdateEvent += OnUpdatePodiumManagerProperties;
    }

    private void UnsuscribeToPhotonNetworkManagerEvent()
    {
        PhotonNetworkManager.Instance.OnPlayerPropertiesUpdateEvent -= OnUpdatePodiumManagerProperties;
    }

    private void OnUpdatePodiumManagerProperties(Player targetPlayer, Hashtable changedProps)
    {
        if (assignedPlayer != null && targetPlayer == assignedPlayer)
        {
            if (changedProps.ContainsKey("Score"))
            {
                playerCurrentScore.text = changedProps["Score"].ToString();
            }

            if (changedProps.ContainsKey("Deaths"))
            {
                playerCurrentDeaths.text = changedProps["Deaths"].ToString();
            }
        }
    }
}
