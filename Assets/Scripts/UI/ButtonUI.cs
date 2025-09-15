using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    [Header("Sonidos")]
    public SoundEffect hoverSound = SoundEffect.None;
    public SoundEffect clickSound = SoundEffect.None;

    private bool isHovering = false;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isHovering && hoverSound != SoundEffect.None)
        {
            isHovering = true;
            AudioManager.Instance.PlaySound(hoverSound);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (clickSound != SoundEffect.None)
            AudioManager.Instance.PlaySound(clickSound);
    }
}