using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class ScenesManager : MonoBehaviour
{
    private static ScenesManager instance;

    [SerializeField] private GameObject loadingScenePanel;
    [SerializeField] private GameObject exitGamePanel;

    [SerializeField] private float duringTimeLoadingScenePanel;
    [SerializeField] private float duringTimeExitGamePanel;

    private bool isInLoadingScenePanel = false;
    private bool isInExitGamePanel = false;

    public static ScenesManager Instance { get => instance; }

    //public bool IsInLoadingScenePanel { get => isInLoadingScenePanel; }
    //public bool IsInExitGamePanel { get => isInExitGamePanel; }


    void Awake()
    {
        CreateSingleton();
    }

    void Start()
    {
        SuscribeToSceneLoadedEvent();
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

    private void CreateSingleton()
    {
        if (instance == null)
        {
            instance = this;
        }

        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    private void SuscribeToSceneLoadedEvent()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Game" || scene.name == "Room" || scene.name == "MainMenu")
        {
            StartCoroutine(ShowLoadingPanel());
        }
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
    }
}