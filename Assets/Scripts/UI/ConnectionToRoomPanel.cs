using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ConnectionToRoomPanel : MonoBehaviour
{
    public TMP_Text errorMessage;
    public TMP_InputField nickname;
    public TMP_InputField roomName;
    public TMP_InputField password;

    private void Awake()
    {
        roomName.characterLimit = 12;
        password.characterLimit = 12;
    }

    public bool ValidateConnectionRoom()
    {
        string nickName = nickname.text;
        string roomName = this.roomName.text;
        string roomPassword = password.text;

        errorMessage.text = "";

        if (nickName.Length < 3 || nickName.Length > 12)
        {
            errorMessage.text = "The nick name must be between 3 and 12 characters.";
            return false;
        }

        if (roomName.Length < 3 || roomName.Length > 12)
        {
            errorMessage.text = "The room name must be between 3 and 12 characters.";
            return false;
        }

        if (roomPassword.Length < 3 || roomPassword.Length > 12)
        {
            errorMessage.text = "The password must be between 3 and 12 characters.";
            return false;
        }

        return true;
    }
    public void CleanData()
    {
        errorMessage.text = "";
        nickname.text = "";
        roomName.text = "";
        password.text = "";
    }
}
