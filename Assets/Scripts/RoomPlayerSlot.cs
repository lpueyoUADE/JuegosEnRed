using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class RoomPlayerSlot : MonoBehaviour
{
    private Player assignedPlayer;

    [SerializeField] private TMP_Text playerNameText;
    [SerializeField] private Image skinPreview;
    [SerializeField] private Button prevButton;
    [SerializeField] private Button nextButton;


    void Awake()
    {
        SuscribeToPhotonNetworkManagerEvent();
    }

    void OnDestroy()
    {
        UnsuscribeToPhotonNetworkManagerEvent();
    }

    private void SetActiveInnerComponents(bool active)
    {
        playerNameText.gameObject.SetActive(active);
        skinPreview.gameObject.SetActive(active);
        prevButton.gameObject.SetActive(active);
        nextButton.gameObject.SetActive(active);
    }
    public void SetEmptySlot()
    {
        SetActiveInnerComponents(false);
    }

    public void AssignPlayerInfoToSlot(Player player)
    {
        SetActiveInnerComponents(true);

        assignedPlayer = player;
        playerNameText.text = player.NickName;

        bool isLocal = player == PhotonNetwork.LocalPlayer;
        prevButton.gameObject.SetActive(isLocal);
        nextButton.gameObject.SetActive(isLocal);

        if (player.CustomProperties.ContainsKey("SkinIndex"))
        {
            int skinIndex = (int)player.CustomProperties["SkinIndex"];
            skinPreview.color = PlayerSkinManager.Instance.PlayerSkins[skinIndex];
        }

        if (isLocal)
        {
            prevButton.onClick.RemoveAllListeners();
            nextButton.onClick.RemoveAllListeners();

            prevButton.onClick.AddListener(() => PlayerSkinManager.Instance.ChangeSkin(skinPreview, -1));
            nextButton.onClick.AddListener(() => PlayerSkinManager.Instance.ChangeSkin(skinPreview, +1));
        }
    }

    public void ClearPlayerInfoFromSlot()
    {
        assignedPlayer = null;
        playerNameText.text = string.Empty;
        skinPreview.color = Color.black;
        prevButton.gameObject.SetActive(false);
        nextButton.gameObject.SetActive(false);
    }


    private void SuscribeToPhotonNetworkManagerEvent()
    {
        PhotonNetworkManager.Instance.OnPlayerPropertiesUpdateEvent += OnUpdateSkinProperties;
    }

    private void UnsuscribeToPhotonNetworkManagerEvent()
    {
        PhotonNetworkManager.Instance.OnPlayerPropertiesUpdateEvent -= OnUpdateSkinProperties;
    }

    private void OnUpdateSkinProperties(Player targetPlayer, Hashtable changedProps)
    {
        if (assignedPlayer != null && targetPlayer == assignedPlayer && changedProps.ContainsKey("SkinIndex"))
        {
            int skinIndex = (int)changedProps["SkinIndex"];
            skinPreview.color = PlayerSkinManager.Instance.PlayerSkins[skinIndex];
        }
    }
}
