using UnityEngine;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;

public class PodiumUI : MonoBehaviour
{
    [SerializeField] private PodiumPlayerSlot[] podiumPlayerSlots;

    [SerializeField] private GameObject panelBackToMainMenu;


    void Awake()
    {
        HybridCursorManager.Instance.SetUIPointer();
        AssignPodiumPlayers();
    }
    private void Start()
    {
        AudioManager.Instance.PlayMusic(MusicTrack.Victory);
    }
    void Update()
    {
        ShowOrHidePanelToGoBackToMainMenu();
    }


    // Funcion asignada a boton de la UI
    public void ButtonYes()
    {
        PhotonNetworkManager.Instance.LeaveRoom();
    }

    // Funcion asignada a boton de la UI
    public void ButtonNo()
    {
        panelBackToMainMenu.SetActive(false);
    }


    private void AssignPodiumPlayers()
    {
        Player[] players = PhotonNetwork.PlayerList;

        // Ordenamos los jugadores
        var orderedPlayers = players
            .OrderByDescending(p => p.CustomProperties.ContainsKey("Score") ? (int)p.CustomProperties["Score"] : 0) // Mayor puntaje primero
            .ThenBy(p => p.CustomProperties.ContainsKey("Deaths") ? (int)p.CustomProperties["Deaths"] : 0)         // Menos muertes primero
            .ToList();

        for (int i = 0; i < podiumPlayerSlots.Length; i++)
        {
            if (i < orderedPlayers.Count)
            {
                podiumPlayerSlots[i].AssignPlayerInfoToSlot(orderedPlayers[i]);
            }

            else
            {
                podiumPlayerSlots[i].gameObject.SetActive(false);
            }
        }
    }

    private void ShowOrHidePanelToGoBackToMainMenu()
    {
        if (PlayerInputsManager.Instance.Settings() && panelBackToMainMenu.activeSelf)
        {
            panelBackToMainMenu.SetActive(false);
            return;
        }

        else if (PlayerInputsManager.Instance.Settings() && !panelBackToMainMenu.activeSelf)
        {
            panelBackToMainMenu.SetActive(true);
            return;
        }
    }
}
