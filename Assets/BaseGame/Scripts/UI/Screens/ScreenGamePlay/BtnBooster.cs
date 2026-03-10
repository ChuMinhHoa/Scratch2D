using System;
using Cysharp.Text;
using R3;
using TMPro;
using TW.Utility.CustomType;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class BtnBooster : MonoBehaviour
{
    [SerializeReference] public IBooster booster;
    
    public GameResource gameResource;
    
    [SerializeField] private Image imgIcon;
    [SerializeField] private GameObject objPrice;
    [SerializeField] private GameObject objAmount;
    [SerializeField] private TextMeshProUGUI txtAmount;
    [SerializeField] private TextMeshProUGUI txtPrice;
    [SerializeField] private BigNumber price = new(0);
    
    [SerializeField] private GameResource coinResource;
    
    private BoosterConfig config;
    
    private void Awake()
    {
        booster.InitData(UseBooster);
        booster.SetUsedCallBack(UsedBooster);
    }

    private void Start()
    {
       LoadData();
    }

    private void LoadData()
    {
        var gameResourceType = MyCache.ConvertBoosterToResourceType(((BoosterBase)booster).boosterType);
        gameResource = PlayerResourceManager.Instance.GetGameResource(gameResourceType);
        gameResource.ReactiveAmount.Subscribe(ChangeValueBooster).AddTo(this);
        config = BoosterGlobalConfig.Instance.GetBoosterConfig(((BoosterBase)booster).boosterType);
        price = config.price;
        txtPrice.SetTextFormat(MyCache.strDefault, price);
        imgIcon.sprite = config.icon;
    }


    private void ChangeValueBooster(BigNumber valueChange)
    {
        var isEnough = valueChange > 0;
        objPrice.SetActive(!isEnough);
        objAmount.SetActive(isEnough);
        if (isEnough)
        {
            txtAmount.SetTextFormat(MyCache.strDefault, valueChange);
            return;
        }
    }

    private void UseBooster()
    {
        UIAnimManager.Instance.AnimButton(transform);
        
        if (gameResource.Amount > 0)
        {
            booster.UseBooster();
            return;
        }

        if (!PlayerResourceManager.Instance.EnoughResource(GameResource.Type.Money, price)) return;
        
        booster.UseBooster();
    }

    private void UsedBooster()
    {
        if (gameResource.Amount > 0)
        {
            PlayerResourceManager.Instance.ChangeResource(gameResource.ResourceType, -1);;
            return;
        }

        if (!PlayerResourceManager.Instance.EnoughResource(GameResource.Type.Money, price)) return;
        PlayerResourceManager.Instance.ChangeResource(GameResource.Type.Money, -price);
    }
}

public enum BoosterType
{
    Magnet,
    AddSlot,
}
