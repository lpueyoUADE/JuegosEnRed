using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class RoomUI : MonoBehaviour
{
    [SerializeField] private Button buttonStartGame;
    [SerializeField] private TMP_Text buttonStartGameText;
    [SerializeField] private GameObject panelBackToMainMenu;

    [Header("PlayerInformation:")]
    [SerializeField] private RoomPlayerSlot[] roomPlayerSlots;


    void Awake()
    {
        SuscribeToPhotonNetworkManagerEvents();
        RefreshSlots();
    }

    void Update()
    {
        // Test para empezar a jugar sin que haya otro jugador en la room
        if (Input.GetKeyDown(KeyCode.T))
        {
            ScenesManager.Instance.LoadScene("Game");
        }

        ShowPanelToGoBackToMainMenu();
    }

    void OnDestroy()
    {
        UnsuscribeToPhotonNetworkManagerEvents();
    }


    // Funciones asignada a boton de la UI
    public void ButtonStartGame()
    {
        PhotonNetworkManager.Instance.CloseRoom();
        ScenesManager.Instance.LoadScene("Game");
    }

    public void ButtonStartGameOnPointerEnter()
    {
        if (PhotonNetworkManager.Instance.GetCurrentPlayersCountInRoom() < 2)
        {
            buttonStartGameText.gameObject.SetActive(true);
        }
    }

    public void ButtonStartGameOnPointerExit()
    {
        if (buttonStartGameText.gameObject.activeSelf)
        {
            buttonStartGameText.gameObject.SetActive(false);
        }
    }

    public void ButtonSelectNextSkin(int direction)
    {
        //PlayerSkinManager.Instance.ChangeSkinIndex(currentSkinPreview, direction);
    }

    public void ButtonYes()
    {
        PhotonNetworkManager.Instance.LeaveRoom();
    }

    public void ButtonNo()
    {
        panelBackToMainMenu.SetActive(false);
    }


    private void SuscribeToPhotonNetworkManagerEvents()
    {
        PhotonNetworkManager.Instance.OnJoinedRoomEvent += OnShowButtonStartGameIfIsHost;
        PhotonNetworkManager.Instance.OnPlayerEnteredRoomEvent += OnChangeButtonStartGameInteraction;
        PhotonNetworkManager.Instance.OnPlayerEnteredRoomEvent += RefreshSlots;
        PhotonNetworkManager.Instance.OnPlayerLeftRoomEvent += OnChangeButtonStartGameInteraction;
        PhotonNetworkManager.Instance.OnPlayerLeftRoomEvent += RefreshSlots;
    }

    private void UnsuscribeToPhotonNetworkManagerEvents()
    {
        PhotonNetworkManager.Instance.OnJoinedRoomEvent -= OnShowButtonStartGameIfIsHost;
        PhotonNetworkManager.Instance.OnPlayerEnteredRoomEvent -= OnChangeButtonStartGameInteraction;
        PhotonNetworkManager.Instance.OnPlayerEnteredRoomEvent -= RefreshSlots;
        PhotonNetworkManager.Instance.OnPlayerLeftRoomEvent -= OnChangeButtonStartGameInteraction;
        PhotonNetworkManager.Instance.OnPlayerLeftRoomEvent -= RefreshSlots;
    }

    private void OnShowButtonStartGameIfIsHost()
    {
        buttonStartGame.gameObject.SetActive(false);

        if (PhotonNetworkManager.Instance.IsHost)
        {
            buttonStartGame.gameObject.SetActive(true);
        }
    }

    private void OnChangeButtonStartGameInteraction()
    {
        if (PhotonNetworkManager.Instance.GetCurrentPlayersCountInRoom() > 1)
        {
            buttonStartGame.interactable = true;

            if (buttonStartGameText.gameObject.activeSelf)
            {
                buttonStartGameText.gameObject.SetActive(false);
            }
        }

        else
        {
            buttonStartGame.interactable = false;
        }
    }

    private void RefreshSlots()
    {
        foreach (var slot in roomPlayerSlots)
        {
            slot.ClearPlayerInfoFromSlot();
        }

        // Asignar jugadores actuales
        Player[] players = PhotonNetwork.PlayerList;
        for (int i = 0; i < players.Length; i++)
        {
            roomPlayerSlots[i].AssignPlayerInfoToSlot(players[i]);
        }
    }

    private void ShowPanelToGoBackToMainMenu()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && panelBackToMainMenu.activeSelf)
        {
            panelBackToMainMenu.SetActive(false);
            return;
        }

        else if (Input.GetKeyDown(KeyCode.Escape) && !panelBackToMainMenu.activeSelf)
        {
            panelBackToMainMenu.SetActive(true);
            return;
        }
    }
}
