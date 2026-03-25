using Cysharp.Text;
using LitMotion;
using TMPro;
using UnityEngine;

public class SlotReward : SlotBase<GameResource>
{
    [SerializeField] private TextMeshProUGUI txtAmount;

    public override void InitData(GameResource data)
    {
        base.InitData(data);
        imgIcon.sprite = SpriteGlobalConfig.Instance.GetShopRewardIcon(data.ResourceType);
        
        txtAmount.SetTextFormat(MyCache.GetFormat(data.ResourceType), data.Amount.ToStringUIFloor());
    }

    public void SetUpAnimShow()
    {
        trsContent.localScale = Vector3.zero;
    }

    public void AnimShow()
    {
        LMotion.Create(0f, 1f, 0.15f).WithEase(Ease.OutBack).Bind(x => trsContent.localScale = Vector3.one * x).AddTo(this);
    }
}
