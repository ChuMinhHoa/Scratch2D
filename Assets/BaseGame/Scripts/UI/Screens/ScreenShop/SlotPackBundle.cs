using Cysharp.Text;
using UnityEngine;

public class SlotPackBundle : SlotPack
{
    [SerializeField] private SlotRewardShop[] rewardSlots;

    public override void InitData(ShopPackageDataConfig data)
    {
        base.InitData(data);
        imgIcon.sprite = data.mainIcon;
        txtName.SetTextFormat(MyCache.strDefault, data.packageNameToUI);
        txtDes?.SetTextFormat(MyCache.strDefault, data.packageDes);

        if(!CheckToShowBundle()) return;
        
        // var productType = data.packProductType;
        // if (productType == PackProductType.NonConsumable)
        // {
        //     var isShow = data.isAvailable && !ShopManager.Instance.IsBuyThisPackage(data.packageName);
        //     if (!isShow)
        //     {
        //         gameObject.SetActive(false);
        //         return;
        //     }
        // }

        if (data.purchaseType == PurchaseType.IAPPay)
        {
            iAPPackage =
                InGamePurchaseManager.Instance.GetIAPPackageByID(MyCache.GetPackageIdByPackageName(data.packageName));
//            Debug.Log($"<color=red> {iAPPackage == null}");
            txtPrice.SetTextFormat(MyCache.strDefault, iAPPackage?.GetPrice());
        }
        else if (data.purchaseType == PurchaseType.ResourcePay)
        {
            var style = MyCache.GetTextResourceStyle(data.resourcePrice.ResourceType);
            txtPrice.textStyle = style;
            txtPrice.SetTextFormat(MyCache.textFormatFloat, data.resourcePrice.Amount.ToFloat());
        }

        if (rewardSlots.Length == 0)
            return;
        for (var i = 0; i < data.shopRewards.Count; i++)
        {
            if (i >= rewardSlots.Length) break;
            rewardSlots[i].gameObject.SetActive(true);
            rewardSlots[i].InitData(data.shopRewards[i]);
        }

        for (var i = data.shopRewards.Count; i < rewardSlots.Length; i++)
        {
            rewardSlots[i].gameObject.SetActive(false);
        }
    }

    public bool CheckToShowBundle()
    {
        var productType = slotData.packProductType;
        if (productType != PackProductType.NonConsumable) return true;
        var isShow = slotData.isAvailable && !ShopManager.Instance.IsBuyThisPackage(slotData.packageName);
        if (isShow) return true;
        gameObject.SetActive(false);
        return false;

    }
}
