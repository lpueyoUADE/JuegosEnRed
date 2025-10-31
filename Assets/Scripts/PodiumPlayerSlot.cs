using UnityEngine;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;

public class PodiumPlayerSlot : MonoBehaviour
{
    private Player assignedPlayer;

    [SerializeField] private TMP_Text playerNameText;
    [SerializeField] private TMP_Text playerCurrentScore;
    [SerializeField] private TMP_Text playerCurrentDeaths;
    [SerializeField] private Image skinPreview;


    void Awake()
    {

    }


    public void AssignPlayerInfoToSlot(Player player)
    {
        assignedPlayer = player;
        playerNameText.text = player.NickName;
        playerCurrentScore.text = player.CustomProperties["Score"].ToString();
        playerCurrentDeaths.text = player.CustomProperties["Deaths"].ToString();

        if (player.CustomProperties.ContainsKey("SkinIndex"))
        {
            int skinIndex = (int)player.CustomProperties["SkinIndex"];
            Color playerColor = PlayerSkinManager.Instance.PlayerSkins[skinIndex];

            //playerNameText.color = playerColor;
            //playerCurrentScore.color = playerColor;
            //playerCurrentDeaths.color = playerColor;
            skinPreview.color = playerColor;
        }
    }
}
