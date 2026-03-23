using System;
using Core.UI.Activities;
using Core.UI.Screens;
using Cysharp.Threading.Tasks;
using R3;
using Sirenix.OdinInspector;
using TW.UGUI.Core.Modals;
using TW.UGUI.MVPPattern;
using UnityEngine;
using UnityEngine.UI;

public class ModalRefill : Modal
{ 
    [field: SerializeField] public ModalRefillContext.UIPresenter UIPresenter { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        AddLifecycleEvent(UIPresenter, 1);
    }

    public override async UniTask Initialize(Memory<object> args)
    {
        await base.Initialize(args);
    }
    [Serializable]
    public class ModalRefillContext
    {
        public static class Events
        {
            public static Action SampleEvent { get; set; }
        }

        [HideLabel]
        [Serializable]
        public class UIModel : IAModel
        {
            [field: Title(nameof(UIModel))]
            [field: SerializeField]
            public SerializableReactiveProperty<int> SampleValue { get; private set; }

            public UniTask Initialize(Memory<object> args)
            {
                return UniTask.CompletedTask;
            }
        }

        [HideLabel]
        [Serializable]
        public class UIView : IAView
        {
            [field: Title(nameof(UIView))]
            [field: SerializeField]
            public CanvasGroup MainView { get; private set; }
            
            [field: SerializeField]
            public Button BtnClose { get; private set; }
            [field: SerializeField] public Button BtnClose2 { get; private set; }
            [field: SerializeField] public Button BtnRefillAds { get; private set; }
            [field: SerializeField] public Button BtnRefillFull { get; private set; }

            public UniTask Initialize(Memory<object> args)
            {
                return UniTask.CompletedTask;
            }
        }

        [HideLabel]
        [Serializable]
        public class UIPresenter : IAPresenter, IModalLifecycleEventSimple
        {
            [field: SerializeField] public UIModel Model { get; private set; } = new();
            [field: SerializeField] public UIView View { get; set; } = new();

            public async UniTask Initialize(Memory<object> args)
            {
                await Model.Initialize(args);
                await View.Initialize(args);
                
                View.BtnClose.onClick.AddListener(OnClickBtnClose);
                View.BtnClose2.onClick.AddListener(OnClickBtnClose);
                View.BtnContinue.onClick.AddListener(OnClickBtnClose);
                View.BtnHome.onClick.AddListener(() => _ = OnClickBtnHome());
                View.BtnReplay.onClick.AddListener(() => _ = OnClickBtnReplay());
            }

            private async UniTask OnClickBtnReplay()
            {
                Level.Instance.ResetLevel();
                await UIManager.Instance.OpenActivityAsync<ActivityLoadingInGamePlay>((Func<UniTask>)Level.Instance.LoadData, (Func<UniTask>)Level.Instance.AnimFirstSpawn);  
                await UIManager.Instance.CloseModalAsync();
            }

            private async UniTask OnClickBtnHome()
            {
                await UIManager.Instance.OpenActivityAsync<ActivityLoadingInGamePlay>(null, null);
                Level.Instance.ResetLevel();
                await UIManager.Instance.CloseScreenAsync();
                await UIManager.Instance.OpenScreenDefaultAsync<ScreenDefault>();
                await UIManager.Instance.CloseModalAsync();
            }

            private void OnClickBtnClose()
            {
                _ = UIAnimManager.Instance.AnimButton(View.BtnClose.transform);
                _ = UIManager.Instance.CloseModalAsync();
            }
        }
    }
}


