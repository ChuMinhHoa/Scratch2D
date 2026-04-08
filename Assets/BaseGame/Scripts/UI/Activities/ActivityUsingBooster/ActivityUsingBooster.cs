using System;
using Core.UI.Screens;
using Cysharp.Threading.Tasks;
using TW.UGUI.MVPPattern;
using UnityEngine;
using R3;
using Sirenix.OdinInspector;
using TW.UGUI.Core.Activities;
using UnityEngine.UI;

namespace Core.UI.Activities
{
    public class ActivityUsingBooster : Activity
    {
        [field: SerializeField] public ActivityUsingBoosterContext.UIPresenter UIPresenter { get; private set; }

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
    public class ActivityUsingBoosterContext
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

            [field: SerializeField] public Button BtnCloseUsingBooster { get; private set; }

            public UniTask Initialize(Memory<object> args)
            {
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
                View.BtnCloseUsingBooster.onClick.AddListener(OnCloseActivity);
                GlobalEventManager.OnBoosterDone += CloseActivity;
            }

            public UniTask Cleanup(Memory<object> args)
            {
                GlobalEventManager.OnBoosterDone -= CloseActivity;
                ScreenGamePlayContext.Events.OnActiveInteractable?.Invoke(true);
                GlobalEventManager.OnBoosterDone?.Invoke();
                return UniTask.CompletedTask;
            }

            private void OnCloseActivity()
            {
                SoundManager.Instance.PlaySoundSfx(AudioKey.Sfx_ButtonClick);
                _ = UIAnimManager.Instance.AnimButton(View.BtnCloseUsingBooster.transform);
                CloseActivity();
            }

            private void CloseActivity()
            {
                TutorialManager.Instance.SetActiveHand(false);
                _ = UIManager.Instance.CloseActivityAsync<ActivityUsingBooster>();
            }
        }
    }
}