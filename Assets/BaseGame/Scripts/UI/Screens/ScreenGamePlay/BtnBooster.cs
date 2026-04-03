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
    [SerializeField] private TextMeshProUGUI txtLevelUnlock;
    [SerializeField] private BigNumber price = new(0);
    [SerializeField] private Button btnUseByPrice;
    [SerializeField] private Button btnUseByAds;
    [SerializeField] private Button btnUseByGameResource;
    [SerializeField] private Button btnDeActive;

    [SerializeField] private GameResource coinResource;
    [SerializeField] private GameObject handTutorial;
    [SerializeField] private GameObject objContent;
    [SerializeField] private GameObject objLock;

    private BoosterConfig config;

    public int countUsed = 0;

    public bool showTutorialHand;

    private void Awake()
    {
        //booster.InitData(UseBooster);
        btnUseByAds.onClick.AddListener(UseByAds);
        btnUseByPrice.onClick.AddListener(UseByPrice);
        btnUseByGameResource.onClick.AddListener(UseByGameResource);
        btnDeActive.onClick.AddListener(OnDeActive);

        booster.SetUsedCallBack(UsedBooster);


        GlobalEventManager.OnUnlockBooster += UnLockBooster;

        AddGlobalEvent();
    }

    private void AddGlobalEvent()
    {
        switch (((BoosterBase)booster).boosterType)
        {
            case BoosterType.BoosterHammer:
                GlobalEventManager.OnHaveCardDone += OnCallCheckBooster;
                break;
            case BoosterType.BoosterAddSlot:
            case BoosterType.BoosterMagnet:
            case BoosterType.BoosterCart:
            default:
                return;
        }
    }

    private void OnCallCheckBooster()
    {
        Debug.Log("check active booster!");
        ChangeValueBooster(gameResource.Amount);
    }

    private void OnDeActive()
    {
        if (!booster.CheckCanUseBooster())
        {
            booster.ShowWarning();
        }
    }

    private void UseBoosterAnimSound()
    {
        UIAnimManager.Instance.AnimButton(imgIcon.transform);
        SoundManager.Instance.PlaySoundSfx(AudioKey.Sfx_ButtonClick);
    }

    private void OnDestroy()
    {
        GlobalEventManager.OnUnlockBooster -= UnLockBooster;
        GlobalEventManager.OnHaveCardDone -= OnCallCheckBooster;
    }

    private void ChangeLevel(int levelChange)
    {
        var e = TutorialManager.Instance.IsUnLockBooster(config.boosterType);
        objContent.SetActive(e);
        objLock.SetActive(!e);
    }

    private void UnLockBooster(BoosterType type)
    {
        if (type != config.boosterType) return;
        var e = TutorialManager.Instance.IsUnLockBooster(config.boosterType);
        if (e) return;
        var eResourceType = MyCache.ConvertBoosterToResourceType(config.boosterType);
        PlayerResourceManager.Instance.ChangeResource(eResourceType, 1);
        objContent.SetActive(true);
        if (showTutorialHand)
        {
            ShowHandTutorial();
        }
        objLock.SetActive(false);
    }

    private void ShowHandTutorial()
    {
        handTutorial.SetActive(true);
    }

    private void UseByGameResource()
    {
        UseBoosterAnimSound();


        if (gameResource.Amount > 0)
        {
            UseBooster();
        }
    }

    private void UseByPrice()
    {
        UseBoosterAnimSound();

        if (!PlayerResourceManager.Instance.IsEnoughResource(GameResource.Type.Money, price))
        {
            GlobalEventManager.OnShowWarning?.Invoke(MyCache.warningPrice);
            return;
        }

        UseBooster();
    }

    private void UseByAds()
    {
        // if (!booster.CheckCanUseBooster())
        // {
        //     booster.ShowWarning();
        //     return;
        // }
#if UNITY_EDITOR
        //UseBooster();
        AddBooster();
#endif

#if !UNITY_EDITOR
        if (ShopManager.Instance.NoAds.Value) AddBooster();
        else
        {
            IngameFirebaseAnalystic.Instance.SetAdsRewardInfo(config.boosterType.ToString(), 1);
            AdsManager.Instance.ShowRewardVideo(PlacementType.InGame.ToString(), config.boosterType.ToString(), AddBooster);
        }
#endif
    }

    private void AddBooster()
    {
        PlayerResourceManager.Instance.ChangeResource(gameResource.ResourceType, 1);
        // IngameFirebaseAnalystic.Instance.SetClaimCurrencyType(ClaimCurrencyType.AdsReward);
        // IngameFirebaseAnalystic.Instance.SetCurrencyPlacement(PlacementType.InGame);
        // IngameFirebaseAnalystic.Instance.TrackCurrencyEarn(gameResource.ResourceType, 1);
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
        var e = booster.CheckCanUseBooster();
        btnDeActive.gameObject.SetActive(!e);
        if (!e)
        {
            objAmount.SetActive(false);
            objPrice.SetActive(false);
            objWatchAds.SetActive(false);
            return;
        }

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

    public void UseBooster()
    {
        if (handTutorial.activeSelf)
            handTutorial.SetActive(false);
        if (GamePlayManager.Instance.gameState != GameState.Playing)
            return;
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

        OnCallCheckBooster();
        Debug.Log("used booster");
        GlobalEventManager.OnBoosterDone?.Invoke();
    }

    private void PayByAds()
    {
        objWatchAds.SetActive(false);
    }

    private void PayByGameResource() => PlayerResourceManager.Instance.ChangeResource(gameResource.ResourceType, -1);

    private void PayByPrice()
    {
        PlayerResourceManager.Instance.ChangeResource(GameResource.Type.Money, -price);
        var spendType = config.boosterType switch
        {
            BoosterType.BoosterMagnet => SpendType.UseBoosterMagnet,
            BoosterType.BoosterAddSlot => SpendType.UseBoosterAddSlot,
            BoosterType.BoosterHammer => SpendType.UseBoosterHammer,
            _ => SpendType.UseBoosterAddSlot
        };
        IngameFirebaseAnalystic.Instance.SetSpendType(spendType);
        IngameFirebaseAnalystic.Instance.SetCurrencyPlacement(PlacementType.InGame);
        IngameFirebaseAnalystic.Instance.TrackCurrencySpend(gameResource.ResourceType, price.ToInt());
    }

    public void ResetBooster()
    {
        SetBoosterCanUseType(BoosterUseType.Ads);
        countUsed = 0;
        objPrice.SetActive(false);
        objAmount.SetActive(false);
        objWatchAds.SetActive(false);
    }

    public bool IsSameBooster(BoosterType boosterType)
    {
        return ((BoosterBase)booster).boosterType == boosterType;
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
    BoosterMagnet,
    BoosterAddSlot,
    BoosterHammer,
    BoosterCart
}