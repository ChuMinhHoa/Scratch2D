using System;
using Core.UI.Screens;
using Cysharp.Threading.Tasks;
using TW.UGUI.MVPPattern;
using UnityEngine;
using R3;
using Sirenix.OdinInspector;
using TW.UGUI.Core.Modals;

namespace Core.UI.Modals
{
    public class ModalRevive : Modal
    {
        [field: SerializeField] public ModalReviveContext.UIPresenter UIPresenter { get; private set; }

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
    public class ModalReviveContext
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
            
            [field: SerializeField] public MainContentBase<SlotRevive, ReviveType> MainContentRevive { get; private set; }

            public UniTask Initialize(Memory<object> args)
            {
                return UniTask.CompletedTask;
            }

            public void InitData()
            {
                MainContentRevive.DeActiveSlotOut();
                var e = Enum.GetValues(typeof(ReviveType)) as ReviveType[];
                MainContentRevive.InitData(e.AsSpan());
            }

            public void SetActionCallBack(Action<SlotRevive> actionCallBack)
            {
                MainContentRevive.SetActionSlotCallBack(actionCallBack);
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

                View.SetActionCallBack(SlotReviveCallBack);
                View.InitData();
            }

            private void SlotReviveCallBack(SlotRevive slotRevive)
            {
                switch (slotRevive.slotData)
                {
                    case ReviveType.AddNote:
                        AddNote();
                        break;
                    case ReviveType.AddSlot:
                        AddSlot();
                        break;
                    case ReviveType.BoosterMagnet:
                        UseBoosterMagnet();
                        break;
                    default:
                        return;
                }
            }

            private void UseBoosterMagnet()
            {
                ScreenGamePlayContext.Events.UseBooster?.Invoke(BoosterType.BoosterMagnet);
                CloseModal();
            }

            private void AddSlot()
            {
                ScreenGamePlayContext.Events.UseBooster?.Invoke(BoosterType.BoosterAddSlot);
                CloseModal();
            } 

            private void AddNote()
            {
                Level.Instance.AddSlotNote();
                CloseModal();
            }

            private void CloseModal() => _ = UIManager.Instance.CloseModalAsync();
        }
    }
}