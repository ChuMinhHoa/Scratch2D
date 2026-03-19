using System;
using Core.UI.Activities;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Object = System.Object;

public interface IBooster
{
    void InitData(UnityAction actionCall);
    void UseBooster();
    void UsedBooster(SelectAbleOnBooster data);
    void ActiveBooster(bool active);
    void OnChangeBoosterCount(int count);
    void SetUsedCallBack(Action actionCallback);
    void SetUsingType(BoosterUseType uType);
}

[Serializable]
public class BoosterBase : IBooster
{
    public BoosterType boosterType;
    public TextMeshProUGUI boosterText;
    public Action actionUsedCallBack;
    
    public Reactive<bool> isCanUse = new(false);
    public LayerMask layerCanSelect;
    public BoosterUseType useType;
    
    public virtual void InitData(UnityAction actionCall)
    {
        
    }

    public virtual void UseBooster()
    {
        if (GamePlayManager.Instance.gameState != GameState.Playing)
            return;
        GamePlayManager.Instance.SetWhatCanSelectOnBooster(layerCanSelect);
        _ = UIManager.Instance.OpenActivityAsync<ActivityUsingBooster>();
        GlobalEventManager.OnBoosterUsing?.Invoke(boosterType, this);
    }

    public virtual void UsedBooster(SelectAbleOnBooster data)
    {
        actionUsedCallBack?.Invoke();
    }

    public virtual void ActiveBooster(bool active)
    {
        
    }

    public virtual void OnChangeBoosterCount(int count)
    {
        boosterText.text = count.ToString();
    }

    public void SetUsedCallBack(Action actionCallback)
    {
        actionUsedCallBack = actionCallback;
    }

    public void SetUsingType(BoosterUseType uType)
    {
        useType = uType;
    }
}

