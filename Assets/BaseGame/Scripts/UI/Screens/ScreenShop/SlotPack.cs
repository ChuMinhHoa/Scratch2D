using System;
using System.Linq;
using Cysharp.Text;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class SlotPack : SlotBase<ShopPackageDataConfig>
{
    [SerializeField] private PackageName packName;
    public TextMeshProUGUI txtPrice;
    public TextMeshProUGUI txtName;
    public TextMeshProUGUI txtDes;
    public IAPPackage iAPPackage;

    private void Start()
    {
        InitData();
    }

    [Button]
    public virtual void InitData()
    {
        var data = ShopGlobalConfig.Instance.GetPackageShopConfig(packName);
        if(data == null)
        {
            Debug.LogError($"SlotAllPack: No package config found for {packName}");
            gameObject.SetActive(false);
            return;
        }

        if (!data.isAvailable)
        {
            gameObject.SetActive(false);
            return;
        }
        InitData(data);
    }

    public override void InitData(ShopPackageDataConfig data)
    {
        base.InitData(data);
        txtName.SetTextFormat(MyCache.strDefault,  data.packageNameToUI);
        txtDes.SetTextFormat(MyCache.strDefault,  data.packageDes);

        if (data.purchaseType == PurchaseType.IAPPay)
        {
            iAPPackage = InGamePurchaseManager.Instance.GetIAPPackageByID(data.packageName.ToString());
            Debug.Log($"<color=red> {iAPPackage == null}");
            txtPrice.SetTextFormat(MyCache.strDefault, iAPPackage?.GetPrice());
        }
        else if (data.purchaseType == PurchaseType.ResourcePay)
        {
            var style = MyCache.GetTextResourceStyle(data.resourcePrice.ResourceType);
            txtPrice.textStyle = style;
            txtPrice.SetTextFormat(MyCache.textFormatFloat, data.resourcePrice.Amount.ToFloat());
        }
    }

    public override void OnChoose()
    {
        AnimChoose();
        base.OnChoose();
    }

    private void AnimChoose()
    {
        
    }
}