using System;
using LitMotion;
using UnityEngine;
using UnityEngine.UI;

public enum SlotTabType
{
    None = -1,
    Shop,
    Home,
    ComingSoon
}

public class SlotTabMenu : MonoBehaviour
{
    public Button btnChoose;
    public SlotTabType slotTabType;

    private Action<SlotTabType> actionCallBackOnChooseTab;

    [SerializeField] private LayoutElement layoutElement;
    [SerializeField] private Image imgIcon;
    [SerializeField] private float sizeTabOnChoose = 1.2f;
    [SerializeField] private float sizeTabDefault = 1f;
    [SerializeField] private Vector3 offSetIcon;
    [SerializeField] private Vector3 posIconDefault;
    [SerializeField] private Vector3 posIconTarget;
    
    private MotionHandle motionMove;
    private MotionHandle motionIconScale;

    public void SetActionCallBackOnChooseTab(Action<SlotTabType> action)
    {
        actionCallBackOnChooseTab = action;
    }

    private void Awake()
    {
        btnChoose.onClick.AddListener(OnChooseTab);
    }

    private void Start()
    {
        posIconTarget = posIconDefault + offSetIcon;
    }

    private void OnChooseTab()
    {
        actionCallBackOnChooseTab?.Invoke(slotTabType);
    }

    public void OnSelect()
    {
        if (motionIconScale.IsPlaying())
            motionIconScale.TryCancel();

        if (motionMove.IsPlaying())
            motionMove.TryCancel();

        var currentIconPos = imgIcon.transform.localPosition;
        motionMove = LMotion.Create(currentIconPos, posIconTarget, 0.15f).Bind(x => imgIcon.transform.localPosition = x)
            .AddTo(this);

        layoutElement.flexibleWidth = sizeTabOnChoose;

        var currentScaleIcon = imgIcon.transform.localScale;
        motionIconScale = LMotion.Create(currentScaleIcon, Vector3.one, 0.15f)
            .Bind(x => imgIcon.transform.localScale = x)
            .AddTo(this);
        
    }

    public void OnDeSelectTab()
    {
        if (motionIconScale.IsPlaying())
            motionIconScale.TryCancel();

        if (motionMove.IsPlaying())
            motionMove.TryCancel();

        var currentIconPos = imgIcon.transform.localPosition;

        motionMove = LMotion.Create(currentIconPos, posIconDefault, 0.15f).Bind(x => imgIcon.transform.localPosition = x)
            .AddTo(this);

        layoutElement.flexibleWidth = sizeTabDefault;

        var currentScaleIcon = imgIcon.transform.localScale;
        motionIconScale = LMotion.Create(currentScaleIcon, Vector3.one * 0.7f, 0.15f)
            .Bind(x => imgIcon.transform.localScale = x)
            .AddTo(this);
    }
}