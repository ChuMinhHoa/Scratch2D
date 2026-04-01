using System;
using UnityEngine;

[Serializable]
public class BoosterCart : BoosterBase
{
    public int stickerID;
    
    public override void UseBooster()
    {
        if (GamePlayManager.Instance.gameState != GameState.Playing)
            return;
        //ScreenGamePlayContext.Events.OnActiveInteractable?.Invoke(false);
        //GamePlayManager.Instance.SetWhatCanSelectOnBooster(layerCanSelect);
        //_ = UIManager.Instance.OpenActivityAsync<ActivityUsingBooster>();
        Level.Instance.fSpaceController.cartBooster.OnUseCardBooster(this);
        Debug.Log("Use Booster Cart");
        UsedBooster(null);
    }
    
    public override void ShowWarning()
    {
        GlobalEventManager.OnShowWarning?.Invoke(MyCache.warningNoteOnMove);
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
        var e = Level.Instance.fSpaceController.cartBooster.IsCanUseBoosterCart();
        return true;
    }
    
}
