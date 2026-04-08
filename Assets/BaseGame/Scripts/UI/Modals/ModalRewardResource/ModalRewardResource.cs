using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TW.UGUI.MVPPattern;
using UnityEngine;
using R3;
using Sirenix.OdinInspector;
using TW.UGUI.Core.Modals;
using UnityEngine.UI;

namespace Core.UI.Modals
{
    public class ModalRewardResource : Modal
    {
        [field: SerializeField] public ModalRewardResourceContext.UIPresenter UIPresenter { get; private set; }

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
    public class ModalRewardResourceContext
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

            public List<GameResource> resourceRewardList = new();

            public UniTask Initialize(Memory<object> args)
            {
                resourceRewardList = RewardManager.Instance.resourceRewardList;
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

            public MainContentBase<SlotReward, GameResource> resourceRewardContent;

            [field: SerializeField] public Button BtnClaim { get; private set; }
            [field: SerializeField] public ScrollRect MyScroll { get; set; }

            public UniTask Initialize(Memory<object> args)
            {
                return UniTask.CompletedTask;
            }

            public void InitData(List<GameResource> modelResourceRewardList)
            {
                resourceRewardContent.InitData(modelResourceRewardList.ToArray());
            }

            public void HideAllSlot()
            {
                for (var i = 0; i < resourceRewardContent.slots.Count; i++)
                {
                    resourceRewardContent.slots[i].SetUpAnimShow();
                }
            }

            public async UniTask AnimShow()
            {
                for (var i = 0; i < resourceRewardContent.slots.Count; i++)
                {
                    await UniTask.WaitForSeconds(0.05f * i);
                    resourceRewardContent.slots[i].AnimShow();
                    //Debug.Log( (float)i / resourceRewardContent.slots.Count);
                    MyScroll.horizontalNormalizedPosition = (float)i / (resourceRewardContent.slots.Count-1);
                }
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

                View.InitData(Model.resourceRewardList);
                View.HideAllSlot();
                View.BtnClaim.onClick.AddListener(ClaimReward);
            }

            public void DidPushEnter(Memory<object> args)
            {
                _ = View.AnimShow();
                SoundManager.Instance.PlaySoundSfx(AudioKey.Sfx_Win);
                SoundManager.Instance.PlaySoundSfx(AudioKey.Sfx_FireWork);
            }

            private void ClaimReward()
            {
                UIAnimManager.Instance.AnimButton(View.BtnClaim.transform);
                RewardManager.Instance.ClearResourceReward();
                _ = UIManager.Instance.CloseModalAsync();
            }
        }
    }
}