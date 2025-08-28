using UnityEngine;

public class PlayerInputsManager : SingletonMonoBehaviour<PlayerInputsManager>
{
    [SerializeField] private Inputs inputs;
    

    void Awake()
    {
        CreateSingleton(true);
    }


    public Vector2 GetMoveAxis()
    {
        return new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }

    public bool Interact() => Input.GetKeyDown(inputs.Interact);
}

[System.Serializable]
public class Inputs
{
    [SerializeField] private KeyCode shoot;
    [SerializeField] private KeyCode interact;

    public KeyCode Shoot { get => shoot; }
    public KeyCode Interact { get => interact; }
}
