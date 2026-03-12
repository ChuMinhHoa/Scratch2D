using System;
using Cysharp.Text;
using Cysharp.Threading.Tasks;
using TW.UGUI.MVPPattern;
using UnityEngine;
using R3;
using Sirenix.OdinInspector;
using TMPro;
using TW.UGUI.Core.Screens;
using UniRx;
using UnityEngine.UI;
using Screen = TW.UGUI.Core.Screens.Screen;

namespace Core.UI.Screens
{
    public class ScreenGamePlay : Screen
    {
        [field: SerializeField] public ScreenGamePlayContext.UIPresenter UIPresenter { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            AddLifecycleEvent(UIPresenter, 1);
        }

        public override async UniTask Initialize(Memory<object> args)
        {
            await base.Initialize(args);
        }
    }


    [Serializable]
    public class ScreenGamePlayContext
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
            
            public Reactive<int> level = new(0);
            public UniTask Initialize(Memory<object> args)
            {
                level = Level.Instance.levelIndex;
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
            [field: SerializeField] public Button BtnSetting { get; private set; }
            [field: SerializeField] public TextMeshProUGUI TxtLevel { get; private set; }

            public UniTask Initialize(Memory<object> args)
            {
                return UniTask.CompletedTask;
            }
        }

        [HideLabel]
        [Serializable]
        public class UIPresenter : IAPresenter, IScreenLifecycleEventSimple
        {
            [field: SerializeField] public UIModel Model { get; private set; } = new();
            [field: SerializeField] public UIView View { get; set; } = new();

            public async UniTask Initialize(Memory<object> args)
            {
                await Model.Initialize(args);
                await View.Initialize(args);
                
                View.BtnSetting.onClick.AddListener(OnClickSetting);
                Model.level.Subscribe(ChangeLevel).AddTo(View.MainView);
            }

            public void ChangeLevel(int levelChange)
            {
                View.TxtLevel.SetTextFormat(MyCache.strLevel, levelChange);
            }

            private void OnClickSetting()
            {
                _ = UIAnimManager.Instance.AnimButton(View.BtnSetting.transform, null);
            }
        }
    }
}