using UnityEngine;

public class PlayerInputsManager : SingletonMonoBehaviour<PlayerInputsManager>
{
    [SerializeField] private Inputs inputs;
    
    public Inputs Inputs { get => inputs; }


    void Awake()
    {
        CreateSingleton(true);
    }


    public Vector2 GetMoveAxis()
    {
        return new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }
}

[System.Serializable]
public class Inputs
{
    [SerializeField] private KeyCode shoot;

    public KeyCode Shoot { get => shoot; }
}
