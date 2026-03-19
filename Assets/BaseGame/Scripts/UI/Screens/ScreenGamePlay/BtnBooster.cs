using System;
using Cysharp.Text;
using R3;
using Sirenix.OdinInspector;
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
    [SerializeField] private GameObject objWatchAds;
    [SerializeField] private TextMeshProUGUI txtAmount;
    [SerializeField] private TextMeshProUGUI txtPrice;
    [SerializeField] private BigNumber price = new(0);
    [SerializeField] private Button btnUseByPrice;
    [SerializeField] private Button btnUseByAds;
    [SerializeField] private Button btnUseByGameResource;
    
    [SerializeField] private GameResource coinResource;
    
    private BoosterConfig config;

    public int countUsed = 0;
    
    private void Awake()
    {
        //booster.InitData(UseBooster);
        
        btnUseByAds.onClick.AddListener(UseByAds);
        btnUseByPrice.onClick.AddListener(UseByPrice);
        btnUseByGameResource.onClick.AddListener(UseByGameResource);
        
        booster.SetUsedCallBack(UsedBooster);
    }

    private void UseByGameResource()
    {
        if (gameResource.Amount > 0)
        {
            booster.UseBooster();
        }
    }

    private void UseByPrice()
    {
        if (!PlayerResourceManager.Instance.EnoughResource(GameResource.Type.Money, price)) return;
        booster.UseBooster();
    }

    private void UseByAds()
    {
#if UNITY_EDITOR
        UseBooster();
#endif
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

    private void SetBoosterCanUseType(BoosterUseType useType)
    {
        booster.SetUsingType(useType);
    }

    [Button]
    private void ChangeValueBooster(BigNumber valueChange)
    {
        var isEnough = valueChange > 0;
        var usedByAds = countUsed > 0;
        objPrice.SetActive(!isEnough && usedByAds);
        objAmount.SetActive(isEnough);
        objWatchAds.SetActive(!isEnough && !usedByAds);


        if (isEnough)
        {
            SetBoosterCanUseType(BoosterUseType.GameResource);
        }else if (usedByAds)
        {
            SetBoosterCanUseType(BoosterUseType.Price);
        }
        else
        {
            SetBoosterCanUseType(BoosterUseType.Ads);
        }

        txtAmount.SetTextFormat(MyCache.strDefault, valueChange);
    }

    private void UseBooster()
    {
        UIAnimManager.Instance.AnimButton(imgIcon.transform);
        booster.UseBooster();
    }
    
    private void UsedBooster()
    {
        var boosterUseType = ((BoosterBase)booster).useType;
        switch (boosterUseType)
        {
            case BoosterUseType.Price:
                PayByPrice();
                break;
            case BoosterUseType.GameResource:
                PayByGameResource();
                break;
            case BoosterUseType.Ads:
                countUsed++;
                PayByAds();
                break;
            case BoosterUseType.None:
            default:
                return;
        }
        ChangeValueBooster(gameResource.Amount);
        GlobalEventManager.OnBoosterDone?.Invoke();
    }

    private void PayByAds()
    {
        objWatchAds.SetActive(false);
    }

    private void PayByGameResource() => PlayerResourceManager.Instance.ChangeResource(gameResource.ResourceType, -1);

    private void PayByPrice() => PlayerResourceManager.Instance.ChangeResource(GameResource.Type.Money, -price);

    public void ResetBooster()
    {
        SetBoosterCanUseType(BoosterUseType.Ads);
        countUsed = 0;
        objPrice.SetActive(false);
        objAmount.SetActive(false);
        objWatchAds.SetActive(false);
    }
}

public enum BoosterUseType
{
    None,
    Price,
    GameResource,
    Ads
}

public enum BoosterType
{
    Magnet,
    AddSlot,
    Hammer
}
