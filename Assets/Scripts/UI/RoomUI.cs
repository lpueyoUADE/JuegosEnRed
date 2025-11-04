using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomUI : MonoBehaviour
{
    private PhotonView photonView;
    private Coroutine countdownCoroutine;

    [SerializeField] private Button buttonReadyOrNot;
    [SerializeField] private TMP_Text buttonReadyOrNotText;
    [SerializeField] private TMP_Text countdownText;
    [SerializeField] private GameObject panelBackToMainMenu;

    [Header("PlayerInformation:")]
    [SerializeField] private RoomPlayerSlot[] roomPlayerSlots;


    void Awake()
    {
        SuscribeToPhotonNetworkManagerEvents();
        GetComponents();
    }

    void Start()
    {
        HybridCursorManager.Instance.SetUIPointer();
    }

    void Update()
    {
        // Test para empezar a jugar sin que haya otro jugador en la room
        if (Input.GetKeyDown(KeyCode.T) && PhotonNetworkManager.Instance.IsHost)
        {
            ScenesManager.Instance.LoadScene("Level1");
        }

        ShowOrHidePanelToGoBackToMainMenu();
    }

    void OnDestroy()
    {
        UnsuscribeToPhotonNetworkManagerEvents();
    }


    // Funciones asignada a boton de la UI
    public void ButtonReadyOrNot()
    {
        Player localPlayer = PhotonNetwork.LocalPlayer;

        bool currentReadyState = localPlayer.CustomProperties.ContainsKey("IsReady") && (bool)localPlayer.CustomProperties["IsReady"];
        bool newReadyState = !currentReadyState;
        buttonReadyOrNotText.text = newReadyState ? "CANCEL" : "READY";
        Hashtable props = new Hashtable();
        props["IsReady"] = newReadyState;
        localPlayer.SetCustomProperties(props);
    }

    public void ButtonStartGameOnPointerEnter()
    {
        if (PhotonNetworkManager.Instance.GetCurrentPlayersCountInRoom() == 1)
        {
            buttonReadyOrNotText.gameObject.SetActive(true);
        }
    }

    public void ButtonStartGameOnPointerExit()
    {
        if (buttonReadyOrNotText.gameObject.activeSelf)
        {
            buttonReadyOrNotText.gameObject.SetActive(false);
        }
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
        PhotonNetworkManager.Instance.OnPlayerPropertiesUpdateEvent += OnPlayerReadyStateChanged;
        PhotonNetworkManager.Instance.OnJoinedRoomEvent += OnSelectableTrueButtonReadyOrNot;
        PhotonNetworkManager.Instance.OnJoinedRoomEvent += OnRefreshSlots;
        PhotonNetworkManager.Instance.OnPlayerEnteredRoomEvent += OnChangeButtonReadyOrNotInteraction;
        PhotonNetworkManager.Instance.OnPlayerEnteredRoomEvent += OnRefreshSlots;
        PhotonNetworkManager.Instance.OnPlayerEnteredRoomEvent += OnStopStartingGameIfCurrentPlayersChange;
        PhotonNetworkManager.Instance.OnPlayerLeftRoomEvent += OnChangeButtonReadyOrNotInteraction;
        PhotonNetworkManager.Instance.OnPlayerLeftRoomEvent += OnRefreshSlots;
        PhotonNetworkManager.Instance.OnPlayerLeftRoomEvent += OnStopStartingGameIfCurrentPlayersChange;
    }

    private void UnsuscribeToPhotonNetworkManagerEvents()
    {
        PhotonNetworkManager.Instance.OnPlayerPropertiesUpdateEvent -= OnPlayerReadyStateChanged;
        PhotonNetworkManager.Instance.OnJoinedRoomEvent -= OnSelectableTrueButtonReadyOrNot;
        PhotonNetworkManager.Instance.OnJoinedRoomEvent -= OnRefreshSlots;
        PhotonNetworkManager.Instance.OnPlayerEnteredRoomEvent -= OnChangeButtonReadyOrNotInteraction;
        PhotonNetworkManager.Instance.OnPlayerEnteredRoomEvent -= OnRefreshSlots;
        PhotonNetworkManager.Instance.OnPlayerEnteredRoomEvent -= OnStopStartingGameIfCurrentPlayersChange;
        PhotonNetworkManager.Instance.OnPlayerLeftRoomEvent -= OnChangeButtonReadyOrNotInteraction;
        PhotonNetworkManager.Instance.OnPlayerLeftRoomEvent -= OnRefreshSlots;
        PhotonNetworkManager.Instance.OnPlayerLeftRoomEvent -= OnStopStartingGameIfCurrentPlayersChange;
    }

    private void OnPlayerReadyStateChanged(Player targetPlayer, Hashtable changedProps)
    {
        if (changedProps.ContainsKey("IsReady"))
        {
            if (PhotonNetworkManager.Instance.IsHost)
            {
                bool isReady = (bool)changedProps["IsReady"];
                if (!isReady && countdownCoroutine != null)
                {
                    StopCoroutine(countdownCoroutine);
                    countdownCoroutine = null;
                    photonView.RPC("UpdateCountdownText", RpcTarget.All, -1); // -1 indica reset
                    return;
                }

                // Si nadie se bajó, seguimos chequeando si todos están ready
                CheckAllPlayersReady();
            }
        }
    }

    private void OnSelectableTrueButtonReadyOrNot()
    {
        if (PhotonNetworkManager.Instance.GetCurrentPlayersCountInRoom() > 1)
        {
            buttonReadyOrNot.interactable = true;
        }
    }

    private void OnChangeButtonReadyOrNotInteraction()
    {
        if (PhotonNetworkManager.Instance.GetCurrentPlayersCountInRoom() > 1)
        {
            buttonReadyOrNot.interactable = true;

            if (buttonReadyOrNotText.gameObject.activeSelf)
            {
                buttonReadyOrNotText.gameObject.SetActive(false);
            }
        }

        else
        {
            buttonReadyOrNot.interactable = false;
        }
    }

    private void OnStopStartingGameIfCurrentPlayersChange()
    {
        if (countdownCoroutine != null)
        {
            StopCoroutine(countdownCoroutine);
            countdownCoroutine = null;
        }

        countdownText.text = "Waiting for all players to be ready";
    }

    private void OnRefreshSlots()
    {
        foreach (var slot in roomPlayerSlots)
        {
            slot.ClearPlayerInfoFromSlot();
        }

        // Asignar jugadores actuales
        Player[] players = PhotonNetwork.PlayerList;
        for (int i = 0; i < 4; i++)
        {
            if (i < players.Length)
            {
                roomPlayerSlots[i].AssignPlayerInfoToSlot(players[i]);
            }
        }
    }

    private void GetComponents()
    {
        photonView = GetComponent<PhotonView>();    
    }

    private void CheckAllPlayersReady()
    {
        Player[] players = PhotonNetwork.PlayerList;
        foreach (var player in players)
        {
            if (!player.CustomProperties.ContainsKey("IsReady") || !(bool)player.CustomProperties["IsReady"])
            {
                return; // Si alguien no esta lista finalizar
            }
        }

        if (countdownCoroutine == null)
        {
            countdownCoroutine = StartCoroutine(StartGameCountdown());
        }
    }

    private System.Collections.IEnumerator StartGameCountdown()
    {
        float countdown = 4f;
        while (countdown > 0)
        {
            photonView.RPC("UpdateCountdownText", RpcTarget.All, Mathf.CeilToInt(countdown));

            yield return new WaitForSeconds(1f);
            countdown -= 1f;
        }

        photonView.RPC("UpdateCountdownText", RpcTarget.All, 0);
        yield return new WaitForSeconds(1f);

        if (PhotonNetworkManager.Instance.IsHost)
        {
            ScenesManager.Instance.LoadScene("Level1");
            PhotonNetworkManager.Instance.CloseRoom();
        }

        countdownCoroutine = null;
    }

    [PunRPC]
    private void UpdateCountdownText(int timeLeft)
    {
        if (timeLeft > 0)
        {
            countdownText.text = $"Game starting in {timeLeft}...";
        }

        else if (timeLeft == 0)
        {
            countdownText.text = "Starting!";
        }

        else if (timeLeft == -1) // Valor para reiniciar el mensaje
        {
            countdownText.text = "Waiting for all players to be ready";
        }
    }

    private void ShowOrHidePanelToGoBackToMainMenu()
    {
        if (PlayerInputsManager.Instance.BackUI() && panelBackToMainMenu.activeSelf)
        {
            panelBackToMainMenu.SetActive(false);
            return;
        }

        else if (PlayerInputsManager.Instance.BackUI() && !panelBackToMainMenu.activeSelf)
        {
            panelBackToMainMenu.SetActive(true);
            return;
        }
    }
}
