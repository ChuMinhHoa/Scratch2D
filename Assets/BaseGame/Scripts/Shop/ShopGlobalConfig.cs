using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using TW.Utility.CustomType;
using TW.Utility.Extension;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

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

#if UNITY_EDITOR
    private string linkSheet = "1NFKTM7gS7x6asEz9v1yLLLSAO73L2oM0ljQ6wj5dnqI";
    private string sheetTab = "ShopConfig";
    [ShowInInspector] private List<Dictionary<string, string>> tableData;

    [Button]
    private async void FetchData()
    {
        var listShopPackageDataConfig = new List<ShopPackageDataConfig>();
        tableData = await ABakingSheet.GetDataTable(linkSheet, sheetTab);

        foreach (var data in tableData)
        {
            if (!data["ID"].Equals(""))
            {
                var sprIcon = AssetDatabase.LoadAssetAtPath<Sprite>(
                    @"Assets\BaseGame\Graphic\Sprites\UI\08_Shop\PackIcon\" + data["Name"] + ".png");
                var des = "";
                if (data.ContainsKey("Des") && !data["Des"].Equals("")) des = data["Des"];
                var newShopConfig = new ShopPackageDataConfig
                {
                    mainIcon = sprIcon,
                    packageName = (PackageName)Enum.Parse(typeof(PackageName), data["ID"]),
                    packageNameToUI = data["Name"],
                    packageDes = des,
                    price = float.Parse(data["Price"]),
                    purchaseType = PurchaseType.IAPPay,
                    shopRewards = new List<GameResource>(),
                    isAvailable = true
                };
                listShopPackageDataConfig.Add(newShopConfig);
            }

            if (data["RewardType"].Equals("")) continue;

            var strRewardType = data["RewardType"];
            var isAllBooster = strRewardType.Equals("AllBooster");
            var rewardAmount = float.Parse(data["Amount"]);
            Debug.Log(rewardAmount);
            if (isAllBooster)
            {
                for (var i = 5; i < 9; i++)
                {
                    var rewardType = (GameResource.Type)i;
                    var gameResource = new GameResource(rewardType, rewardAmount);
                    listShopPackageDataConfig[^1].shopRewards.Add(gameResource);
                }
            }
            else
            {
                var rewardType = (GameResource.Type)Enum.Parse(typeof(GameResource.Type), data["RewardType"]);
                var gameResource = new GameResource(rewardType, rewardAmount);
                listShopPackageDataConfig[^1].shopRewards.Add(gameResource);
            }
        }

        shopPackage = listShopPackageDataConfig.ToArray();
        
    }
#endif
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
    public PackProductType packProductType;

    [ShowIf("@this.purchaseType == PurchaseType.IAPPay")]
    public float price;

    [ShowIf("@this.purchaseType == PurchaseType.ResourcePay")]
    public GameResource resourcePrice;

    public List<GameResource> shopRewards;

    public bool isAvailable;
}

public enum PackageName
{
    removeAds = 0,
    removeAdsBundle = 1,
    limitedBundle = 2,
    smallBundle = 3,
    mediumBundle = 4,
    ultraBundle = 5,
    coin1 = 100,
    coin2 = 101,
    coin3 = 102,
    coin4 = 103,
    coin5 = 104,
    coin6 = 105,
}

public enum PackProductType
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