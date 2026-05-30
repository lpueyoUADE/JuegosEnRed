using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NotificationsUI : SingletonMonoBehaviour<NotificationsUI>
{
    [Header("Configuración")]
    public GameObject notificationPrefab;      // Prefab con TextMeshProUGUI
    public Transform notificationContainer;    // Contenedor (Panel)
    public int maxNotifications = 5;
    public float fadeDuration = 0.3f;
    public float displayDuration = 3f;

    private List<TextMeshProUGUI> activeNotifications = new List<TextMeshProUGUI>();

    public static event Action<string> OnNotify;
    private static bool notifying = false;
    void Awake()
    {
        CreateSingleton(true);
    }
    private void OnEnable()
    {
        OnNotify -= HandleNotification; // evita doble suscripción
        OnNotify += HandleNotification;
    }

    private void OnDisable()
    {
        OnNotify -= HandleNotification;
    }

    private void HandleNotification(string message)
    {
        //StartCoroutine(AddNotificationRoutine(message));
    }

    private IEnumerator AddNotificationRoutine(string message)
    {
        // Si hay demasiadas, eliminar la más antigua
        if (activeNotifications.Count >= maxNotifications)
        {
            var oldest = activeNotifications[0];
            activeNotifications.RemoveAt(0);
            StartCoroutine(FadeOutAndDestroy(oldest));
        }

        // Instanciar nueva notificación
        var newObj = Instantiate(notificationPrefab, notificationContainer);
        var tmp = newObj.GetComponent<TextMeshProUGUI>();

        if (tmp == null)
        {
            Debug.LogError("El prefab debe tener un TextMeshProUGUI.");
            yield break;
        }

        tmp.text = message;
        tmp.alpha = 0f;

        activeNotifications.Add(tmp);

        // Fade In
        yield return StartCoroutine(Fade(tmp, 0f, 1f, fadeDuration));

        // Mostrar por un tiempo
        yield return new WaitForSeconds(displayDuration);

        // Fade Out y eliminar
        activeNotifications.Remove(tmp);
        yield return StartCoroutine(FadeOutAndDestroy(tmp));
    }

    private IEnumerator Fade(TextMeshProUGUI text, float from, float to, float duration)
    {
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            text.alpha = Mathf.Lerp(from, to, t / duration);
            yield return null;
        }
        text.alpha = to;
    }

    private IEnumerator FadeOutAndDestroy(TextMeshProUGUI text)
    {
        yield return StartCoroutine(Fade(text, text.alpha, 0f, fadeDuration));
        if (text != null)
            Destroy(text.gameObject);
    }

    public static void Notify(string message)
    {
        if (notifying) return; // previene cascadas
        notifying = true;
        OnNotify?.Invoke(message);
        notifying = false;
    }
}
