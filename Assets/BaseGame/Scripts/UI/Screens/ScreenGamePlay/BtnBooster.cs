using System;
using Cysharp.Text;
using R3;
using SDK;
using Sirenix.OdinInspector;
using TMPro;
using TW.Utility.CustomType;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class BtnBooster : MonoBehaviour
{
    //public int levelUnlock;
    public Reactive<int> levelIndex = new(0);
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
    [SerializeField] private GameObject handTutorial;
    [SerializeField] private GameObject objContent;

    private BoosterConfig config;

    public int countUsed = 0;

    private void Awake()
    {
        //booster.InitData(UseBooster);


        btnUseByAds.onClick.AddListener(UseByAds);
        btnUseByPrice.onClick.AddListener(UseByPrice);
        btnUseByGameResource.onClick.AddListener(UseByGameResource);

        booster.SetUsedCallBack(UsedBooster);


        GlobalEventManager.OnUnlockBooster += UnLockBooster;
    }

    private void OnDestroy()
    {
        GlobalEventManager.OnUnlockBooster -= UnLockBooster;
    }

    private void ChangeLevel(int levelChange)
    {
        var e = TutorialManager.Instance.IsUnLockBooster(config.boosterType);
        objContent.SetActive(e);
    }

    private void UnLockBooster(BoosterType type)
    {
        if (type != config.boosterType) return;
        var e = TutorialManager.Instance.IsUnLockBooster(config.boosterType);
        if (e) return;
        var eResourceType = MyCache.ConvertBoosterToResourceType(config.boosterType);
        PlayerResourceManager.Instance.ChangeResource(eResourceType, 1);
        objContent.SetActive(true);
        ShowHandTutorial();
    }

    private void ShowHandTutorial()
    {
        handTutorial.SetActive(true);
    }

    private void UseByGameResource()
    {
        UIAnimManager.Instance.AnimButton(imgIcon.transform);
        if (!booster.CheckCanUseBooster())
        {
            booster.ShowWarning();
            return;
        }

        if (gameResource.Amount > 0)
        {
            UseBooster();
        }
    }

    private void UseByPrice()
    {
        UIAnimManager.Instance.AnimButton(imgIcon.transform);
        if (!booster.CheckCanUseBooster())
        {
            booster.ShowWarning();
            return;
        }

        if (!PlayerResourceManager.Instance.IsEnoughResource(GameResource.Type.Money, price))
        {
            GlobalEventManager.OnShowWarning?.Invoke(MyCache.warningPrice);
            return;
        }

        UseBooster();
    }

    private void UseByAds()
    {
        UIAnimManager.Instance.AnimButton(imgIcon.transform);
        // if (!booster.CheckCanUseBooster())
        // {
        //     booster.ShowWarning();
        //     return;
        // }
#if UNITY_EDITOR || Cheat_Android
        //UseBooster();
        AddBooster();
#endif

#if !UNITY_EDITOR
        if (ShopManager.Instance.NoAds.Value) AddBooster();
        else
            AdsManager.Instance.ShowRewardVideo("BoosterReward", AddBooster);
#endif
    }

    private void AddBooster()
    {
        PlayerResourceManager.Instance.ChangeResource(gameResource.ResourceType, 1);
        countUsed++;
        PayByAds();
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

        levelIndex = Level.Instance.levelIndex;
        levelIndex.Subscribe(ChangeLevel).AddTo(this);
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
        }
        else if (usedByAds)
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
        if (handTutorial.activeSelf)
            handTutorial.SetActive(false);
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