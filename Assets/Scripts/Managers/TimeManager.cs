using System.Collections;
using UnityEngine;

public class TimeManager : SingletonMonoBehaviour<TimeManager>
{
    /// <summary>
    /// Agregar que cuando se reinicia la escena se ponga el booleano en false
    /// </summary>

    [SerializeField] private TMPro.TextMeshProUGUI countdownText;
    [SerializeField] private float countdownTime;

    private bool isCountDownFinished = false;

    public bool IsCountDownFinished { get => isCountDownFinished; }


    void Awake()
    {
        CreateSingleton(true);
    }

    void Start()
    {
        SuscribeToScenesManagerEvent();
    }


    private void SuscribeToScenesManagerEvent()
    {
        ScenesManager.Instance.OnSceneGameLoaded += Countdown;
    }

    private void Countdown()
    {
        StartCoroutine(StartCountdown());
    }

    private IEnumerator StartCountdown()
    {
        AudioManager.Instance.PlaySoundChoice(SoundEffect.Countdown);
        float timer = countdownTime;
        while (timer > 0)
        {
            countdownText.text = Mathf.Ceil(timer).ToString();
            timer -= Time.deltaTime;
            yield return null;
        }

        countdownText.text = "GO!";
        isCountDownFinished = true;
        yield return new WaitForSeconds(1.5f);
        countdownText.text = string.Empty;
    }
}
