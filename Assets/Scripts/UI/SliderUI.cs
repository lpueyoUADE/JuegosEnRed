using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SliderUI : MonoBehaviour
{
    public TextMeshProUGUI percentageText;

    private Slider slider;

    void Awake()
    {
        slider = GetComponent<Slider>();

        slider.onValueChanged.Invoke(slider.value);
        UpdateText(slider.value);

        slider.onValueChanged.AddListener(UpdateText);
    }

    void UpdateText(float value)
    {
        int percent = Mathf.RoundToInt(value * 100f);
        percentageText.text = percent + "%";
    }
}
