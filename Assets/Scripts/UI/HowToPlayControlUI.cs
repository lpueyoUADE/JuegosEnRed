using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class HowToPlayControlUI : MonoBehaviour
{
    public GameObject joystickPanel;
    public GameObject keyboardPanel;

    private void OnEnable()
    {
        // Escuchamos cambios de dispositivo y cualquier acción de entrada
        InputSystem.onEvent += OnInputEvent;
        UpdatePanel();
    }

    private void OnDisable()
    {
        InputSystem.onEvent -= OnInputEvent;
    }

    private void OnInputEvent(InputEventPtr eventPtr, InputDevice device)
    {
        if (device == null)
            return;

        // Solo procesamos eventos de estado reales
        if (!eventPtr.IsA<StateEvent>() && !eventPtr.IsA<DeltaStateEvent>())
            return;

        // Si es Gamepad mostramos el panel de joystick
        if (device is Gamepad)
        {
            ShowGamepadPanel();
        }
        //  Si es Teclado mostramos el panel de teclado
        else if (device is Keyboard)
        {
            ShowKeyboardPanel();
        }
    }

    private void UpdatePanel()
    {
        bool hasGamepad = Gamepad.all.Count > 0;

        if (hasGamepad)
            ShowGamepadPanel();
        else
            ShowKeyboardPanel();
    }

    private void ShowGamepadPanel()
    {
        if (joystickPanel != null) joystickPanel.SetActive(true);
        if (keyboardPanel != null) keyboardPanel.SetActive(false);
    }

    private void ShowKeyboardPanel()
    {
        if (joystickPanel != null) joystickPanel.SetActive(false);
        if (keyboardPanel != null) keyboardPanel.SetActive(true);
    }
}