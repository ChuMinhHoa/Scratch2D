using System;
using Cysharp.Threading.Tasks;
using TW.UGUI.MVPPattern;
using UnityEngine;
using R3;
using Sirenix.OdinInspector;
using TMPro;
using TW.UGUI.Core.Modals;
using UnityEngine.UI;

namespace Core.UI.Modals
{
    public class ModalTutorial : Modal
    {
        [field: SerializeField] public ModalTutorialContext.UIPresenter UIPresenter { get; private set; }

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
    public class ModalTutorialContext
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
            
            public TutorialConfig currentTutorialConfig;
            
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
            
            [field: SerializeField] public MainContentBase<SlotSubObstacle, Sprite> MainContentSubObstacle { get; set; }
            [field: SerializeField] public Button BtnClose { get; set; }
            [field: SerializeField] public Image ImgIcon { get; set; }
            [field: SerializeField] public TextMeshProUGUI TxtName { get; set; }
            [field: SerializeField] public TextMeshProUGUI TxtDes { get; set; }
            
            public UniTask Initialize(Memory<object> args)
            {
                return UniTask.CompletedTask;
            }

            public void InitTutorial(TutorialConfig tutorialConfig)
            {
                ImgIcon.sprite = tutorialConfig.spriteIcon;
                TxtName.text = tutorialConfig.tutorialName;
                TxtDes.text = tutorialConfig.tutorialDes;
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

                Model.currentTutorialConfig = (TutorialConfig)args.Span[0];
                View.MainContentSubObstacle.InitData(Model.currentTutorialConfig.sprSubIcon.AsSpan());
                View.BtnClose.onClick.AddListener(() => _ = CloseModal());
                View.InitTutorial(Model.currentTutorialConfig);
            }

            private async UniTask CloseModal()
            {
                await UIManager.Instance.CloseModalAsync();
                TutorialManager.Instance.DoneTutorial();
            }
        }
    }
}