using System;
using Cysharp.Threading.Tasks;
using TW.UGUI.MVPPattern;
using UnityEngine;
using R3;
using Sirenix.OdinInspector;
using TW.UGUI.Core.Screens;
using Screen = TW.UGUI.Core.Screens.Screen;

namespace Core.UI.Screens
{
    public class ScreenDefault : Screen
    {
        [field: SerializeField] public ScreenDefaultContext.UIPresenter UIPresenter { get; private set; }

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
    public class ScreenDefaultContext
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

            [field: SerializeField] public SlotTabMenu[] SlotTabMenu { get; private set; }

            public UniTask Initialize(Memory<object> args)
            {
                return UniTask.CompletedTask;
            }

            public void InitSlotTabMenu(Action<SlotTabType> action)
            {
                for (var i = 0; i < SlotTabMenu.Length; i++)
                {
                    SlotTabMenu[i].SetActionCallBackOnChooseTab(action);
                }
            }

            public async UniTask OpenScreenHome()
            {
                await UIManager.Instance.OpenScreenAsync<ScreenHome>();
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

                View.InitSlotTabMenu(ActionSlotTabCallback);
                await View.OpenScreenHome();
            }

            public void DidPushEnter(Memory<object> args)
            {
                ActionSlotTabCallback(SlotTabType.Home);
            }

            private SlotTabType currentTabType = SlotTabType.None;

            [Button]
            private void ActionSlotTabCallback(SlotTabType type)
            {
                if (type == currentTabType) return;
                currentTabType = type;

                for (var i = 0; i < View.SlotTabMenu.Length; i++)
                {
                    if (View.SlotTabMenu[i].slotTabType == type) View.SlotTabMenu[i].OnSelect();
                    else
                    {
                        View.SlotTabMenu[i].OnDeSelectTab();
                    }
                }

                switch (type)
                {
                    case SlotTabType.Shop:
                        Debug.Log("Open shop");
                        _ = UIManager.Instance.OpenScreenAsync<ScreenShop>();
                        break;
                    case SlotTabType.Home:
                        Debug.Log("Open Home");
                        _ = UIManager.Instance.OpenScreenAsync<ScreenHome>();
                        break;
                    case SlotTabType.ComingSoon:
                        Debug.Log("Coming soon");
                        _ = UIManager.Instance.OpenScreenAsync<ScreenHome>();
                        break;
                    default:
                        break;
                }
            }
        }
    }
}