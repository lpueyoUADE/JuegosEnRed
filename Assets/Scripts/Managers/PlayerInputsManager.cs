using UnityEngine;

public class PlayerInputsManager : SingletonMonoBehaviour<PlayerInputsManager>
{
    [SerializeField] private Inputs inputsKeyboard;
    [SerializeField] private Inputs inputsJoystick;

    [SerializeField] private bool testJoystickButtonsInDebbuger;


    void Awake()
    {
        CreateSingleton(true);
    }

    void Update()
    {
        TesteJoystickButtonsInDebbuger();
    }


    public Vector2 GetMoveAxis()
    {
        return new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }

    public bool Interact() => Input.GetKeyDown(inputsKeyboard.Interact) || Input.GetKeyDown(inputsJoystick.Interact);
    public bool Attack() => Input.GetKeyDown(inputsKeyboard.Attack) || Input.GetKeyDown(inputsJoystick.Attack);
    public bool Jump() => Input.GetKeyDown(inputsKeyboard.Jump) || Input.GetKeyDown(inputsJoystick.Jump);

    
    private void TesteJoystickButtonsInDebbuger()
    {
        if (testJoystickButtonsInDebbuger)
        {
            for (int i = 0; i < 20; i++)
            {
                if (Input.GetKeyDown(KeyCode.JoystickButton0 + i))
                {
                    Debug.Log("Joystick button " + i + " presionado");
                }
            }
        }
    }
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
