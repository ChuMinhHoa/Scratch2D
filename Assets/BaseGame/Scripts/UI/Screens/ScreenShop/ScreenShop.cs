using System;
using Cysharp.Threading.Tasks;
using TW.UGUI.MVPPattern;
using UnityEngine;
using R3;
using Sirenix.OdinInspector;
using TW.UGUI.Core.Screens;
using TW.Utility.CustomType;
using UnityEngine.UI;
using Screen = TW.UGUI.Core.Screens.Screen;

namespace Core.UI.Screens
{
    public class ScreenShop : Screen
    {
        [field: SerializeField] public ScreenShopContext.UIPresenter UIPresenter { get; private set; }

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
    public class ScreenShopContext
    {
        public static class Events
        {
            //public static Action ActionGoToShop { get; set; }
            public static Action<PackageName> OnBuyNonConsumeAblePackage { get; set; }
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

            [field: SerializeField]
            public MainContentBase<SlotPack, ShopPackageDataConfig> MainDealContent { get; private set; }
            
            [field: SerializeField]
            public MainContentBase<SlotPack, ShopPackageDataConfig> MainCoinContent { get; private set; }

            [field: SerializeField] public RectTransform mainRect;

            public UniTask Initialize(Memory<object> args)
            {
                return UniTask.CompletedTask;
            }

            public void InitCoinSlot(Action<SlotPack> actionSlotCoinCallBack)
            {
                MainCoinContent.SetActionSlotCallBack(actionSlotCoinCallBack);
                MainCoinContent.SetActionSlotExistCallBack();
            }
            
            public void InitDealSlot(Action<SlotPack> actionSlotCoinCallBack)
            {
                MainDealContent.SetActionSlotCallBack(actionSlotCoinCallBack);
                MainDealContent.SetActionSlotExistCallBack();
            }

            public void OnBuyNonConsumeAblePackageCallBack(PackageName packID)
            {
                for (var i = 0; i < MainDealContent.slots.Count; i++)
                {
                    if (MainDealContent.slots[i].slotData.packageName == packID)
                    {
                        (MainDealContent.slots[i] as SlotPackBundle)?.CheckToShowBundle();
                    }
                }
                
                LayoutRebuilder.ForceRebuildLayoutImmediate(mainRect);
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
                View.InitCoinSlot(ActionBuyCallback);
                View.InitDealSlot(ActionBuyCallback);
                Events.OnBuyNonConsumeAblePackage += OnBuyNonConsumeAblePackageCallBack;
            }

            private void OnBuyNonConsumeAblePackageCallBack(PackageName packID)
            {
                View.OnBuyNonConsumeAblePackageCallBack(packID);
            }

            private void ActionBuyCallback(SlotPack slotPackCallBack)
            {
                ShopManager.Instance.Purchase(slotPackCallBack.slotData);
            }
        }
    }
}