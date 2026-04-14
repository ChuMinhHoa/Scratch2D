using System;
using Firebase.Analytics;
//using GlobalEnum;
//using Manager;
using SDK;
using TW.Utility.DesignPattern;
using UnityEngine;

public class IngameFirebaseAnalystic : Singleton<IngameFirebaseAnalystic>
{
    #region Gameplay

    public void StartTimePlayLevel()
    {
        TimeManager.OnTimeChange += OnTimeChange;
    }

    private void OnDestroy()
    {
        TimeManager.OnTimeChange -= OnTimeChange;
    }

    public void PauseTimePlayedLevel()
    {
        TimeManager.OnTimeChange -= OnTimeChange;
    }

    private void OnTimeChange()
    {
        timePlayLevelDuration += Time.deltaTime;
    }

    private void ResetDurationLevel() => timePlayLevelDuration = 0;

    public int lastLevel = -1;

    public int retry;
    public int useRevive;

    public int winStreak;
    public int reviveUsed;

    public int noteFail;
    public int noteComplete;

    public double timePlayLevelDuration;

    public LoseType loseType;

    public void SetNoteFail(int value) => noteFail = value;

    public void AddNoteComplete() => noteComplete++;

    private void AddWinStreak() => winStreak++;
    private void ResetWinStreak() => winStreak = 0;

    public void AddReviveUsed() => reviveUsed++;

    private void AddRetry() => retry++;

    public void AddUseRevive() => useRevive++;

    public void SetLoseType(LoseType value) => loseType = value;

    public void SetLevel(int level)
    {
        ResetDurationLevel();
        if (level == lastLevel)
        {
            AddRetry();
            winStreak = 0;
            useRevive = 0;
        }
        else
        {
            lastLevel = level;
            retry = 0;
            useRevive = 0;
            noteFail = 0;
            noteComplete = 0;
            reviveUsed = 0;
        }
    }

    public void TrackLevelStart()
    {
        var level = PlayerInfoManager.Instance.playerLevel.Value + 1;
        var remainingCoins = PlayerResourceManager.Instance.GetGameResource(GameResource.Type.Money).Amount;

        var remainingBoosterAddSlot = PlayerResourceManager.Instance.GetGameResource(GameResource.Type.BoosterAddSlot)
            .Amount.ToInt();
        var remainingBoosterHammer = PlayerResourceManager.Instance.GetGameResource(GameResource.Type.BoosterHammer)
            .Amount.ToInt();
        var remainingBoosterMagnet = PlayerResourceManager.Instance.GetGameResource(GameResource.Type.BoosterMagnet)
            .Amount.ToInt();
        var remainingBooster = remainingBoosterMagnet + remainingBoosterHammer + remainingBoosterAddSlot;

        var parameters = new Parameter[]
        {
            new("level", level.ToString()),
            new("retry", retry),
            new("remaining_coins", remainingCoins.ToInt()),
            new("remaining_booster", remainingBooster),
        };
        FirebaseManager.Instance.LogFirebaseEvent("level_start", parameters);
    }

    public void TrackLevelComplete()
    {
        AddWinStreak();
        var level = PlayerInfoManager.Instance.playerLevel.Value;
        
        var remainingBoosterAddSlot = PlayerResourceManager.Instance.GetGameResource(GameResource.Type.BoosterAddSlot)
            .Amount.ToInt();
        var remainingBoosterHammer = PlayerResourceManager.Instance.GetGameResource(GameResource.Type.BoosterHammer)
            .Amount.ToInt();
        var remainingBoosterMagnet = PlayerResourceManager.Instance.GetGameResource(GameResource.Type.BoosterMagnet)
            .Amount.ToInt();
        var remainingBooster = remainingBoosterMagnet + remainingBoosterHammer + remainingBoosterAddSlot;

        var remainingCoins = PlayerResourceManager.Instance.GetGameResource(GameResource.Type.Money).Amount.ToInt();
#if UNITY_EDITOR
        Debug.Log($"Track level complete: level {level} " +
                  $"\n duration {timePlayLevelDuration} " +
                  $"\n retry {retry} " +
                  $"\n revive_used {reviveUsed} " +
                  $"\n win_streak {winStreak} " +
                  $"\n remaining_booster {remainingBooster}");
#endif
        var parameters = new Parameter[]
        {
            new("level", level.ToString()),
            new("revive_used", reviveUsed),
            new("retry", retry),
            new("duration_win", timePlayLevelDuration),
            new("winstreak", winStreak),
            new("remaining_booster", remainingBooster),
            new("remaining_coins", remainingCoins)
        };
        FirebaseManager.Instance.LogFirebaseEvent("level_complete", parameters);
    }

    public void TrackLevelFail()
    {
        ResetWinStreak();
        var level = PlayerInfoManager.Instance.playerLevel.Value + 1;

        var remainingBoosterAddSlot = PlayerResourceManager.Instance.GetGameResource(GameResource.Type.BoosterAddSlot)
            .Amount.ToInt();
        var remainingBoosterHammer = PlayerResourceManager.Instance.GetGameResource(GameResource.Type.BoosterHammer)
            .Amount.ToInt();
        var remainingBoosterMagnet = PlayerResourceManager.Instance.GetGameResource(GameResource.Type.BoosterMagnet)
            .Amount.ToInt();
        var remainingBooster = remainingBoosterMagnet + remainingBoosterHammer + remainingBoosterAddSlot;

        var remainingCoins = PlayerResourceManager.Instance.GetGameResource(GameResource.Type.Money).Amount.ToInt();

#if UNITY_EDITOR
        Debug.Log($"Track level fail: level {level} " +
                  $"\n duration {timePlayLevelDuration} " +
                  $"\n retry {retry} " +
                  $"\n revive_used {reviveUsed} " +
                  $"\n win_streak {winStreak} " +
                  $"\n remaining_booster {remainingBooster}" +
                  $"\n note_fail {noteFail}" +
                  $"\n note_complete {noteComplete}" +
                  $"\n lose_type {loseType}");
#endif
        
        var parameters = new Parameter[]
        {
            new("level", level.ToString()),
            new("remaining_coins", remainingCoins),
            new("remaining_booster", remainingBooster),
            new("page_fail", noteFail),
            new("page_complete", noteComplete),
            new("duration_fail", timePlayLevelDuration),
            new("revive_used", reviveUsed),
            new("retry", retry),
            new("winstreak", winStreak),
            new("lose_type", loseType.ToString())
        };
        FirebaseManager.Instance.LogFirebaseEvent("level_fail", parameters);
    }

    #endregion

    #region ADS

    public string currentAdsRewardType;
    public int adsRewardValue;
    public int levelAdsShow;

    public void SetAdsRewardInfo(string rewardType, int rewardValue, int level)
    {
        Debug.Log("set ads reward info " + rewardType + " value " + rewardValue);
        levelAdsShow = level;
        currentAdsRewardType = rewardType;
        adsRewardValue = rewardValue;
    }

    public void TrackAdsRewardShow(string placement, string buttonName)
    {
        var parameters = new Parameter[]
        {
            new("level", levelAdsShow.ToString()),
            new("button_name", buttonName),
            new("reward_name", currentAdsRewardType),
            new("value", adsRewardValue),
            new("placement", placement)
        };
        FirebaseManager.Instance.LogFirebaseEvent("ads_reward_complete", parameters);
    }

    public void TrackAdsInterShow(string placement)
    {
        var level = PlayerInfoManager.Instance.playerLevel.Value;
        var parameters = new Parameter[]
        {
            new("level", level.ToString()),
            new("placement", placement)
        };
        FirebaseManager.Instance.LogFirebaseEvent("ads_inter_show", parameters);
    }

    #endregion

    #region Currency

    public PlacementType currencyPlacement;
    public ClaimCurrencyType claimCurrencyType;
    public SpendType spendType;
    public void SetSpendType(SpendType type) => spendType = type;
    public void SetClaimCurrencyType(ClaimCurrencyType claimType) => claimCurrencyType = claimType;
    public void SetCurrencyPlacement(PlacementType placement) => currencyPlacement = placement;

    public void TrackCurrencyEarn(GameResource.Type currencyType, int amount, int level)
    {
        var parameters = new Parameter[]
        {
            new("level", level.ToString()),
            new("value", amount),
            new("currency_name", currencyType.ToString()),
            new("placement", currencyPlacement.ToString()),
            new("claim_type", claimCurrencyType.ToString())
        };
        FirebaseManager.Instance.LogFirebaseEvent("currency_earn", parameters);
    }

    public void TrackCurrencySpend(GameResource.Type currencyType, int amount)
    {
        var level = PlayerInfoManager.Instance.playerLevel.Value + 1;
        var parameters = new Parameter[]
        {
            new("level", level.ToString()),
            new("value", amount),
            new("currency_name", currencyType.ToString()),
            new("spend_type", spendType.ToString()),
            new("placement", currencyPlacement.ToString())
        };
        FirebaseManager.Instance.LogFirebaseEvent("currency_spend", parameters);
    }

    #endregion

    #region Purchase

    public PlacementType placementPurchase;
    public void SetPlacementPurchase(PlacementType placement) => placementPurchase = placement;

    public void TrackEventPurchase(string packageName, string productID)
    {
        var level = PlayerInfoManager.Instance.playerLevel.Value + 1;
        var isFirstOrder = ShopManager.Instance.IsFirstPurchase.Value;
        var parameters = new Parameter[]
        {
            new("level", level.ToString()),
            new("pack_id", productID),
            new("placement", placementPurchase.ToString()),
            new("iap_first_order", isFirstOrder.ToString()),
        };
        FirebaseManager.Instance.LogFirebaseEvent("iapPurchased_confirmed", parameters);
    }

    public void TrackEventPurchaseClick(string productID)
    {
        var level = PlayerInfoManager.Instance.playerLevel.Value + 1;
        var parameters = new Parameter[]
        {
            new("level", level.ToString()),
            new("placement", placementPurchase.ToString()),
            new("pack_id", productID),
        };
        FirebaseManager.Instance.LogFirebaseEvent("iap_click", parameters);
    }

    #endregion

    #region UserProperty

    public void SetLevelUserProperty()
    {
        var level = PlayerInfoManager.Instance.playerLevel.Value + 1;
        FirebaseAnalytics.SetUserProperty("Level", level.ToString());
    }

    public void SetUserRetention()
    {
        var totalDay = (int)TimeManager.Instance.GetDayRetention();
        FirebaseAnalytics.SetUserProperty("day_retention", totalDay.ToString());
    }

    public void SetUserPropertyIapCount()
    {
        var iapCount = ShopManager.Instance.iapCount.Value;
        FirebaseAnalytics.SetUserProperty("iap_count", iapCount.ToString());
    }

    public void SetUserPropertyAdsRewardCount()
    {
        var adsRewardCount = PlayerInfoManager.Instance.adsRewardCount;
        FirebaseAnalytics.SetUserProperty("ads_reward_count", adsRewardCount.ToString());
    }

    public void SetUserPropertyAdsInterCount()
    {
        var adsInterCount = PlayerInfoManager.Instance.adsInterCount;
        FirebaseAnalytics.SetUserProperty("ads_inter_count", adsInterCount.ToString());
    }

    #endregion

    #region Booster
    public PlacementType boosterPlacement;
    public void SetBoosterPlacement(PlacementType placement) => boosterPlacement = placement;
    
    public void TrackBoosterEarn(GameResource.Type rewardType, int value, int level)
    {
        var parameters = new Parameter[]
        {
            new("level", level.ToString()),
            new("source", boosterPlacement.ToString()),
            new("booster_name", rewardType.ToString()),
            new("value", value.ToString())
        };
        FirebaseManager.Instance.LogFirebaseEvent("booster_earn", parameters);
    }

    public void TrackBoosterSpend(GameResource.Type boosterType)
    {
        var level = PlayerInfoManager.Instance.playerLevel.Value + 1;
        var parameters = new Parameter[]
        {
            new("level", level.ToString()),
            new("source", boosterPlacement.ToString()),
            new("booster_name", boosterType.ToString()),
        };
        FirebaseManager.Instance.LogFirebaseEvent("booster_spend", parameters);
    }

    #endregion
   
}

public enum PlacementType
{
    InGame,
    Shop,
    ShopInGame,
    WinGame,
    Home
}

public enum ClaimCurrencyType
{
    AdsReward,
    IAP,
    WinGame
}

public enum SpendType
{
    UseBoosterAddSlot,
    UseBoosterMagnet,
    UseBoosterHammer,
    Revive
}

public enum LoseType
{
    OutSlot,
    BackHome,
    Replay
}