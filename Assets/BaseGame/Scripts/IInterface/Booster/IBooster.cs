using System;
using Core.UI.Activities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Object = System.Object;

public interface IBooster
{
    void InitData();
    void UseBooster();
    void UsedBooster(SelectAbleOnBooster data);
    void ActiveBooster(bool active);
    void OnChangeBoosterCount(int count);
}

[Serializable]
public class BoosterBase : IBooster
{
    public BoosterType boosterType;
    public TextMeshProUGUI boosterText;
    public Button boosterButton;
    
    public virtual void InitData()
    {
        boosterButton.onClick.AddListener(UseBooster);
    }

    public virtual void UseBooster()
    {
        if (GamePlayManager.Instance.gameState != GameState.Playing)
            return;
        _ = UIManager.Instance.OpenActivityAsync<ActivityUsingBooster>();
        GlobalEventManager.OnBoosterUsing?.Invoke(boosterType, this);
    }

    public virtual void UsedBooster(SelectAbleOnBooster data)
    {
    }

    public virtual void ActiveBooster(bool active)
    {
        boosterButton.interactable = active;
    }

    public virtual void OnChangeBoosterCount(int count)
    {
        boosterText.text = count.ToString();
    }
}

