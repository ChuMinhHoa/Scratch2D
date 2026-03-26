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
    public class ScreenShopInGame : Screen
    {
        [field: SerializeField] public ScreenShopInGameContext.UIPresenter UIPresenter { get; private set; }

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
    public class ScreenShopInGameContext
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
            [field: SerializeField]
            public MainContentBase<SlotPack, ShopPackageDataConfig> MainCoinContent { get; private set; }
            [field: SerializeField] public Button BtnCloseInGame { get; private set; }

            public UniTask Initialize(Memory<object> args)
            {
                return UniTask.CompletedTask;
            }

            public void InitCoinSlot(Action<SlotPack> actionSlotCoinCallBack)
            {
                MainCoinContent.SetActionSlotCallBack(actionSlotCoinCallBack);
                MainCoinContent.SetActionSlotExistCallBack();
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
                View.BtnCloseInGame?.onClick.AddListener(()=>_ = CloseScreen());

                View.InitCoinSlot(ActionBuyCallback);
            }
             private async UniTask CloseScreen()
            {
                await UIManager.Instance.CloseScreenDefaultAsync();
                GamePlayManager.Instance.BackToLastState();
            }

            private void ActionBuyCallback(SlotPack slotPackCallBack)
            {
                Debug.Log(slotPackCallBack.slotData.packageName);

                switch (slotPackCallBack.slotData.purchaseType)
                {
                    case PurchaseType.IAPPay:
                        var packageId = MyCache.GetPackageIdByPackageName(slotPackCallBack.slotData.packageName);
                        InGamePurchaseManager.Instance.PurchaseIAPProduct(packageId,
                            () => OnPurchaseSuccess(slotPackCallBack.slotData),
                            () => OnPurchaseFailed(slotPackCallBack.slotData));
                        break;
                    case PurchaseType.ResourcePay:
                        OnPurchaseBuyResourcePay(slotPackCallBack.slotData);
                        break;
                    default:
                        break;
                }
            }

            private void OnPurchaseBuyResourcePay(ShopPackageDataConfig packageConfig)
            {
                GameResource.Type resourceType = packageConfig.resourcePrice.ResourceType;
                BigNumber amount = packageConfig.resourcePrice.Amount;
                if (PlayerResourceManager.Instance.IsEnoughResource(resourceType, amount))
                {
                    //_ = UIManager.Instance.OpenModalAsync<ModalConfirmShop>(packageConfig);
                }
                else
                {
                    //_ = UIManager.Instance.OpenModalAsync<ModalPayResourcePremium>(packageConfig);
                }
            }

            private void OnPurchaseSuccess(ShopPackageDataConfig packageConfig)
            {
                Debug.Log("Purchase Success: " + packageConfig.packageName);
                ShopManager.Instance.PurchaseSuccess(packageConfig);
            }

            private void OnPurchaseFailed(ShopPackageDataConfig packageConfig)
            {
                Debug.Log("Purchase Failed: " + packageConfig.packageName);
                ShopManager.Instance.PurchaseFailed(packageConfig);
            }
            
        }
    }
}