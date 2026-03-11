using System;
using Core.UI.Activities;
using Cysharp.Threading.Tasks;
using TW.UGUI.MVPPattern;
using UnityEngine;
using R3;
using Sirenix.OdinInspector;
using TW.UGUI.Core.Screens;
using UnityEngine.UI;
using Screen = TW.UGUI.Core.Screens.Screen;

namespace Core.UI.Screens
{
    public class ScreenHome : Screen
    {
        [field: SerializeField] public ScreenHomeContext.UIPresenter UIPresenter { get; private set; }

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
    public class ScreenHomeContext
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
            [field: SerializeField] public Button BtnPlay { get; private set; }

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
                View.BtnPlay.onClick.AddListener(() => _ = PlayGame());
            }

            private async UniTask PlayGame()
            {
                //await GamePlayManager.Instance.PlayGame();
                await UIManager.Instance.OpenActivityAsync<ActivityLoadingInGamePlay>();
                await UIManager.Instance.OpenScreenAsync<ScreenGamePlay>(stackChange: true);
            }
        }
    }
}