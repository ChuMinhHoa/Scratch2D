using System;
using Cysharp.Text;
using LitMotion;
using TMPro;
using TW.Utility.CustomType;
using UnityEngine;
public enum ReviveType
{
    AddNote,
    AddSlot,
    BoosterMagnet
}
public class SlotRevive : SlotBase<ReviveType>
{
    public GameObject[] objRevive;
    public GameObject objCoin;
    public GameObject objAds;
    public TextMeshProUGUI txtPrice;

    public bool useByAds;
    public BigNumber price;

    public override void InitData(ReviveType data)
    {
        base.InitData(data);
        for (var i = 0; i < objRevive.Length; i++)
        {
            objRevive[i].SetActive(false);
        }
        objRevive[(int)slotData].SetActive(true);

        var e = IngameFirebaseAnalystic.Instance.useRevive == 0;

        useByAds = e;
        
        objCoin.SetActive(!e);
        objAds.SetActive(e);

        if (!e)
        {
            price = DefaultGlobalConfig.Instance.priceRevive;
            txtPrice.SetTextFormat(MyCache.strDefault, price);
        }
      
        var isShowRevive = IsShowRevive();
        gameObject.SetActive(isShowRevive);
    }

    private bool IsShowRevive()
    {
        return slotData switch
        {
            ReviveType.AddNote => Level.Instance.oSController.IsCanAddNote(),
            ReviveType.AddSlot => Level.Instance.fSpaceController.IsCanUseBoosterAddSlot(),
            ReviveType.BoosterMagnet => true,
            _ => false
        };
    }

    public AnimationCurve animChoose;

    public override void OnChoose()
    {
        LMotion.Create(0f, 1f, 0.15f).Bind(x => transform.localScale = Vector3.one * animChoose.Evaluate(x)).AddTo(this);
        base.OnChoose();
    }
}
