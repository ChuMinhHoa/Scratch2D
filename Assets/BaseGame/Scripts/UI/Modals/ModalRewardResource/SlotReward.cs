using Cysharp.Text;
using TMPro;
using UnityEngine;

public class SlotReward : SlotBase<GameResource>
{
    [SerializeField] private TextMeshProUGUI txtAmount;

    public override void InitData(GameResource data)
    {
        base.InitData(data);
        imgIcon.sprite = SpriteGlobalConfig.Instance.GetIconReward(data.ResourceType);
        
        txtAmount.SetTextFormat(MyCache.strDefault, data.Amount.ToStringUI());
    }
}
