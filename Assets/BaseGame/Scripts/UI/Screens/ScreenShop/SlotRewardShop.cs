using System;
using Cysharp.Text;
using TMPro;
using UnityEngine;

public class SlotRewardShop : SlotBase<GameResource>
{
    [SerializeField] private TextMeshProUGUI txtAmount;
    
    public override void InitData(GameResource data)
    {
        base.InitData(data);
        var e = data.ResourceType == GameResource.Type.Money;
        var sprIcon =
            e
                ? SpriteGlobalConfig.Instance.GetShopRewardIcon(data.Amount)
                : SpriteGlobalConfig.Instance.GetShopRewardIcon(data.ResourceType);
        imgIcon.sprite = sprIcon;
        if (sprIcon != null)
            txtAmount.SetTextFormat(MyCache.GetFormat(data.ResourceType), data.Amount.ToStringUIFloor());
    }
}