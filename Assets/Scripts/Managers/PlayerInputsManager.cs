using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputsManager : SingletonMonoBehaviour<PlayerInputsManager>
{
    [SerializeField] private InputActionReference interactInput;
    [SerializeField] private InputActionReference attackInput;
    [SerializeField] private InputActionReference jumpInput;
    [SerializeField] private InputActionReference backUI;
    [SerializeField] private InputActionReference tabUI;
    [SerializeField] private InputActionReference settings;


    void Awake()
    {
        CreateSingleton(true);
    }

    void OnEnable()
    {
        InitializeInput();
    }


    public Vector2 GetMoveAxis()
    {
        return new Vector2(Input.GetAxisRaw("Horizontal"), 0);
    }

    public bool Interact() => interactInput.action.WasPressedThisFrame();
    public bool Attack() => attackInput.action.WasPressedThisFrame();
    public bool Jump() => jumpInput.action.WasPressedThisFrame();
    public bool BackUI() => backUI.action.WasPressedThisFrame();
    public bool TabUI() => tabUI.action.IsPressed();
    public bool Settings() => settings.action.WasPerformedThisFrame();


    private void InitializeInput()
    {
        jumpInput?.action.Enable();
        attackInput?.action.Enable();
        interactInput?.action.Enable();
        backUI?.action.Enable();
        tabUI?.action.Enable();
        settings?.action.Enable();
    }
}
