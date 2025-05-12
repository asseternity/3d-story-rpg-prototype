using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class ButtonHoverHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public UnityAction enterCallback;
    public UnityAction exitCallback;

    public void OnPointerEnter(PointerEventData eventData)
    {
        enterCallback?.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        exitCallback?.Invoke();
    }
}
