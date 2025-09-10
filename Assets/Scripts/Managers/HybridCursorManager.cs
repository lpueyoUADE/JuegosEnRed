using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class HybridCursorManager : SingletonMonoBehaviour<HybridCursorManager>
{
    [SerializeField] private InputActionReference pointAction; // Pointer position (mouse)
    [SerializeField] private InputActionReference moveAction;  // Right stick
    [SerializeField] private InputActionReference clickAction;

    [SerializeField] private RectTransform cursor;
    [SerializeField] private Canvas canvas;

    private PointerEventData pointerData;
    private Vector2 cursorPos;

    [SerializeField] private float joystickSpeed;


    private List<GameObject> hoveredObjects = new List<GameObject>();


    void Awake()
    {
        CreateSingleton(true);
        Initialize();
    }

    void OnEnable()
    {
        pointAction?.action.Enable();
        moveAction?.action.Enable();
        if (clickAction != null)
        {
            clickAction.action.Enable();
            clickAction.action.performed += OnClickPerformed;
        }
    }

    void OnDisable()
    {
        if (clickAction != null)
            clickAction.action.performed -= OnClickPerformed;

        pointAction?.action.Disable();
        moveAction?.action.Disable();
        clickAction?.action.Disable();
    }

    void Update()
    {
        HandleMovement();
        HandleUIInteraction();
    }


    public Vector2 GetCursorPosition()
    {
        return cursorPos;
    }


    private void Initialize()
    {
        Cursor.visible = false;
        pointerData = new PointerEventData(EventSystem.current);
        cursorPos = new Vector2(Screen.width / 2f, Screen.height / 2f);
    }

    private void HandleMovement()
    {
        // 1) Preferimos mouse absoluto si movieron el mouse
        bool mousePresent = Mouse.current != null;
        bool mouseMoved = mousePresent && Mouse.current.delta.ReadValue() != Vector2.zero;

        if (mouseMoved)
        {
            // Mouse position es absoluta (pantalla)
            cursorPos = Mouse.current.position.ReadValue();
        }
        else
        {
            // 2) Si no hubo movimiento de mouse, usamos el stick (delta)
            Vector2 stick = Vector2.zero;
            if (moveAction != null)
                stick = moveAction.action.ReadValue<Vector2>();

            if (stick.sqrMagnitude > 0.0001f)
            {
                cursorPos += stick * joystickSpeed * Time.deltaTime;
                cursorPos.x = Mathf.Clamp(cursorPos.x, 0f, Screen.width);
                cursorPos.y = Mathf.Clamp(cursorPos.y, 0f, Screen.height);
            }
            else
            {
                // Fallback: si el usuario usa Input clásico (legacy), mantenemos compatibilidad
                if (Input.mousePresent && (Mathf.Abs(Input.GetAxis("Mouse X")) > 0 || Mathf.Abs(Input.GetAxis("Mouse Y")) > 0))
                    cursorPos = Input.mousePosition;
            }
        }

        // Convertir a anchoredPosition del rect transform del cursor
        Vector2 localPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            cursorPos,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
            out localPos
        );
        cursor.anchoredPosition = localPos;
    }

    private void HandleUIInteraction()
    {
        pointerData.position = cursorPos;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        List<GameObject> newHovered = new List<GameObject>();
        foreach (var result in results)
        {
            var selectable = result.gameObject.GetComponent<Selectable>();
            if (selectable != null)
            {
                ExecuteEvents.Execute(result.gameObject, pointerData, ExecuteEvents.pointerEnterHandler);
                newHovered.Add(result.gameObject);
            }
        }

        foreach (var obj in hoveredObjects)
        {
            if (!newHovered.Contains(obj))
            {
                ExecuteEvents.Execute(obj, pointerData, ExecuteEvents.pointerExitHandler);
            }
        }

        hoveredObjects = newHovered;
    }

    private void OnClickPerformed(InputAction.CallbackContext ctx)
    {
        if (Mouse.current != null && ctx.control.device == Mouse.current)
            return;

        // 2) Solo ejecutar submitHandler para joystick/gamepad
        pointerData.position = cursorPos;
        pointerData.pointerId = -2; // ID único para joystick
        pointerData.button = PointerEventData.InputButton.Left;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        foreach (var result in results)
        {
            var selectable = result.gameObject.GetComponent<Selectable>();
            if (selectable != null)
            {
                // Esto dispara el onClick automáticamente
                ExecuteEvents.Execute(selectable.gameObject, pointerData, ExecuteEvents.submitHandler);
                return;
            }
        }
    }
}
