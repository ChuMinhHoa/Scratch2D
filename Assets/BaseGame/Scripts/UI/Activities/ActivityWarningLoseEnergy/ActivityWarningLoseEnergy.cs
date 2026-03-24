using System;
using Cysharp.Threading.Tasks;
using TW.UGUI.MVPPattern;
using UnityEngine;
using R3;
using Sirenix.OdinInspector;
using TW.UGUI.Core.Activities;
using UnityEngine.UI;

namespace Core.UI.Activities
{
    public class ActivityWarningLoseEnergy : Activity
    {
        [field: SerializeField] public ActivityWarningLoseEnergyContext.UIPresenter UIPresenter { get; private set; }

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
    public class ActivityWarningLoseEnergyContext
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
            
            [field: SerializeField] public Button BtnClose { get; private set; }
            [field: SerializeField] public Button BtnClose2 { get; private set; }
            [field: SerializeField] public Button BtnConfirm { get; private set; }

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

            private Func<UniTask> actionCallBack;
            public async UniTask Initialize(Memory<object> args)
            {
                await Model.Initialize(args);
                await View.Initialize(args);
                actionCallBack = (Func<UniTask>)args.Span[0];
                
                View.BtnClose.onClick.AddListener(CloseActivity);
                View.BtnClose2.onClick.AddListener(CloseActivity);
                View.BtnConfirm.onClick.AddListener(OnClickConfirm);
            }

            private void OnClickConfirm()
            {
                actionCallBack?.Invoke();
                _ = UIManager.Instance.CloseActivityAsync<ActivityWarningLoseEnergy>();
            }

            private void CloseActivity()
            {
                UIAnimManager.Instance.AnimButton(View.BtnClose.transform);
                _ = UIManager.Instance.CloseActivityAsync<ActivityWarningLoseEnergy>();
            }
        }
    }
}