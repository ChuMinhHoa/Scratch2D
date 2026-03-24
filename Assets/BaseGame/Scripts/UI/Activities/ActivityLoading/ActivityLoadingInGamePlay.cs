using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using LitMotion;
using TW.UGUI.MVPPattern;
using UnityEngine;
using R3;
using Sirenix.OdinInspector;
using TW.UGUI.Core.Activities;
using Random = UnityEngine.Random;

namespace Core.UI.Activities
{
    public class ActivityLoadingInGamePlay : Activity
    {
        [field: SerializeField] public ActivityLoadingInGamePlayContext.UIPresenter UIPresenter { get; private set; }

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
    public class ActivityLoadingInGamePlayContext
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
            private float currentProgress;

            [field: Title(nameof(UIView))]
            [field: SerializeField]
            public CanvasGroup MainView { get; private set; }

            [field: SerializeField] public ProgressBar LoadingProgressBar { get; private set; }

            public Func<UniTask> ActionCallBack;
            public Func<UniTask> ActionCallBack2;

            public bool loadToHome;

            public UniTask Initialize(Memory<object> args)
            {
                return UniTask.CompletedTask;
            }

            public async UniTask Loading()
            {
                currentProgress = 0f;
                var targetProgress = Random.Range(50f, 70f);
                await LMotion.Create(currentProgress, targetProgress, 0.5f)
                    .WithEase(Ease.Linear)
                    .Bind(ShowTextProgress).AddTo(MainView);

                if (ActionCallBack != null)
                    await ActionCallBack();
                
                await LMotion.Create(targetProgress, 100f, 0.5f)
                    .WithEase(Ease.Linear)
                    .Bind(ShowTextProgress).AddTo(MainView);
                await UIManager.Instance.CloseActivityAsync<ActivityLoadingInGamePlay>();
                
                if (ActionCallBack2 != null)
                    await ActionCallBack2();
            }

            private void ShowTextProgress(float value)
            {
                currentProgress = value;
                LoadingProgressBar.OnlyChangeProgress(value / 100f);
                _ = LoadingProgressBar.ChangeTextProgress(value);
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
                View.ActionCallBack = (Func<UniTask>)args.Span[0];
                View.ActionCallBack2 = (Func<UniTask>)args.Span[1];
                //View.loadToHome = (bool)args.Span[0];
            }

            public void DidEnter(Memory<object> args)
            {
                _ = View.Loading();
            }
        }
    }
}