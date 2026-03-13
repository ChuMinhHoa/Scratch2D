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
    public class ActivityWinGame : Activity
    {
        [field: SerializeField] public ActivityWinGameContext.UIPresenter UIPresenter { get; private set; }

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
    public class ActivityWinGameContext
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
            
            [field: SerializeField] public CanvasGroup WrapInfo { get; set; }
            [field: SerializeField] public GameObject ObjNewFeature { get; set; }
            [field: SerializeField] public Button BtnClaim { get; set; }
            [field: SerializeField] public Button BtnClaimX2 { get; set; }
            [field: SerializeField] public TextMeshProUGUI TxtLevel { get; set; }
            [field: SerializeField] public TextMeshProUGUI TxtReward { get; set; }

            public UniTask Initialize(Memory<object> args)
            {
                ObjNewFeature.SetActive(false);
                var currentLevel = Level.Instance.levelIndex;
                TxtLevel.SetTextFormat(MyCache.strLevel, currentLevel);
                var reward = 10;
                TxtReward.SetTextFormat(MyCache.strDefault, reward);
                return UniTask.CompletedTask;
            }
            
            public async UniTask AnimWrapInfo()
            {
                await UniTask.WaitForSeconds(0.5f);
                LMotion.Create(0f, 1f, 0.5f).Bind(x => WrapInfo.alpha = x).AddTo(MainView);
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
                View.WrapInfo.alpha = 0f;
                View.BtnClaim.onClick.AddListener(ClaimReward);
                View.BtnClaimX2.onClick.AddListener(ClaimRewardX2);
            }

            private void ClaimRewardX2()
            {
                UIAnimManager.Instance.AnimButton(View.BtnClaimX2.transform);
                Claim(true);
            }

            private void ClaimReward()
            {
                UIAnimManager.Instance.AnimButton(View.BtnClaim.transform);
                Claim();
            }

            private void Claim(bool isX2 = false)
            {
                PlayerResourceManager.Instance.ChangeResource(GameResource.Type.Money, 10 * (isX2 ? 2 : 1));
                UIControl().Forget();
            }

            private async UniTask UIControl()
            {
                await UIManager.Instance.CloseScreenAsync();
                await UIManager.Instance.OpenScreenDefaultAsync<ScreenDefault>();
                await UIManager.Instance.CloseActivityAsync<ActivityWinGame>();
            }

            public void DidEnter(Memory<object> args)
            {
                _ = View.AnimWrapInfo();
            }
        }
    }
}