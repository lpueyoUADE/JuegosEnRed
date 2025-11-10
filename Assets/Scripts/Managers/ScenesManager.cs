using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using System;

public enum GameScenes
{
    MainMenu, Room, Level1, Level2, Level3, Podium
}

public class ScenesManager : SingletonMonoBehaviour<ScenesManager>
{
    private event Action onSceneGameLoaded;

    [SerializeField] private GameObject connectingToOnlineServicesPanel;
    [SerializeField] private GameObject loadingScenePanel;
    [SerializeField] private GameObject exitGamePanel;

    [SerializeField] private float duringTimeconnectingToOnlineServicesPanel;
    [SerializeField] private float duringTimeLoadingScenePanel;
    [SerializeField] private float duringTimeExitGamePanel;

    private bool isInLoadingScenePanel = false;
    private bool isInExitGamePanel = false;

    public Action OnSceneGameLoaded { get => onSceneGameLoaded; set => onSceneGameLoaded = value; }

    public bool IsInLoadingScenePanel { get => isInLoadingScenePanel; }
    public bool IsInExitGamePanel { get => isInExitGamePanel; }


    void Awake()
    {
        CreateSingleton(true);
    }

    void Start()
    {
        StartCoroutine(InitializeBootstrapScene());
        SuscribeToSceneLoadedEvent();
        SuscribeToPhotonNetworkManager();

        connectingToOnlineServicesPanel.SetActive(true);
    }


    // Para pasar de una escena a otra
    public void LoadScene(string sceneName)
    {
        PhotonNetwork.LoadLevel(sceneName);        
    }

    // Para cerrar el juego
    public IEnumerator ExitGame()
    {
        exitGamePanel.SetActive(true);
        isInExitGamePanel = true;

        yield return new WaitForSecondsRealtime(duringTimeExitGamePanel);

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }


    private IEnumerator InitializeBootstrapScene()
    {
        AsyncOperation loadOpDataScene = SceneManager.LoadSceneAsync("Bootstrap", LoadSceneMode.Additive);

        yield return new WaitUntil(() => loadOpDataScene.isDone);

        SceneManager.UnloadSceneAsync("Bootstrap");
    }

    private void SuscribeToSceneLoadedEvent()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void SuscribeToPhotonNetworkManager()
    {
        PhotonNetworkManager.Instance.OnConnectedToMasterEvent += HandleShowConnectingToOnlineServicesPanel;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (Enum.TryParse(scene.name, out GameScenes parsedScene))
        {
            StartCoroutine(ShowLoadingPanel());
        }
    }

    private IEnumerator ShowConnectingToOnlineServicesPanel()
    {
        yield return new WaitForSecondsRealtime(duringTimeconnectingToOnlineServicesPanel);

        connectingToOnlineServicesPanel.SetActive(false);
    }

    private IEnumerator ShowLoadingPanel()
    {
        loadingScenePanel.SetActive(true);
        isInLoadingScenePanel = true;

        float elapsedTime = 0f;
        float waitingTime = 1f;

        while (elapsedTime < waitingTime)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        StartCoroutine(DisableLoadingScenePanelAfterSeconds());
    }

    private IEnumerator DisableLoadingScenePanelAfterSeconds()
    {
        yield return new WaitForSecondsRealtime(duringTimeLoadingScenePanel);

        isInLoadingScenePanel = false;
        loadingScenePanel.SetActive(false);

        Scene scene = SceneManager.GetActiveScene();

        if (scene.name.StartsWith("Level"))
        {
            onSceneGameLoaded?.Invoke();
        }
    }

    private void HandleShowConnectingToOnlineServicesPanel()
    {
        StartCoroutine(ShowConnectingToOnlineServicesPanel());
    }
}