using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public class HybridCursorManager : SingletonMonoBehaviour<HybridCursorManager>
{
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
        if (Input.mousePresent && (Mathf.Abs(Input.GetAxis("Mouse X")) > 0 || Mathf.Abs(Input.GetAxis("Mouse Y")) > 0))
        {
            cursorPos = Input.mousePosition;
        }

        float moveX = Input.GetAxisRaw("RightStickHorizontal");
        float moveY = Input.GetAxisRaw("RightStickVertical");

        if (Mathf.Abs(moveX) > 0.1f || Mathf.Abs(moveY) > 0.1f)
        {
            cursorPos += new Vector2(moveX, moveY) * joystickSpeed * Time.deltaTime;

            // Limitar dentro de la pantalla
            cursorPos.x = Mathf.Clamp(cursorPos.x, 0, Screen.width);
            cursorPos.y = Mathf.Clamp(cursorPos.y, 0, Screen.height);
        }

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

        // Raycast actual
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        // PointerEnterExit
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

        // PointerEnter
        foreach (var obj in hoveredObjects)
        {
            if (!newHovered.Contains(obj))
            {
                ExecuteEvents.Execute(obj, pointerData, ExecuteEvents.pointerExitHandler);
            }
        }

        hoveredObjects = newHovered;

        if (Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.JoystickButton1))
        {
            foreach (var result in results)
            {
                ExecuteEvents.Execute(result.gameObject, pointerData, ExecuteEvents.pointerClickHandler);
                return;
            }
        }
    }
}
