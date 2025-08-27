using UnityEngine;
using UnityEngine.UI;
using ExitGames.Client.Photon;
using Photon.Pun;

public class PlayerSkinManager : SingletonMonoBehaviour<PlayerSkinManager>
{
    [SerializeField] private Color[] playersSkins;

    private int skinIndex = 0;

    public Color[] PlayerSkins { get => playersSkins; }


    void Awake()
    {
        CreateSingleton(true);
    }

    void Start()
    {
        SuscribeToPhotonNetworkManagerEvent();
    }


    public void ChangeSkin(Image roomUISkinPreview, int direction)
    {
        skinIndex += direction;

        if (skinIndex >= playersSkins.Length)
        {
            skinIndex = 0; 
        }

        else if (skinIndex < 0)
        {
            skinIndex = playersSkins.Length - 1;
        }

        roomUISkinPreview.color = playersSkins[skinIndex];

        Hashtable props = new Hashtable();
        props["SkinIndex"] = skinIndex;
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }


    private void SuscribeToPhotonNetworkManagerEvent()
    {
        PhotonNetworkManager.Instance.OnJoinedRoomEvent += OnInitializeRandomSkin;
    }

    private void OnInitializeRandomSkin()
    {
        int randomColor = Random.Range(0, playersSkins.Length);
        skinIndex = randomColor;

        Hashtable props = new Hashtable();
        props["SkinIndex"] = skinIndex;
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }
}
