using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Analytics;

public class AnalyticsManager : MonoBehaviour
{
    async void Start()
    {
       
        try
        {
            await UnityServices.InitializeAsync();
            Debug.Log("Unity Services inicializado correctamente");

            AskForConsent();
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error al inicializar servicios: " + e.Message);
        }
    }

    void AskForConsent()
    {
        Debug.Log("Iniciando recolección de datos...");
        AnalyticsService.Instance.StartDataCollection();
    }
}