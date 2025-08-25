using Photon.Pun;

public abstract class SingletonMonoBehaviourPunCallbacks<T> : MonoBehaviourPunCallbacks where T : MonoBehaviourPunCallbacks
{
    protected static T instance;

    public static T Instance { get => instance; }


    // El parametro decide si se crea un singleton que no se destruye o si se crea uno que se destruye
    protected virtual void CreateSingleton(bool dontDestroyOnLoad)
    {
        if (instance == null)
        {
            instance = this as T;
        }

        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }

        if (dontDestroyOnLoad)
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}