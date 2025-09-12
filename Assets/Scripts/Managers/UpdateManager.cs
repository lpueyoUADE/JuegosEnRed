using System;

public class UpdateManager : SingletonMonoBehaviour<UpdateManager>
{
    private static event Action onUpdate;
    private static event Action onFixedUpdate;

    public static Action OnUpdate { get => onUpdate; set => onUpdate = value; }
    public static Action OnFixedUpdate { get => onFixedUpdate; set => onFixedUpdate = value; }


    void Awake()
    {
        CreateSingleton(true);
    }

    void Update()
    {
        if (!ScenesManager.Instance.IsInLoadingScenePanel && TimeManager.Instance.IsCountDownFinished)
        {
            onUpdate?.Invoke();
        }
    }

    void FixedUpdate()
    {
        if (!ScenesManager.Instance.IsInLoadingScenePanel && TimeManager.Instance.IsCountDownFinished)
        {
            onFixedUpdate?.Invoke();
        }
    }
}
