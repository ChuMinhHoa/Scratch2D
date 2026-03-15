using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

[CreateAssetMenu(fileName = "ShopGlobalConfig", menuName = "GlobalConfigs/ShopGlobalConfig")]
[GlobalConfig("Assets/Resources/GlobalConfig/")]
public class ShopGlobalConfig : GlobalConfig<ShopGlobalConfig>
{
    public ShopPackageDataConfig[] shopPackage;

    public ShopPackageDataConfig GetPackageShopConfig(PackageName packageName)
    {
        for (var i = 0; i < shopPackage.Length; i++)
        {
            if (shopPackage[i].packageName == packageName)
                return shopPackage[i];
        }

        return null;
    }
}

[Serializable]
public class ShopPackageDataConfig
{
    [HideLabel, PreviewField] public Sprite mainIcon;

    public PackageName packageName;

    public string packageNameToUI;

    public string packageSaleOffTag;

    public float percentSaleOff;

    public string packageDes;

    public PurchaseType purchaseType;
    public ProductType productType;

    [ShowIf("@this.purchaseType == PurchaseType.IAPPay")]
    public float price;

    [ShowIf("@this.purchaseType == PurchaseType.ResourcePay")]
    public GameResource resourcePrice;

    public List<GameResource> gameResources;

    public bool isAvailable;
}

public enum PackageName
{
    coin1 = 100,
    coin2 = 101,
    coin3 = 102,
    coin4 = 103,
    coin5 = 104,
    coin6 = 105,
}

public enum ProductType
{
    Consumable = 0,
    NonConsumable = 1,
    Subscription = 2
}

public enum PurchaseType
{
    None = 0,
    ResourcePay = 1,
    IAPPay = 2,
    Free = 3,
    Ads = 4
}