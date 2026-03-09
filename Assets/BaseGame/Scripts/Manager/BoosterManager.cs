using System;
using System.Collections.Generic;
using TW.Utility.DesignPattern;
using UnityEngine;

public class BoosterManager : Singleton<BoosterManager>
{
    Dictionary<Collider2D, SelectAbleOnBooster> selectAbleOnBoosters = new Dictionary<Collider2D, SelectAbleOnBooster>();
    public bool onUsingBooster;

    private void Start()
    {
        GlobalEventManager.OnBoosterUsing += OnUsingBooster;
        GlobalEventManager.OnBoosterDone += OnUseBoosterDone;
    }

    private void OnUseBoosterDone()
    {
        onUsingBooster = false;
        GamePlayManager.Instance.ChangeGameState(GameState.Playing);
    }

    private void OnUsingBooster(BoosterType arg1, IBooster arg2)
    {
        onUsingBooster = true;
        GamePlayManager.Instance.ChangeGameState(GameState.OnBooster);
    }
    
    

    public void ChooseObjOnBooster(Collider2D col)
    {
        var sBo = GetSelectAbleOnBooster(col);
        if (sBo == null) return;
        sBo.OnSelect();
    }

    private SelectAbleOnBooster GetSelectAbleOnBooster(Collider2D col)
    {
        if (selectAbleOnBoosters.TryGetValue(col, out var booster))
        {
            return booster;
        }

        booster = col.GetComponent<SelectAbleOnBooster>();
        selectAbleOnBoosters.Add(col, booster);
        return booster;
    }
}
