using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSLimiter : MonoBehaviour
{
    [SerializeField] int fps=60;
    private void Awake()
    {
        Application.targetFrameRate = fps;
        QualitySettings.vSyncCount = 0;
    }
}
