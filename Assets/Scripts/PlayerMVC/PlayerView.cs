using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerView : MonoBehaviourPun
{
    private TextMeshPro nicknameText;
    private Slider healthBar;

    private Vector3 textInitialScale;
    private Vector3 sliderInitialScale;


    void Awake()
    {
        SuscribeToPlayerModelEvent();
        GetComponents();
        InitializeRotationLookAt();
        InitializeNickNameText();
        InitializeHealthBarSlider();
    }

    void LateUpdate()
    {
        UpdateScaleRotationText();
        UpdateScaleRotationSlider();
    }

    void OnDestroy()
    {
        UnscribeToPlayerModelEvent();
    }


    private void SuscribeToPlayerModelEvent()
    {
        PlayerModel.OnDisableNicknameText += OnDisbaleNicknameText;
    }

    private void UnscribeToPlayerModelEvent()
    {
        PlayerModel.OnDisableNicknameText -= OnDisbaleNicknameText;
    }


    private void GetComponents()
    {
        nicknameText =  GetComponentInChildren<TextMeshPro>();
        healthBar = GetComponentInChildren<Slider>();
    }

    private void OnDisbaleNicknameText(int viewID)
    {
        if (photonView.ViewID != viewID) return;

        nicknameText.enabled = false;
    }

    private void InitializeRotationLookAt()
    {
        switch (photonView.OwnerActorNr)
        {
            case 1: case 2:
                transform.localScale = new Vector3(1, 1, 1);
                break;

            case 3: case 4:
                transform.localScale = new Vector3(-1, 1, 1);
                break;
        }
    }

    private void InitializeNickNameText()
    {
        nicknameText.text = photonView.Owner.NickName;
        textInitialScale = nicknameText.transform.localScale;
    }

    private void InitializeHealthBarSlider()
    {
        sliderInitialScale = healthBar.transform.localScale;
    }

    private void UpdateScaleRotationText()
    {
        if (transform.localScale.x < 0)
        {
            nicknameText.transform.localScale = new Vector3(-textInitialScale.x, textInitialScale.y, textInitialScale.z);
        }

        else
        {
            nicknameText.transform.localScale = textInitialScale;
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
