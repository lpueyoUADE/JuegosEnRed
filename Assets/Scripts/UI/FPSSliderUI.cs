using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FPSSliderUI : MonoBehaviour
{
    public TextMeshProUGUI valueText;
    public Toggle showToggle;
    public Slider slider;

    [SerializeField] List<int> fps;

    const string SHOW_FPS = "ShowFPS";
    const string FPS_INDEX = "FPSIndex";

    const int DEFAULT_SHOW_FPS_VALUE = 60;
    const int DEFAULT_FPS_INDEX = 1;


    void Start()
    {
        slider.maxValue = fps.Count - 1;
        slider.onValueChanged.AddListener(UpdateValueAndText);

        int savedIndex = PlayerPrefs.GetInt(FPS_INDEX, fps.IndexOf(DEFAULT_SHOW_FPS_VALUE));
        slider.value = savedIndex;

        int initialFPS = fps[savedIndex];
        valueText.text = initialFPS.ToString();
        Application.targetFrameRate = initialFPS;
        QualitySettings.vSyncCount = 0;

        showToggle.onValueChanged.AddListener(ToggleActive);
        showToggle.isOn = PlayerPrefs.GetInt(SHOW_FPS, 0) == 1;
        if (showToggle.isOn && FPSDisplayManager.Instance != null)
            FPSDisplayManager.Instance.gameObject.SetActive(true);
    }

    void OnDestroy()
    {
        if (slider != null)
        slider.onValueChanged.RemoveListener(UpdateValueAndText);
        
        if (showToggle != null)
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
