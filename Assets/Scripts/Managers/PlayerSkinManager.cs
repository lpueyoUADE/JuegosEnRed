using UnityEngine;
using UnityEngine.UI;
using ExitGames.Client.Photon;
using Photon.Pun;
using System.Collections.Generic;

public class PlayerSkinManager : SingletonMonoBehaviour<PlayerSkinManager>
{
    [SerializeField] private Color[] playersSkins;

    private int skinIndex = 0;

    private bool[] skinsOccupied;

    public Color[] PlayerSkins { get => playersSkins; }


    void Awake()
    {
        CreateSingleton(true);
    }

    void Start()
    {
        SuscribeToPhotonNetworkManagerEvent();
        InitializeSkinsOccupied();
    }


    public void ChangeSkin(Image roomUISkinPreview, int direction)
    {
        int originalIndex = skinIndex;

        do
        {
            skinIndex += direction;

            if (skinIndex >= playersSkins.Length) skinIndex = 0;
            if (skinIndex < 0) skinIndex = playersSkins.Length - 1;

            // Si paso por todos los inidices y no hay ninguna skin disponible terminar el metodo
            if (skinIndex == originalIndex && !IsSkinAvailable(skinIndex))
            {
                return;
            }

        } while (!IsSkinAvailable(skinIndex));

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
        List<int> availableSkins = new List<int>();

        for (int i = 0; i < playersSkins.Length; i++)
        {
            if (IsSkinAvailable(i))
            {
                availableSkins.Add(i);
            }
        }

        if (availableSkins.Count == 0)
        {
            return;
        }

        skinIndex = availableSkins[Random.Range(0, availableSkins.Count)];

        Hashtable props = new Hashtable();
        props["SkinIndex"] = skinIndex;
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }

    private void InitializeSkinsOccupied()
    {
        skinsOccupied = new bool[playersSkins.Length];
    }

    private bool IsSkinAvailable(int index)
    {
        foreach (Photon.Realtime.Player p in PhotonNetwork.PlayerList)
        {
            if (p.CustomProperties.ContainsKey("SkinIndex") && (int)p.CustomProperties["SkinIndex"] == index)
            {
                return false;
            }
        }

        return true;
    }
}
