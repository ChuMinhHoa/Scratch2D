using System;
using R3;
using UniRx.Triggers;
using UnityEngine;

public class ButtonGameObject : MonoBehaviour
{
    private Action actionCallBack;
    
    public void AddListeners(Action action) => actionCallBack = action;

    public void OnClick()
    {
        actionCallBack?.Invoke();
    }

    public void SetActive(bool active)
    {
        gameObject.SetActive(active);
    }
}
