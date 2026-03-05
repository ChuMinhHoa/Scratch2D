using System;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class SelectAbleOnBooster : MonoBehaviour, ISelectAbleOnBooster
{
    public BoosterType boosterActive;
    public IBooster Booster;
    public bool onCanSelect;
    public Vector3 defaultPos;

    private void Start()
    {
        GlobalEventManager.OnBoosterUsing += OnBoosterUsing;
        GlobalEventManager.OnBoosterDone += OnBoosterDone;
    }

    private void OnBoosterDone()
    {
        onCanSelect = false;
        var position = transform.localPosition;
        position.z = 0;
        transform.localPosition = position;
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
        onCanSelect = true;
        var position = transform.localPosition;
        defaultPos = position;
        position.z = -5;
        transform.localPosition = position;
    }

    public void OnSelect()
    {
        Booster?.UsedBooster(this);
        GlobalEventManager.OnBoosterDone?.Invoke();
    }
}