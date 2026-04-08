using System;
using System.Collections.Generic;
using Core.UI.Activities;
using Core.UI.Modals;
using Core.UI.Screens;
using Cysharp.Threading.Tasks;
using TW.Utility.CustomType;
using TW.Utility.DesignPattern;
using UnityEngine;

public class ShopManager : Singleton<ShopManager>
{
    public Reactive<bool> IsFirstPurchase = new(false);
    public Reactive<bool> NoAds = new(false);
    public Reactive<int> iapCount = new(0);
    public List<PackageName> packNoneConsumeAbleBought = new();
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
        packNoneConsumeAbleBought = ShopDataSave.Instance.PackNoneConsumeAbleBought;
        IsFirstPurchase = ShopDataSave.Instance.IsFirstPurchase;
        iapCount = ShopDataSave.Instance.iapCount;
        NoAds = ShopDataSave.Instance.NoAds;
    }

    public Span<ShopPackageDataConfig> GetPackageShopConfigs()
    {
        return ShopGlobalConfig.Instance.shopPackage.AsSpan();
    }

    public void Purchase(ShopPackageDataConfig packageConfig)
    {
        switch (packageConfig.purchaseType)
        {
            case PurchaseType.IAPPay:
                IngameFirebaseAnalystic.Instance.SetPlacementPurchase(PlacementType.ShopInGame);
                var packageId = MyCache.GetPackageIdByPackageName(packageConfig.packageName);
                InGamePurchaseManager.Instance.PurchaseIAPProduct(packageId,
                    () => PurchaseSuccess(packageConfig),
                    () => PurchaseFailed(packageConfig));
                break;
            case PurchaseType.ResourcePay:
                OnPurchaseBuyResourcePay(packageConfig);
                break;
            case PurchaseType.None:
            case PurchaseType.Free:
            case PurchaseType.Ads:
            default:
                break;
        }
    }
    
    private void OnPurchaseBuyResourcePay(ShopPackageDataConfig packageConfig)
    {
        GameResource.Type resourceType = packageConfig.resourcePrice.ResourceType;
        BigNumber amount = packageConfig.resourcePrice.Amount;
        if (PlayerResourceManager.Instance.IsEnoughResource(resourceType, amount))
        {
            //_ = UIManager.Instance.OpenModalAsync<ModalConfirmShop>(packageConfig);
        }
        else
        {
            //_ = UIManager.Instance.OpenModalAsync<ModalPayResourcePremium>(packageConfig);
        }
    }

    public void PurchaseSuccess(ShopPackageDataConfig packageConfig)
    {
        if (!IsFirstPurchase.Value)
        {
            IsFirstPurchase.Value = true;
        }

        if (packageConfig.packProductType == PackProductType.NonConsumable)
        {
            packNoneConsumeAbleBought.Add(packageConfig.packageName);
            ScreenShopContext.Events.OnBuyNonConsumeAblePackage?.Invoke(packageConfig.packageName);
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
                var isBooster = IsBooster(packageConfig.shopRewards[i].ResourceType);
                if (!isBooster)
                    IngameFirebaseAnalystic.Instance.TrackCurrencyEarn(rewardType, amount.ToInt());
                else
                {
                    IngameFirebaseAnalystic.Instance.SetBoosterPlacement(PlacementType.Shop);
                    IngameFirebaseAnalystic.Instance.TrackBoosterEarn(rewardType, amount.ToInt());
                }
            }
        }

        _ = DelayPurChaseSuccess();
    }

    private bool IsBooster(GameResource.Type resourceType)  
    {
        switch (resourceType)
        {
          
            case GameResource.Type.BoosterCart:
            case GameResource.Type.BoosterMagnet:
            case GameResource.Type.BoosterAddSlot:
            case GameResource.Type.BoosterHammer:
                return true;
            case GameResource.Type.None:
            case GameResource.Type.Money:
            case GameResource.Type.Gem:
            case GameResource.Type.Energy:
            case GameResource.Type.NoAds:
            default:
                return false;
        }
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
        await UIManager.Instance.OpenModalAsync<ModalPurchaseFaild>();
        await UIManager.Instance.CloseActivityAsync<ActivityBlock>();
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

    public bool IsBuyThisPackage(PackageName packID)
    {
        return packNoneConsumeAbleBought.Contains(packID);
    }
}