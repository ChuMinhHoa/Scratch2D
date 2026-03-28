using System.Collections.Generic;
using Core.UI.Modals;
using TW.Utility.DesignPattern;
using UnityEngine;

public class RewardManager : Singleton<RewardManager>
{
    public List<GameResource> resourceRewardList = new();
    public List<GameResource> boosterRewardList = new();

    public void AddBoosterReward(GameResource reward)
    {
        PlayerResourceManager.Instance.ChangeResource(reward.ResourceType, reward.Amount);
    }

    public void AddResourceReward(GameResource reward)
    {
        Debug.Log(reward.ResourceType);
        switch (reward.ResourceType)
        {
            case GameResource.Type.NoAds:
            {
                var e = ShopManager.Instance.SetNoAds();
                if (!e)
                {
                    PlayerResourceManager.Instance.ChangeResource(GameResource.Type.Money, 5000);
                    var newRewardAds = new GameResource(GameResource.Type.Money, 5000);
                    AddRewardToList(newRewardAds);
                    return;
                }
                break;
            }
            case GameResource.Type.Energy:
                Debug.Log("add energy!");
                EnergyManager.Instance.AddEnergyInfiniteTime(reward.Amount);
                break;
            
            default:
                PlayerResourceManager.Instance.ChangeResource(reward.ResourceType, reward.Amount);
                break;
        }

        AddRewardToList(reward);
    }

    private void AddRewardToList(GameResource reward)
    {
        for (var i = 0; i < resourceRewardList.Count; i++)
        {
            if (resourceRewardList[i].ResourceType == reward.ResourceType)
            {
                resourceRewardList[i].Amount += reward.Amount;
                return;
            }
        }
        var newReward = new GameResource(reward.ResourceType, reward.Amount);
        resourceRewardList.Add(newReward);
    }

    public void ShowReward()
    {
        if (resourceRewardList.Count > 0)
        {
            _ = UIManager.Instance.OpenModalAsync<ModalRewardResource>();
        }

        if (boosterRewardList.Count > 0)
        {
            //_ = UIManager.Instance.OpenModalAsync<ModalRewardBooster>();
        }
    }

    public void ClearResourceReward() => resourceRewardList.Clear();
    public void ClearBoosterReward() => boosterRewardList.Clear();
}
