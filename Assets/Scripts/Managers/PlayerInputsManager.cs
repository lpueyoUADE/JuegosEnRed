using UnityEngine;

public class PlayerInputsManager : MonoBehaviour
{
    private static PlayerInputsManager instance;

    [SerializeField] private Inputs inputs;

    public static PlayerInputsManager Instance { get =>  instance; } 
    
    public Inputs Inputs { get => inputs; }


    void Awake()
    {
        CreateSingleton();
    }


    public Vector2 GetMoveAxis()
    {
        return new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
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
}

[System.Serializable]
public class Inputs
{
    [SerializeField] private KeyCode shoot;

    public KeyCode Shoot { get => shoot; }
}
