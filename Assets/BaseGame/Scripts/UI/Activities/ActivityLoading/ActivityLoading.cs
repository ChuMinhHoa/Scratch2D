using System;
using Cysharp.Threading.Tasks;
using LitMotion;
using TW.UGUI.MVPPattern;
using UnityEngine;
using R3;
using Sirenix.OdinInspector;
using TW.UGUI.Core.Activities;
using TW.UGUI.Core.Screens;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;
using Screen = TW.UGUI.Core.Screens.Screen;

namespace Core.UI.Screens
{
    public class ActivityLoading : Activity
    {
        [field: SerializeField] public ActivityLoadingContext.UIPresenter UIPresenter { get; private set; }

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
    public class ActivityLoadingContext
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
            [field: SerializeField] public ProgressBar LoadingProgressBar { get; private set; }
            public UniTask Initialize(Memory<object> args)
            {
                return UniTask.CompletedTask;
            }

            private float currentProgress;
            public async UniTask AnimLoadBar()
            {
                currentProgress = 0f;
                await LMotion.Create(currentProgress, 50f, 0.5f)
                    .WithEase(Ease.Linear)
                    .Bind(ShowTextProgress).AddTo(MainView);
                currentProgress = 50f;
                await LMotion.Create(currentProgress, 75f, 0.5f)
                    .WithEase(Ease.Linear)
                    .Bind(ShowTextProgress).AddTo(MainView);
                currentProgress = 75f;
                await LoadSceneGamePlayAsync();
                await LMotion.Create(currentProgress, 100f, 0.5f)
                    .WithEase(Ease.Linear)
                    .Bind(ShowTextProgress).AddTo(MainView);
                await UIManager.Instance.OpenScreenAsync<ScreenHome>(stackChange: true);
                await UIManager.Instance.CloseActivityAsync<ActivityLoading>();
            }

            private void CloseActivity()
            {
               
            }

            private void ShowTextProgress(float value)
            {
                LoadingProgressBar.OnlyChangeProgress(value/100f);
                _ = LoadingProgressBar.ChangeTextProgress(value);
            }
            
            public async UniTask LoadSceneGamePlayAsync()
            {
                var loadingOperation = SceneManager.LoadSceneAsync("GamePlayScenes");
                while (loadingOperation is { isDone: false })
                {
                    // Optionally, you can track the progress here
                    //Debug.Log($"Loading progress: {loadingOperation.progress * 100}%");
                    await UniTask.WaitForEndOfFrame();
                }
                //Debug.Log("Scene loaded successfully!");
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
               
            }

            public void DidEnter(Memory<object> args)
            {
                _ = View.AnimLoadBar();
            }
        }
    }
}