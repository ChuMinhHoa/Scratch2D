using System;
using Core.UI.Activities;
using Core.UI.Modals;
using Cysharp.Text;
using Cysharp.Threading.Tasks;
using TW.UGUI.MVPPattern;
using UnityEngine;
using R3;
using Sirenix.OdinInspector;
using TMPro;
using TW.UGUI.Core.Screens;
using UniRx;
using UnityEngine.UI;
using Screen = TW.UGUI.Core.Screens.Screen;

namespace Core.UI.Screens
{
    public class ScreenGamePlay : Screen
    {
        [field: SerializeField] public ScreenGamePlayContext.UIPresenter UIPresenter { get; private set; }

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
    public class ScreenGamePlayContext
    {
        public static class Events
        {
            public static Action SampleEvent { get; set; }
            public static Action<bool> OnActiveInteractable { get; set; }

            public static Action<BoosterType> UseBooster { get; set; }
        }

        [HideLabel]
        [Serializable]
        public class UIModel : IAModel
        {
            [field: Title(nameof(UIModel))]
            
            public Reactive<int> level = new(0);
            public Reactive<int> countDone = new(0);
            public Reactive<int> maxCount = new(0);
            public UniTask Initialize(Memory<object> args)
            {
                level = Level.Instance.levelIndex;
                countDone = Level.Instance.oSController.countDone;
                maxCount = Level.Instance.oSController.totalCount;
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
            [field: SerializeField] public Button BtnSetting { get; private set; }
            [field: SerializeField] public TextMeshProUGUI TxtLevel { get; private set; }
            [field: SerializeField] public TextMeshProUGUI TxtCount { get; private set; }
            [field: SerializeField] public BtnBooster[] BtnBoosters { get; private set; }
            [field: SerializeField] public GameObject[] ObjDifficult { get; private set; }

            public UniTask Initialize(Memory<object> args)
            {
                return UniTask.CompletedTask;
            }

            public void RefreshBtnBooster()
            {
                for (var i = 0; i < BtnBoosters.Length; i++)
                {
                    BtnBoosters[i].ResetBooster();
                }
            }

            public void OnChangeLevel()
            {
                var difficult = Level.Instance.GetLevelDifficult();
                //Debug.Log(difficult);
                for (var i = 0; i < ObjDifficult.Length; i++)
                {
                    ObjDifficult[i].SetActive(i == (int)difficult);
                }
            }

            public void OnUserBoosterMagnet(BoosterType boosterType)
            {
                for (var i = 0; i < BtnBoosters.Length; i++)
                {
                    if (BtnBoosters[i].IsSameBooster(boosterType))
                    {
                        BtnBoosters[i].UseBooster();
                    }
                }
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
                
                View.BtnSetting.onClick.AddListener(OnClickSetting);
                Model.level.Subscribe(ChangeLevel).AddTo(View.MainView);
                Model.countDone.Subscribe(ChangeTotalCount).AddTo(View.MainView);
                Model.maxCount.Subscribe(ChangeMaxCount).AddTo(View.MainView);
                View.RefreshBtnBooster();

                //await OpenUI();

                View.MainView.interactable = false;
                Events.OnActiveInteractable += CallInteractable;
                Events.UseBooster += UseBooster;
            }

            private void UseBooster(BoosterType boosterType)
            {
                View.OnUserBoosterMagnet(boosterType);
            }

            private void CallInteractable(bool active)
            {
                View.MainView.interactable = active;
            }

            public UniTask Cleanup(Memory<object> args)
            {
                Events.OnActiveInteractable = null;
                Events.UseBooster = null;
                return UniTask.CompletedTask;
            }

            private async UniTask OpenUI()
            {
                await UIManager.Instance.OpenActivityInGameAsync<ActivityWarning>();
                
            }

            public void ChangeMaxCount(int maxChange)
            {
                View.TxtCount.SetTextFormat(MyCache.strProgress, Model.countDone, maxChange);
            }

            public void ChangeTotalCount(int countChange)
            {
                View.TxtCount.SetTextFormat(MyCache.strProgress, countChange, Model.maxCount);
            }

            public void ChangeLevel(int levelChange)
            {
                View.TxtLevel.SetTextFormat(MyCache.strLevel, levelChange + 1);
                View.OnChangeLevel();
            }

            private void OnClickSetting()
            {
                SoundManager.Instance.PlaySoundSfx(AudioKey.Sfx_ButtonClick);
                Events.OnActiveInteractable?.Invoke(false);
                _ = UIAnimManager.Instance.AnimButton(View.BtnSetting.transform, null);
                _ = UIManager.Instance.OpenModalAsync<ModalSettingInGame>();
            }
        }
    }
}