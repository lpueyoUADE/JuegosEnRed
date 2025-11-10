using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FPSSliderUI : MonoBehaviour
{
    public TextMeshProUGUI valueText;
    public Toggle showToggle;

    [SerializeField] List<int> fps;

    private Slider slider;

    const string SHOW_FPS = "ShowFPS";
    const string FPS_INDEX = "FPSIndex";

    const int DEFAULT_SHOW_FPS_VALUE = 0;
    const int DEFAULT_FPS_INDEX = 1;

    private void Start()
    {
        // Slider
        slider = GetComponent<Slider>();
        slider.maxValue = fps.Count - 1;
        slider.onValueChanged.AddListener(UpdateValueAndText);
        slider.value = PlayerPrefs.GetInt(FPS_INDEX, DEFAULT_FPS_INDEX);       

        // Toggle
        showToggle.onValueChanged.AddListener(ToggleActive);
        print(showToggle.isOn);
        print(PlayerPrefs.GetInt(SHOW_FPS, DEFAULT_SHOW_FPS_VALUE));
        showToggle.isOn = PlayerPrefs.GetInt(SHOW_FPS, DEFAULT_SHOW_FPS_VALUE) == 1;
    }
    private void OnDestroy()
    {
        slider.onValueChanged.RemoveListener(UpdateValueAndText);
        showToggle.onValueChanged.RemoveListener(ToggleActive);
    }

    void UpdateValueAndText(float value)
    {
        int newValue = fps[(int)value];
        valueText.text = newValue.ToString();
        PlayerPrefs.SetInt(FPS_INDEX, (int)value);

        Application.targetFrameRate = newValue;
        QualitySettings.vSyncCount = 0;
    }

    void ToggleActive(bool isActive)
    {
        FPSDisplayManager.Instance.gameObject.SetActive(isActive);
        PlayerPrefs.SetInt(SHOW_FPS, isActive ? 1 : 0);
    }
}
