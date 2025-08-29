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
    public bool Attack() => Input.GetKeyDown(inputs.Attack);
    public bool Jump() => Input.GetKeyDown(inputs.Jump);
}

[System.Serializable]
public class Inputs
{
    [SerializeField] private KeyCode interact;
    [SerializeField] private KeyCode attack;
    [SerializeField] private KeyCode jump;

    public KeyCode Interact { get => interact; }
    public KeyCode Attack { get => attack; }
    public KeyCode Jump { get => jump; }
}
