using System;
using UnityEngine;

public class SelectAbleOnBooster : MonoBehaviour, ISelectAbleOnBooster
{
    public BoosterType boosterActive;
    public IBooster Booster;
    
    private void Start()
    {
        GlobalEventManager.OnBoosterUsing += OnBoosterUsing;
    }

    private void OnBoosterUsing(BoosterType bType, IBooster booster)
    {
        if (bType != boosterActive)
            return;
        Booster = booster;
        OnCanSelect();
    }

    public void OnCanSelect()
    {
        var position = transform.localPosition;
        position.z = -5;
        transform.localPosition = position;
    }

    public void OnSelect()
    {
        Booster?.UsedBooster(this);
    }
}
