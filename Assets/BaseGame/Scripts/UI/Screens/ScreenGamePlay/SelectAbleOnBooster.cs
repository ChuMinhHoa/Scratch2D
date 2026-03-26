using System;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class SelectAbleOnBooster : MonoBehaviour, ISelectAbleOnBooster
{
    public BoosterType boosterActive;
    public IBooster Booster;
    public bool onCanSelect;
    public Vector3 defaultPos;

    private Func<bool> conditionToSelect;
    
    public bool CheckCondition => conditionToSelect();

    public void SetConditionToSelect(Func<bool> condition)
    {
        conditionToSelect = condition;
    }

    private void Start()
    {
        GlobalEventManager.OnBoosterUsing += OnBoosterUsing;
        GlobalEventManager.OnBoosterDone += OnBoosterDone;
    }

    private void OnDestroy()
    {
        GlobalEventManager.OnBoosterUsing -= OnBoosterUsing;
        GlobalEventManager.OnBoosterDone -= OnBoosterDone;
    }

    private void OnBoosterDone()
    {
        if (!onCanSelect)
            return;
        onCanSelect = false;
        transform.localPosition = defaultPos;
    }

    public void OnBoosterUsing(BoosterType bType, IBooster booster)
    {
        if (bType != boosterActive)
            return;
        
        if (conditionToSelect != null)
        {
            var e = conditionToSelect();
            if (!e) return;
        }
        
        Booster = booster;
        OnCanSelect();
    }

    public void OnCanSelect()
    {
        onCanSelect = true;
        var position = transform.localPosition;
        defaultPos = transform.localPosition;
        position.z = -5;
        transform.localPosition = position;
    }

    public void OnSelect()
    {
        Booster?.UsedBooster(this);
    }
}