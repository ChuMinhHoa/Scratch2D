using System;
using UnityEngine;

[Serializable]
public class BoosterHammer : BoosterBase
{
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
    }

    public override void OnChangeBoosterCount(int count)
    {
        base.OnChangeBoosterCount(count);
        Debug.Log($"Change Booster count: {count}");
    }
}
