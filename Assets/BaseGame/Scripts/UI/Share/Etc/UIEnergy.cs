using System;
using Core.UI.Modals;
using Core.UI.Screens;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class UIEnergy : UIResource
{
}

public class ActionCallOnResource
{
    public virtual void ActionCallOnUIResource()
    {
    }
}

public class ActionCallOnEnergy : ActionCallOnResource
{
    public override void ActionCallOnUIResource()
    {
        base.ActionCallOnUIResource();
        //Debug.Log("Action call on energy");
        _ = OpenModalEnergy();
    }

    private async UniTask OpenModalEnergy()
    {
        await UIManager.Instance.OpenModalAsync<ModalRefill>();
    }
}

public class ActionCallOnMoneyInGame : ActionCallOnResource
{
    public override void ActionCallOnUIResource()
    {
        base.ActionCallOnUIResource();
        //Debug.Log("Action call on money");
        _ = GoToShop();
    }

    private async UniTask GoToShop()
    {
        GamePlayManager.Instance.ChangeGameState(GameState.Normal);
        await UIManager.Instance.OpenScreenDefaultAsync<ScreenShopInGame>(
            (Action)GamePlayManager.Instance.BackToLastState);
    }
}

public class ActionCallOnMoneyHome : ActionCallOnResource
{
    public override void ActionCallOnUIResource()
    {
        base.ActionCallOnUIResource();
        //Debug.Log("Action call on money");
        ScreenDefaultContext.Events.GoToTabEvent?.Invoke(SlotTabType.Shop);
        //_ = GoToShop();
    }
}

public class ActionCallOnMoneyRevive: ActionCallOnResource
{
    public override void ActionCallOnUIResource()
    {
        base.ActionCallOnUIResource();
        //Debug.Log("Action call on money");
        _ = GoToShop();
    }

    private async UniTask GoToShop()
    {
        await UIManager.Instance.CloseModalAsync();
        await UIManager.Instance.OpenScreenDefaultAsync<ScreenShopInGame>((Action)(() => OpenModalRevive().Forget()));
    }
    
    
    private async UniTask OpenModalRevive()
    {
        await UIManager.Instance.OpenModalAsync<ModalRevive>();
    }
}
