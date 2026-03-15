using System;
using System.Collections.Generic;
using System.Globalization;
using Core.UI.Activities;
using TW.Utility.DesignPattern;
using UnityEngine;

public class InGamePurchaseManager : Singleton<InGamePurchaseManager>
{
    [field: SerializeField] public List<IAPPackage> IAPPackages { get; private set; }
    private void Start()
    {
        LoadData();
    }

    private void LoadData()
    {
        IAPPackages = new List<IAPPackage>();
        var shopPackages = ShopGlobalConfig.Instance.shopPackage;
        for (var i = 0; i < shopPackages.Length; i++)
        {
            if (shopPackages[i].purchaseType == PurchaseType.IAPPay)
            {
                InitIAPPackage(shopPackages[i].packageName.ToString(), shopPackages[i].price.ToString(CultureInfo.InvariantCulture));
            }
        }
    }

    private void InitIAPPackage(string productId, string strPrice)
    {
        IAPPackage iapPackage = new IAPPackage(productId, strPrice);
        IAPPackages.Add(iapPackage);
        //Debug.Log("<color=red>[Purchase]</color>add package: " + productId+ " price: " + strPrice);
    }
    
    public IAPPackage GetIAPPackageByID(string packageID)
    {
        for (var i = 0; i < IAPPackages.Count; i++)
        {
            //Debug.Log(packageID+ " == "+ IAPPackages[i].ProductID);
            if (IAPPackages[i].ProductID == packageID)
                return IAPPackages[i];
        }

        //Debug.Log("iap package not found: " + packageID);
        return null;
    }
    
    public void PurchaseIAPProduct(string packageID, Action onPurchaseSuccess, Action onPurchaseFailed)
    {
        _ = UIManager.Instance.OpenActivityAsync<ActivityBlock>();
        OnBuyIAP(packageID, onPurchaseSuccess, onPurchaseFailed);
        //InGameAnalyticController.EventTrackIAPClick?.Invoke(packageID, "shop");
    }
    
    private void OnBuyIAP(string productId, Action onBuySuccess, Action onBuyFailed)
    {
        //Debug.Log("OnBuyIAP: " + productId);
        var product = new IAPProduct(productId, onBuySuccess, onBuyFailed);
        Purchaser.Instance.BuyIAPProduct(product);
    }
}
