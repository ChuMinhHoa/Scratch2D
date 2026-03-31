using System;
using Core.UI.Activities;
using Core.UI.Screens;
using Cysharp.Threading.Tasks;
using TW.UGUI.MVPPattern;
using UnityEngine;
using R3;
using SDK;
using Sirenix.OdinInspector;
using TW.UGUI.Core.Modals;
using UnityEngine.Events;
using UnityEngine.UI;

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

            [field: SerializeField]
            public MainContentBase<SlotRevive, ReviveType> MainContentRevive { get; private set; }

            [field: SerializeField] public Button BtnClose { get; private set; }

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
                View.BtnClose.onClick.AddListener(() => _ = QuitGame());
            }

            private async UniTask QuitGame()
            {
                CloseModal();
                await UIManager.Instance.OpenActivityAsync<ActivityLoseGame>();
            }

            private UnityAction actionCallBack;

            private void SlotReviveCallBack(SlotRevive slotRevive)
            {
                var useAds = slotRevive.useByAds;

                switch (slotRevive.slotData)
                {
                    case ReviveType.AddNote:
                        IngameFirebaseAnalystic.Instance.SetAdsRewardInfo("ads_reward_slot_folder", 1);
                        actionCallBack = AddNote;
                        break;
                    case ReviveType.AddSlot:
                        IngameFirebaseAnalystic.Instance.SetAdsRewardInfo("ads_reward_reviveAddSlot", 1);
                        actionCallBack = AddSlot;
                        break;
                    case ReviveType.BoosterMagnet:
                        IngameFirebaseAnalystic.Instance.SetAdsRewardInfo("ads_reward_reviveMagnet", 1);
                        actionCallBack = UseBoosterMagnet;
                        break;
                    default:
                        return;
                }

                if (useAds)
                {
#if UNITY_EDITOR
                    actionCallBack?.Invoke();
#endif
                   
#if !UNITY_EDITOR
                    if (!ShopManager.Instance.NoAds.Value)
                    {
                        AdsManager.Instance.ShowRewardVideo(nameof(PlacementType.InGame),$"Revive{actionCallBack}", actionCallBack); 
                    }else
                    {
                        actionCallBack?.Invoke();
                    }
#endif
                }
                else
                {
                    var e = PlayerResourceManager.Instance.IsEnoughResource(GameResource.Type.Money, slotRevive.price);
                    if (!e)
                    {
                        GlobalEventManager.OnShowWarning?.Invoke(MyCache.warningPrice);
                        return;
                    }
                    actionCallBack?.Invoke();
                    PlayerResourceManager.Instance.ChangeResource(GameResource.Type.Money, -slotRevive.price);
                }
            }

            private void UseBoosterMagnet()
            {
                ActionReviveDone();
                PlayerResourceManager.Instance.ChangeResource(GameResource.Type.BoosterMagnet, 1);
                ScreenGamePlayContext.Events.UseBooster?.Invoke(BoosterType.BoosterMagnet);
                CloseModal();
                GamePlayManager.Instance.ChangeGameState(GameState.OnBooster);
            }

            private void AddSlot()
            {
                ActionReviveDone();
                PlayerResourceManager.Instance.ChangeResource(GameResource.Type.BoosterMagnet, 1);
                ScreenGamePlayContext.Events.UseBooster?.Invoke(BoosterType.BoosterAddSlot);
                CloseModal();
            }

            private void AddNote()
            {
                ActionReviveDone();
                Level.Instance.AddSlotNote();
                CloseModal();
            }

            private void ActionReviveDone()
            {
                IngameFirebaseAnalystic.Instance.AddUseRevive();
                GamePlayManager.Instance.ChangeGameState(GameState.Playing);
                Level.Instance.isEndGame = false;
            }

            private void CloseModal() => _ = UIManager.Instance.CloseModalAsync();
        }
    }
}