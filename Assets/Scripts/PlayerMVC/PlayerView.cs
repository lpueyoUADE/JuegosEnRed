using Photon.Pun;
using TMPro;

public class PlayerView : MonoBehaviourPun
{
    private TextMeshPro nickNameText;


    void Awake()
    {
        InitializeNickNameText();
    }


    private void InitializeNickNameText()
    {
        nickNameText =  GetComponentInChildren<TextMeshPro>();
        nickNameText.text = photonView.Owner.NickName;
    }
}
