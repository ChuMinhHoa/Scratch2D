using System;
using System.Collections.Generic;
using Core.UI.Activities;
using Core.UI.Modals;
using Cysharp.Threading.Tasks;
using TW.Utility.DesignPattern;
using UnityEngine;

public class ShopManager : Singleton<ShopManager>
{
    public Reactive<bool> IsFirstPurchase = new(false);
    public Reactive<bool> NoAds = new(false);
    public Reactive<int> iapCount = new(0);

    public void Start()
    {
        LoadData();
        _ = WaitForSetAds();
    }

    private async UniTask WaitForSetAds()
    {
        await UniTask.WaitForSeconds(1f);
        if (PlayerResourceManager.Instance.gameBuildType == GameBuildType.Cheat)
        {
            NoAds.Value = true;
        }
    }

    private void LoadData()
    {
        IsFirstPurchase = ShopDataSave.Instance.IsFirstPurchase;
        iapCount = ShopDataSave.Instance.iapCount;
        NoAds = ShopDataSave.Instance.NoAds;
    }

    public Span<ShopPackageDataConfig> GetPackageShopConfigs()
    {
        return ShopGlobalConfig.Instance.shopPackage.AsSpan();
    }

    public void PurchaseSuccess(ShopPackageDataConfig packageConfig)
    {
        if (!IsFirstPurchase.Value)
        {
            IsFirstPurchase.Value = true;
            ShopDataSave.Instance.SaveData();
        }

        iapCount.Value++;
        IngameFirebaseAnalystic.Instance.SetUserPropertyIapCount();
        ShopDataSave.Instance.SaveData();
        
        for (var i = 0; i < packageConfig.shopRewards.Count; i++)
        {
            RewardManager.Instance.AddResourceReward(packageConfig.shopRewards[i]);
            var reward = packageConfig.shopRewards[i];
            var rewardType = reward.ResourceType;

            if (rewardType is GameResource.Type.BoosterAddSlot or GameResource.Type.BoosterHammer or GameResource.Type.BoosterMagnet)
                continue;
            
            var isNoAds = packageConfig.shopRewards[i].ResourceType == GameResource.Type.NoAds;
            var currentNoAds = NoAds.Value;
          
            var amount = reward.Amount;
            IngameFirebaseAnalystic.Instance.SetClaimCurrencyType(ClaimCurrencyType.IAP);
            IngameFirebaseAnalystic.Instance.SetCurrencyPlacement(PlacementType.Shop);
            if (isNoAds && currentNoAds)
            {
                IngameFirebaseAnalystic.Instance.TrackCurrencyEarn(GameResource.Type.Money, 5000);
            }
            else
            {
                IngameFirebaseAnalystic.Instance.TrackCurrencyEarn(rewardType, amount.ToInt());
            }
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

    public bool SetNoAds()
    {
        if (NoAds.Value)
            return false;
        Debug.Log("set no ads");
        NoAds.Value = true;
        ShopDataSave.Instance.SaveData();
        return true;
    }
}