using System;
using Core.UI.Screens;
using Cysharp.Text;
using Cysharp.Threading.Tasks;
using LitMotion;
using TW.UGUI.MVPPattern;
using UnityEngine;
using R3;
using Sirenix.OdinInspector;
using TMPro;
using TW.UGUI.Core.Activities;
using UnityEngine.UI;

namespace Core.UI.Activities
{
    public class ActivityLoseGame : Activity
    {
        [field: SerializeField] public ActivityLoseGameContext.UIPresenter UIPresenter { get; private set; }

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
    public class ActivityLoseGameContext
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

            [field: SerializeField] public Button btnClose;
            [field: SerializeField] public TextMeshProUGUI txtLevel;

            public UniTask Initialize(Memory<object> args)
            {
                var currentLevel = Level.Instance.levelIndex;
                txtLevel.SetTextFormat(MyCache.strLevel, currentLevel);
                return UniTask.CompletedTask;
            }
        }

        [HideLabel]
        [Serializable]
        public class UIPresenter : IAPresenter, IActivityLifecycleEventSimple
        {
            [field: SerializeField] public UIModel Model { get; private set; } = new();
            [field: SerializeField] public UIView View { get; set; } = new();

            public async UniTask Initialize(Memory<object> args)
            {
                await Model.Initialize(args);
                await View.Initialize(args);
                View.btnClose.onClick.AddListener(CloseActivity);
            }

            private void CloseActivity()
            {
                Level.Instance.ResetLevel();
                CloseActivityAsync().Forget();
            }

            private async UniTask CloseActivityAsync()
            {
                await UIManager.Instance.CloseScreenAsync();
                await UIManager.Instance.OpenScreenDefaultAsync<ScreenDefault>();
                await UIManager.Instance.CloseActivityAsync<ActivityLoseGame>();
            }
        }
    }
}