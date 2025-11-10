using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FPSDisplayManager : SingletonMonoBehaviour<FPSDisplayManager>
{
    public TextMeshProUGUI fpsText;
    public float updateRate = 0.5f;

    float timer;
    void Awake()
    {
        CreateSingleton(true);
    }

    void Update()
    {
        timer += Time.unscaledDeltaTime;

        if (timer < updateRate) return;

        int fps = (int)(1f / Time.unscaledDeltaTime);
        fpsText.text = fps + " FPS";
        timer = 0f;
    }
}
