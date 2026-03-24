using System;
using Core.UI.Activities;
using Core.UI.Screens;
using Cysharp.Text;
using Cysharp.Threading.Tasks;
using R3;
using Sirenix.OdinInspector;
using TMPro;
using TW.UGUI.Core.Modals;
using TW.UGUI.MVPPattern;
using TW.Utility.CustomType;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class ModalRefill : Modal
{ 
    [field: SerializeField] public ModalRefillContext.UIPresenter UIPresenter { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        AddLifecycleEvent(UIPresenter, 1);
    }

    public override async UniTask Initialize(Memory<object> args)
    {
        await base.Initialize(args);
    }
    [Serializable]
    public class ModalRefillContext
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

            [field: SerializeField] public Reactive<BigNumber> CurrentEnergy { get; set; }

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
            public Button BtnClose { get; private set; }
            [field: SerializeField] public Button BtnClose2 { get; private set; }
            [field: SerializeField] public Button BtnRefillAds { get; private set; }
            [field: SerializeField] public Button BtnRefillFull { get; private set; }
            [field: SerializeField] public TextMeshProUGUI TxtEnergy { get; private set; }

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
                View.BtnRefillAds.onClick.AddListener(OnClickBtnRefillADS);
                View.BtnRefillFull.onClick.AddListener(OnClickBtnRefillFull);

                Model.CurrentEnergy = EnergyManager.Instance.energyResource.ReactiveAmount;
                Model.CurrentEnergy.Subscribe(ChangeEnergy).AddTo(View.MainView);
            }

            public void ChangeEnergy(BigNumber energy)
            {
                var e = energy < DefaultGlobalConfig.Instance.maxEnergy;
                View.TxtEnergy.SetTextFormat(MyCache.strDefault, energy.ToInt());
                View.BtnRefillFull.interactable = e;
                View.BtnRefillAds.interactable = e;
            }

            private void OnClickBtnClose()
            {
                _ = UIAnimManager.Instance.AnimButton(View.BtnClose.transform);
                _ = UIManager.Instance.CloseModalAsync();
            }

            private void OnClickBtnRefillADS()
            {
                EnergyManager.Instance.AddOneEnergy();
            }

            private void OnClickBtnRefillFull()
            {
                EnergyManager.Instance.RefillFullEnergy();
            }
        }
    }
}


