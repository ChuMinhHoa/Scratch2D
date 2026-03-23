using System;
using UnityEngine;

[Serializable]
public class BoosterAddSlot : BoosterBase
{
    public override void UseBooster()
    {
        if (GamePlayManager.Instance.gameState != GameState.Playing)
            return;
        if (!Level.Instance.fSpaceController.IsCanAddSlot())
        {
            GlobalEventManager.OnShowWarning?.Invoke(MyCache.warningSlotAdded);
            return;
        }
        //GlobalEventManager.OnBoosterUsing?.Invoke(boosterType, this);
        Level.Instance.AddSlot();
        Debug.Log("Use Booster Magnet");
        UsedBooster(null);
    }

    public override void ActiveBooster(bool active)
    {
        base.ActiveBooster(active);
        Debug.Log($"Active Booster {active}");
        CheckActiveAddSlot();
    }

    public override void UsedBooster(SelectAbleOnBooster data)
    {
        base.UsedBooster(data);
        Debug.Log($"Used Booster with data: {data}");
    }

    public override void OnChangeBoosterCount(int count)
    {
        base.OnChangeBoosterCount(count);
        Debug.Log($"Change Booster count: {count}");
    }

    public void CheckActiveAddSlot()
    {
        var e = Level.Instance.fSpaceController.spaceStickers.Count < 5;
        ActiveBooster(e);
    }

}
