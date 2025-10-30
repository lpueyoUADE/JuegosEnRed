using Photon.Pun;
using ExitGames.Client.Photon;
using Photon.Realtime;

public class PodiumManager : SingletonMonoBehaviour<PodiumManager>
{
    void Awake()
    {
        CreateSingleton(true);
    }

    void Start()
    {
        SuscribeToPhotonNetworkManagerEvent();
        InitializeInformation();
    }


    public void AddScore(Player targetPlayer, int amount)
    {
        int currentScore = 0;

        if (targetPlayer.CustomProperties.ContainsKey("Score"))
        {
            currentScore = (int)targetPlayer.CustomProperties["Score"];
        }

        int newScore = currentScore + amount;

        Hashtable props = new Hashtable();
        props["Score"] = newScore;
        targetPlayer.SetCustomProperties(props);
    }

    public void AddDeath(int amount)
    {
        Player localPlayer = PhotonNetwork.LocalPlayer;
        int currentDeaths = 0;

        if (localPlayer.CustomProperties.ContainsKey("Deaths"))
        {
            currentDeaths = (int)localPlayer.CustomProperties["Deaths"];
        }

        int newDeaths = currentDeaths + amount;

        Hashtable props = new Hashtable();
        props["Deaths"] = newDeaths;
        localPlayer.SetCustomProperties(props);
    }


    private void SuscribeToPhotonNetworkManagerEvent()
    {
        PhotonNetworkManager.Instance.OnLeftRoomEvent += OnClearInformationWhenPlayerLeftRoom;
    }

    private void InitializeInformation()
    {
        Hashtable props = new Hashtable
        {
            { "Score", 0 },
            { "Deaths", 0 }
        };

        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }

    private void OnClearInformationWhenPlayerLeftRoom()
    {
        Hashtable props = new Hashtable
        {
            { "Score", 0 },
            { "Deaths", 0 }
        };

        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }
}
