using Photon.Pun;
using TMPro;
using UnityEngine;

public class PlayerView : MonoBehaviourPun
{
    private TextMeshPro nickNameText;
    private Vector3 initialScale;


    void Awake()
    {
        InitializeNickNameText();
    }

    void LateUpdate()
    {
        UpdateScaleRotationText();
    }


    private void InitializeNickNameText()
    {
        nickNameText =  GetComponentInChildren<TextMeshPro>();
        nickNameText.text = photonView.Owner.NickName;
        initialScale = nickNameText.transform.localScale;
    }

    private void UpdateScaleRotationText()
    {
        if (transform.localScale.x < 0)
        {
            nickNameText.transform.localScale = new Vector3(-initialScale.x, initialScale.y, initialScale.z);
        }

        else
        {
            nickNameText.transform.localScale = initialScale;
        }
    }
}
