using System.Collections.Generic;
using Core.UI.Modals;
using TW.Utility.DesignPattern;

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
        PlayerResourceManager.Instance.ChangeResource(reward.ResourceType, reward.Amount);
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
