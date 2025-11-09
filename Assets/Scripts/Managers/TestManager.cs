using UnityEngine;

public class TestManager : SingletonMonoBehaviour<TestManager>
{
    [SerializeField] private bool useTestSystem;

    public bool UseTestSystem { get => useTestSystem; }


    void Awake()
    {
        CreateSingleton(true);
    }
}
