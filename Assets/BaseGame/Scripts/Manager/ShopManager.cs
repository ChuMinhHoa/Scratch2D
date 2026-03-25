using System;
using System.Collections.Generic;
using Core.UI.Activities;
using Core.UI.Modals;
using Cysharp.Threading.Tasks;
using TW.Utility.DesignPattern;
using UnityEngine;

public class ShopManager : Singleton<ShopManager>
{
    public Reactive<bool> IsFirstPurchase = new (false);
    public void Start()
    {
        LoadData();
    }

    private void LoadData()
    {
        IsFirstPurchase = ShopDataSave.Instance.IsFirstPurchase;
    }

    public Span<ShopPackageDataConfig> GetPackageShopConfigs()
    {
        return ShopGlobalConfig.Instance.shopPackage.AsSpan();
    }

    public void PurchaseSuccess(ShopPackageDataConfig packageConfig)
    {
        for (var i = 0; i < packageConfig.shopRewards.Count; i++)
        {
            RewardManager.Instance.AddResourceReward(packageConfig.shopRewards[i]);
        }

        _ = DelayPurChaseSuccess();
    }
    
    private async UniTask DelayPurChaseSuccess()
    {
        await UniTask.Delay(1000);
        await UIManager.Instance.CloseActivityAsync<ActivityBlock>();
        RewardManager.Instance.ShowReward();
    }

    public void PurchaseFailed(ShopPackageDataConfig packageConfig)
    {
        _ = DelayPurChaseFailed();
    }
    
    private async UniTask DelayPurChaseFailed()
    {
        await UniTask.Delay(1000);
        await UIManager.Instance.CloseActivityAsync<ActivityBlock>();
        //await UIManager.Instance.OpenModalAsync<ModalPurchaseFail>();
    }
}