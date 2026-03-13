using System;
using Cysharp.Text;
using Cysharp.Threading.Tasks;
using LitMotion;
using TW.UGUI.MVPPattern;
using UnityEngine;
using R3;
using Sirenix.OdinInspector;
using TMPro;
using TW.UGUI.Core.Activities;

namespace Core.UI.Activities
{
    public class ActivityFirstShowOnGamePlay : Activity
    {
        [field: SerializeField] public ActivityFirstShowOnGamePlayContext.UIPresenter UIPresenter { get; private set; }

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
    public class ActivityFirstShowOnGamePlayContext
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
            [field: SerializeField] public Transform TrsContent { get; private set; }
            [field: SerializeField] public TextMeshProUGUI TxtTotalItems { get; private set; }
            [field: SerializeField] public TextMeshProUGUI TxtLevel { get; private set; }
            [field: SerializeField] public AnimationCurve CurveAnim { get; set; }

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
                View.TxtTotalItems.SetText("0");
                var level = Level.Instance.levelIndex;
                View.TxtLevel.SetTextFormat(MyCache.strLevel, level.Value + 1);
            }

            public void DidEnter(Memory<object> args)
            {
                AnimOpen().Forget();
            }

            private async UniTask AnimOpen()
            {
                var totalItems = Level.Instance.oSController.totalCount.Value;
                await LMotion.Create(0f, 1f, 0.25f).WithEase(View.CurveAnim).Bind(x => View.TrsContent.localScale = Vector3.one * x);
                await LMotion.Create(0, totalItems, 0.25f).Bind(x=> View.TxtTotalItems.text = $"{x}").AddTo(View.MainView);
                await UniTask.WaitForSeconds(1f);
                await LMotion.Create(1f, 0f, 0.15f).WithEase(View.CurveAnim).Bind(x => View.MainView.alpha = x);
                await UIManager.Instance.CloseActivityAsync<ActivityFirstShowOnGamePlay>();
            }
        }
    }
}