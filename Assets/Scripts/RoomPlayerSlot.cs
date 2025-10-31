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
    [SerializeField] private Image readyIndicator;
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


    public void AssignPlayerInfoToSlot(Player player)
    {
        assignedPlayer = player;
        playerNameText.text = player.NickName;

        bool isLocal = player == PhotonNetwork.LocalPlayer;
        prevButton.gameObject.SetActive(isLocal);
        nextButton.gameObject.SetActive(isLocal);

        if (isLocal)
        {
            Hashtable props = new Hashtable();
            props["IsReady"] = false;
            assignedPlayer.SetCustomProperties(props);
        }

        bool isReady = player.CustomProperties.ContainsKey("IsReady") && (bool)player.CustomProperties["IsReady"];
        readyIndicator.color = isReady ? Color.green : Color.red;
        readyIndicator.gameObject.SetActive(true);

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
        readyIndicator.color = Color.red;
        readyIndicator.gameObject.SetActive(false);
        playerNameText.text = string.Empty;
        skinPreview.color = Color.black;
        prevButton.gameObject.SetActive(false);
        nextButton.gameObject.SetActive(false);
    }


    private void SuscribeToPhotonNetworkManagerEvent()
    {
        PhotonNetworkManager.Instance.OnPlayerPropertiesUpdateEvent += OnUpdateSkinProperties;
        PhotonNetworkManager.Instance.OnPlayerPropertiesUpdateEvent += OnUpdateReadyIndicator;
    }

    private void UnsuscribeToPhotonNetworkManagerEvent()
    {
        PhotonNetworkManager.Instance.OnPlayerPropertiesUpdateEvent -= OnUpdateSkinProperties;
        PhotonNetworkManager.Instance.OnPlayerPropertiesUpdateEvent -= OnUpdateReadyIndicator;
    }

    private void OnUpdateSkinProperties(Player targetPlayer, Hashtable changedProps)
    {
        if (assignedPlayer != null && targetPlayer == assignedPlayer && changedProps.ContainsKey("SkinIndex"))
        {
            int skinIndex = (int)changedProps["SkinIndex"];
            skinPreview.color = PlayerSkinManager.Instance.PlayerSkins[skinIndex];
        }
    }

    private void OnUpdateReadyIndicator(Player targetPlayer, Hashtable changedProps)
    {
        if (assignedPlayer != null && targetPlayer == assignedPlayer && changedProps.ContainsKey("IsReady"))
        {
            bool isReady = (bool)changedProps["IsReady"];
            readyIndicator.color = isReady ? Color.green : Color.red;
        }
    }
}
