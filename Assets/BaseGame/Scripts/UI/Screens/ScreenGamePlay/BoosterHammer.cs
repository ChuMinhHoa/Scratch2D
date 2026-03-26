using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BoosterHammer : BoosterBase
{
    Dictionary<SelectAbleOnBooster, Card> dataCard = new();
    public override void UseBooster()
    {
        base.UseBooster();
        Debug.Log("Use Booster Hammer");
    }

    public override void ActiveBooster(bool active)
    {
        base.ActiveBooster(active);
        Debug.Log($"Active Booster {active}");
    }

    public override void UsedBooster(SelectAbleOnBooster data)
    {
        base.UsedBooster(data);
        var card = data.GetComponent<Card>();
        card.OnUnlockCardByBooster();
    }

    public override void OnChangeBoosterCount(int count)
    {
        base.OnChangeBoosterCount(count);
        Debug.Log($"Change Booster count: {count}");
    }

    public override bool CheckCanUseBooster()
    {
        var e = Level.Instance.CheckCanUsingBooster(boosterType);
        return e;
    }
    
    public override void ShowWarning()
    {
        GlobalEventManager.OnShowWarning?.Invoke(MyCache.warningHammer);
    }
}
