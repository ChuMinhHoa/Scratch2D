using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class BtnHoldAction : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private Action pointerDownAction;
    private Action pointerExitAction;
    
    public void SetPointerDownAction(Action action)
    {
        pointerDownAction = action;
    }
    
    public void SetPointerExitAction(Action action)
    {
        pointerExitAction = action;
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        pointerDownAction?.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        pointerExitAction?.Invoke();
    }
}
