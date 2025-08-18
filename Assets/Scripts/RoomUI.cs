using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RoomUI : MonoBehaviour
{
    [SerializeField] private Button buttonStartGame;
    [SerializeField] private TMP_Text buttonStartGameText;
    [SerializeField] private GameObject panelBackToMainMenu;

    [Header("SkinInformation:")]
    [SerializeField] private Image currentSkinPreview;


    void Awake()
    {
        SuscribeToPhotonNetworkManagerEvents();
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

    public void ButtonSelectNextSkin()
    {
        PlayerSkinManager.Instance.ChangeSkinIndex(currentSkinPreview);
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
        PhotonNetworkManager.Instance.OnPlayerLeftRoomEvent += OnChangeButtonStartGameInteraction;
        PhotonNetworkManager.Instance.OnJoinedRoomEvent += OnSetRandomColorSkin; 
    }

    private void UnsuscribeToPhotonNetworkManagerEvents()
    {
        PhotonNetworkManager.Instance.OnJoinedRoomEvent -= OnShowButtonStartGameIfIsHost;
        PhotonNetworkManager.Instance.OnPlayerEnteredRoomEvent -= OnChangeButtonStartGameInteraction;
        PhotonNetworkManager.Instance.OnPlayerLeftRoomEvent -= OnChangeButtonStartGameInteraction;
        PhotonNetworkManager.Instance.OnJoinedRoomEvent -= OnSetRandomColorSkin;
    }

    private void OnShowButtonStartGameIfIsHost()
    {
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

    private void OnSetRandomColorSkin()
    {
        currentSkinPreview.color = PlayerSkinManager.Instance.FirstRandomSkin;
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
