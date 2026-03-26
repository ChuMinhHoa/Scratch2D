using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TW.Utility.DesignPattern;
using UnityEngine;

public class BoosterManager : Singleton<BoosterManager>
{
    Dictionary<Collider2D, SelectAbleOnBooster> selectAbleOnBoosters = new Dictionary<Collider2D, SelectAbleOnBooster>();
    public bool onUsingBooster;
    [SerializeField] private BoosterGraphicControl[] boosterGraphicControls;
    private BoosterType currentBoosterType;
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
        currentBoosterType = arg1;
        GamePlayManager.Instance.ChangeGameState(GameState.OnBooster);
    }

    public async UniTask ChooseObjOnBooster(Collider2D col)
    {
        var sBo = GetSelectAbleOnBooster(col);
        if (sBo == null) return;
        if (!sBo.CheckCondition) return;
       
        var pos = col.transform;
        for (var i = 0; i < boosterGraphicControls.Length; i++)
        {
            if (boosterGraphicControls[i].bType == currentBoosterType)
            {
                await boosterGraphicControls[i].MoveBoosterTo(pos);
                sBo.OnSelect();
                return;
            }
        }
        
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
