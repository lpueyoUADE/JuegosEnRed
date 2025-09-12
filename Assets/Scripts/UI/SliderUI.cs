using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SliderUI : MonoBehaviour
{
    public enum VolumeType { Master, Music, SFX }
    
    public TextMeshProUGUI percentageText;
    private Slider slider;

    [Header("Volume Type")]
    [SerializeField] private VolumeType volumeType;

    private void Start()
    {
        UpdateFromManager();
        slider.onValueChanged.AddListener(UpdateVolumeAndText);
    }
    private void Awake()
    {
        slider = GetComponent<Slider>();
        AudioManager.InitCompleted += UpdateFromManager;
    }
    private void OnDestroy()
    {
        slider.onValueChanged.RemoveListener(UpdateVolumeAndText);
        AudioManager.InitCompleted -= UpdateFromManager;
    }

    void UpdateFromManager()
    {
        switch (volumeType)
        {
            case VolumeType.Master:
                slider.SetValueWithoutNotify(AudioManager.Instance.masterVolume);
                break;

            case VolumeType.Music:
                slider.SetValueWithoutNotify(AudioManager.Instance.musicVolume);
                break;

            case VolumeType.SFX:
                slider.SetValueWithoutNotify(AudioManager.Instance.sfxVolume);
                break;
        }

        UpdateText(slider.value);
    }

    void UpdateVolumeAndText(float value)
    {
        switch (volumeType)
        {
            case VolumeType.Master:
                AudioManager.Instance.SetMasterVolume(value);
                break;

            case VolumeType.Music:
                AudioManager.Instance.SetMusicVolume(value);
                break;

            case VolumeType.SFX:
                AudioManager.Instance.SetSFXVolume(value);
                break;
        }

        UpdateText(value);
    }

    void UpdateText(float value)
    {
        int percent = Mathf.RoundToInt(value * 100f);
        percentageText.text = percent + "%";
    }
}
