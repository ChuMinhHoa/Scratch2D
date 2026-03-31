using System;
using Cysharp.Threading.Tasks;
using LitMotion;
using TMPro;
using TW.Utility.DesignPattern;
using UnityEngine;
using UnityEngine.UI;

public class UICheat : Singleton<UICheat>
{
    public Button btnCheat;
    public Button btnHideUI;
    public TMP_InputField inputField;
    public Button btnLoadLevel;
    public Button btnAddMoney;
    public GameObject objCheatPanel;
    public Transform trsCheat;
    private bool isCheat;
    public CanvasGroup CanvasGroup;
    public float posYOffset = 180;

    public void CallInit()
    {
        isCheat = PlayerResourceManager.Instance.gameBuildType == GameBuildType.Cheat;
        if (isCheat)
        {
            objCheatPanel.SetActive(true);
            btnHideUI.onClick.AddListener(OnHideUI);
            btnLoadLevel.onClick.AddListener(() => _ = OnLoadLevel());
            btnCheat.onClick.AddListener(ShowUICheat);
            btnAddMoney.onClick.AddListener(AddGold);
        }
        else
        {
            objCheatPanel.SetActive(false);
        }
    }

    private void AddGold()
    {
        PlayerResourceManager.Instance.ChangeResource(GameResource.Type.Money, 100000);
    }

    private bool showCheat;
    private void ShowUICheat()
    {
        showCheat = !showCheat;
        var pos = trsCheat.transform.localPosition;
        pos.y += posYOffset * (showCheat ? -1 : 1);
        LMotion.Create(trsCheat.transform.localPosition, pos, 0.15f).Bind(x => trsCheat.transform.localPosition = x)
            .AddTo(this);
    }

    private async UniTask OnLoadLevel()
    {
        if (GamePlayManager.Instance.gameState == GameState.Playing)
        {
            var strLevel = inputField.text;
            var level = int.Parse(strLevel);
            Level.Instance.levelIndex.Value = level;
            Level.Instance.LoadDataClean();
            await Level.Instance.LoadData();
            await Level.Instance.AnimFirstSpawn();
        }
    }

    public bool hideUI = false;

    private void OnHideUI()
    {
        hideUI = !hideUI;
        GlobalEventManager.HideUI?.Invoke();
        CanvasGroup.alpha = hideUI ? 0 : 1;
        //CanvasGroup.interactable = !hideUI;
    }
}