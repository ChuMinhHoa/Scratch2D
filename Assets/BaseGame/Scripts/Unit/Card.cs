using System;
using ScratchCardAsset;
using UnityEngine;
using UniRx.Triggers;
using UniRx;

public class Card : MonoBehaviour
{
    public ScratchCardManager scratchCardManager;

    public void SetActionCallbackChangeProgress(Action<float> callback)
    {
        scratchCardManager.SetActionCallBackChangeProgress(callback);
    }

    public void RemoveActionCallbackChangeProgress(Action<float> callback)
    {
        scratchCardManager.RemoveActionCallBackChangeProgress(callback);
    }
    
    private void OnMouseEnter()
    {
        // if (UnityEngine.EventSystems.EventSystem.current != null &&
        //     UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
        // {
        //     Debug.Log("Click is blocked by UI.");
        //     return;
        // }
        //
        // if (GamePlayManager.Instance.IsCurrentCard(this))
        //     return;
        // GamePlayManager.Instance.SetCurrentCard(this);
    }

    private void OnMouseExit()
    {
        // if (GamePlayManager.Instance.IsCurrentCard(this))
        //     GamePlayManager.Instance.SetCurrentCard(null);
    }
}