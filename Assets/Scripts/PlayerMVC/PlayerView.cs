using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerView : MonoBehaviourPun
{
    private TextMeshPro nickNameText;
    private Slider healthBar;

    private Vector3 textInitialScale;
    private Vector3 sliderInitialScale;


    void Awake()
    {
        GetComponents();
        InitializeNickNameText();
        InitializeHealthBarSlider();
    }

    void LateUpdate()
    {
        UpdateScaleRotationText();
        UpdateScaleRotationSlider();
    }


    private void GetComponents()
    {
        nickNameText =  GetComponentInChildren<TextMeshPro>();
        healthBar = GetComponentInChildren<Slider>();
    }

    private void InitializeNickNameText()
    {
        nickNameText.text = photonView.Owner.NickName;
        textInitialScale = nickNameText.transform.localScale;
    }

    private void InitializeHealthBarSlider()
    {
        sliderInitialScale = healthBar.transform.localScale;
    }

    private void UpdateScaleRotationText()
    {
        if (transform.localScale.x < 0)
        {
            nickNameText.transform.localScale = new Vector3(-textInitialScale.x, textInitialScale.y, textInitialScale.z);
        }

        else
        {
            nickNameText.transform.localScale = textInitialScale;
        }
    }

    private void UpdateScaleRotationSlider()
    {
        if (transform.localScale.x < 0)
        {
            healthBar.transform.localScale = new Vector3(-sliderInitialScale.x, sliderInitialScale.y, sliderInitialScale.z);
        }
        else
        {
            healthBar.transform.localScale = sliderInitialScale;
        }
    }
}
