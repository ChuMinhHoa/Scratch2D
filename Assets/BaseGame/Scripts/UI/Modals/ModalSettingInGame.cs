using System;
using Core.UI.Activities;
using Core.UI.Screens;
using Cysharp.Threading.Tasks;
using R3;
using TW.UGUI.MVPPattern;
using UnityEngine;
using Sirenix.OdinInspector;
using TW.UGUI.Core.Modals;
using TW.Utility.CustomType;
using UniRx;
using UnityEngine.UI;

namespace Core.UI.Modals
{
    public class ModalSettingInGame : Modal
    {
        [field: SerializeField] public ModalSettingInGameContext.UIPresenter UIPresenter { get; private set; }

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
    public class ModalSettingInGameContext
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

            public Reactive<BigNumber> energy = new(0); 

            public UniTask Initialize(Memory<object> args)
            {
                energy = EnergyManager.Instance.energyResource.ReactiveAmount;
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
            public Button BtnClose { get; private set; }
            [field: SerializeField] public Button BtnClose2 { get; private set; }
            [field: SerializeField] public Button BtnContinue { get; private set; }
            
            [field: SerializeField]
            public Button BtnHome { get; private set; }
            
            [field: SerializeField]
            public Button BtnReplay { get; private set; }

            public UniTask Initialize(Memory<object> args)
            {
                return UniTask.CompletedTask;
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
                
                View.BtnClose.onClick.AddListener(OnClickBtnClose);
                View.BtnClose2.onClick.AddListener(OnClickBtnClose);
                View.BtnContinue.onClick.AddListener(OnClickBtnClose);
                View.BtnHome.onClick.AddListener(() => _ = OnClickBtnHome());
                View.BtnReplay.onClick.AddListener(() => _ = OnClickBtnReplay());

                Model.energy.Subscribe(ChangeEnergy).AddTo(View.MainView);
            }

            public void ChangeEnergy(BigNumber energyChange)
            {
                View.BtnReplay.interactable = energyChange > 0 || EnergyManager.Instance.isOnInfiniteEnergy;
            }

            private async UniTask OnClickBtnReplay()
            {
                SoundManager.Instance.PlaySoundSfx(AudioKey.Sfx_ButtonClick);
                if (EnergyManager.Instance.isOnInfiniteEnergy)
                {
                    await Replay();
                }
                else
                    await UIManager.Instance.OpenActivityAsync<ActivityWarningLoseEnergy>((Func<UniTask>)ConfirmReplay); 
            }
            
            private async UniTask ConfirmReplay()
            {
                EnergyManager.Instance.UseEnergy(1);
                await Replay();
            }

            private async UniTask Replay()
            {
                IngameFirebaseAnalystic.Instance.SetLoseType(LoseType.Replay);
                IngameFirebaseAnalystic.Instance.SetNoteFail(Level.Instance.GetNoteFail());
                IngameFirebaseAnalystic.Instance.TrackLevelFail();
                
                Level.Instance.ResetLevel();
                await UIManager.Instance.OpenActivityAsync<ActivityLoadingInGamePlay>((Func<UniTask>)Level.Instance.LoadData, (Func<UniTask>)Level.Instance.AnimFirstSpawn);  
                await UIManager.Instance.CloseModalAsync();
            }

            private async UniTask OnClickBtnHome()
            {
                await BackToHome();
            }

            private async UniTask BackToHome()
            {
                //SoundManager.Instance.PlaySoundSfx(AudioKey.Sfx_ButtonClick);
                IngameFirebaseAnalystic.Instance.SetLoseType(LoseType.BackHome);
                IngameFirebaseAnalystic.Instance.SetNoteFail(Level.Instance.GetNoteFail());
                IngameFirebaseAnalystic.Instance.TrackLevelFail();
                await UIManager.Instance.OpenActivityAsync<ActivityLoadingInGamePlay>(null, null);
                Level.Instance.ResetLevel();
                await UIManager.Instance.CloseScreenAsync();
                await UIManager.Instance.OpenScreenDefaultAsync<ScreenDefault>();
                await UIManager.Instance.CloseModalAsync();
            }

            private void OnClickBtnClose()
            {
                SoundManager.Instance.PlaySoundSfx(AudioKey.Sfx_ButtonClick);
                UIAnimManager.Instance.AnimButton(View.BtnClose.transform);
                _ = UIManager.Instance.CloseModalAsync();
                ScreenGamePlayContext.Events.OnActiveInteractable?.Invoke(true);
                SoundManager.Instance.PlaySoundSfx(AudioKey.Sfx_Pop);
            }
        }
    }
}