using System;
using Core.UI.Screens;
using Cysharp.Threading.Tasks;
using UnityEngine;

[Serializable]
public class BoosterCart : BoosterBase
{
    public override void UseBooster()
    {
        if (GamePlayManager.Instance.gameState != GameState.Playing)
            return;
        if (!CheckOnUseCart())
        {
            ShowWarning();
            return;
        }

        ScreenGamePlayContext.Events.OnActiveInteractable?.Invoke(false);
        GlobalEventManager.OnBoosterUsing?.Invoke(boosterType, this);
        _ = WaitForUseBooster();
    }

    private async UniTask WaitForUseBooster()
    {
        await Level.Instance.fSpaceController.cartBooster.OnUseCardBooster(this);
        UsedBooster(null);
    }

    public override void ShowWarning()
    {
        GlobalEventManager.OnShowWarning?.Invoke(MyCache.warningNoStickerOnFS);
    }

    public override void ActiveBooster(bool active)
    {
        base.ActiveBooster(active);
        Debug.Log($"Active Booster {active}");
    }

    public override void UsedBooster(SelectAbleOnBooster data)
    {
        base.UsedBooster(data);
        Debug.Log($"Used Booster with data: {data}");
        ScreenGamePlayContext.Events.OnActiveInteractable?.Invoke(true);
    }

    private FolderHaveSticker GetFolderHaveSticker(SelectAbleOnBooster data)
    {
        var folder = data.GetComponent<FolderHaveSticker>();
        return folder;
    }

    public override void OnChangeBoosterCount(int count)
    {
        base.OnChangeBoosterCount(count);
        Debug.Log($"Change Booster count: {count}");
    }
    
    public override bool CheckCanUseBooster()
    {
        return true;
     
    }

    private bool CheckOnUseCart()
    {
        var e = Level.Instance.CheckCanUsingBooster(boosterType);
        return e;
    }
    
}
