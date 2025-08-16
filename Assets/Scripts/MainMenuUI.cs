using UnityEngine;
using TMPro;

public class MainMenuUI : MonoBehaviour
{
    [Header("CreateRoomInformation:")]
    [SerializeField] private GameObject createRoomPanel;
    [SerializeField] private TMP_InputField createRoomNameInputField;
    [SerializeField] private TMP_InputField createRoomPasswordInputField;
    [SerializeField] private TMP_Text errorTextCreateRoom;

    [Header("JoinRoomInformation:")]
    [SerializeField] private GameObject joinRoomPanel;
    [SerializeField] private TMP_InputField joinRoomNameInputFiel;
    [SerializeField] private TMP_InputField joinRoomPasswordInputField;
    [SerializeField] private TMP_Text errorTextJoinRoom;


    void Update()
    {
        CleanAllInformation();
    }


    // Funciones asignadas a botones de la UI

    public void ButtonShowCreateRoomPanel()
    {
        createRoomPanel.SetActive(true);
    }

    public void ButtonShowJoinRoomPanel()
    {
        joinRoomPanel.SetActive(true);
    }

    public void ButtonCreateRoom()
    {
        string roomName = createRoomNameInputField.text;
        string roomPassword = createRoomPasswordInputField.text;

        if (roomName.Length < 3 || roomName.Length > 12)
        {
            errorTextCreateRoom.text = "El nombre de la sala debe tener entre 3 y 12 caracteres.";
            return;
        }

        if (roomPassword.Length < 3 || roomPassword.Length > 12)
        {
            errorTextCreateRoom.text = "La contraseña debe tener entre 3 y 12 caracteres.";
            return;
        }

        errorTextCreateRoom.text = string.Empty;

        PhotonNetworkManager.Instance.CreateRoom(roomName, roomPassword);
    }

    public void ButtonJoinRoom()
    {
        string roomName = joinRoomNameInputFiel.text;
        string roomPassword = joinRoomPasswordInputField.text;

        if (roomName.Length < 3 || roomName.Length > 12)
        {
            errorTextJoinRoom.text = "El nombre de la sala debe tener entre 3 y 12 caracteres.";
            return;
        }

        if (roomPassword.Length < 3 || roomPassword.Length > 12)
        {
            errorTextJoinRoom.text = "La contraseña debe tener entre 3 y 12 caracteres.";
            return;
        }

        errorTextJoinRoom.text = string.Empty;

        PhotonNetworkManager.Instance.JoinRoom(roomName, roomPassword);
    }

    public void ButtonSettings()
    {

    }

    public void ButtonExitGame()
    {

    }


    private void InitializeInputFieldsCharactersLimits()
    {
        createRoomNameInputField.characterLimit = 12;
        createRoomPasswordInputField.characterLimit = 12;

        joinRoomNameInputFiel.characterLimit = 12;
        joinRoomPasswordInputField.characterLimit = 12;
    }

    private void CleanAllInformation()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            createRoomPanel.SetActive(false);
            createRoomNameInputField.text = string.Empty;
            createRoomPasswordInputField.text = string.Empty;
            errorTextCreateRoom.text = string.Empty;

            joinRoomPanel.SetActive(false);
            joinRoomNameInputFiel.text = string.Empty;
            joinRoomPasswordInputField.text = string.Empty;
            errorTextJoinRoom.text = string.Empty;
        }
    }
}
