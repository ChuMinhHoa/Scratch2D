using System;
using Cysharp.Threading.Tasks;
using LitMotion;
using Sirenix.OdinInspector;
using TW.Utility.DesignPattern;
using UnityEngine;

public class UIAnimManager : Singleton<UIAnimManager>
{
    [BoxGroup("UI Button Anim")] [SerializeField]
    private AnimationCurve buttonCurveScale;

    [BoxGroup("UI Button Anim")] [SerializeField]
    private AnimationCurve buttonCurveRotate;

    [BoxGroup("UI Button Anim")] [SerializeField]
    private Vector3 buttonRotateEuler;

    public async UniTask AnimButton(Transform trs, Action actionCallBack)
    {
        await LMotion.Create(0f, 1f, .25f)
            .WithOnComplete(() => actionCallBack?.Invoke())
            .Bind(x =>
            {
                trs.localScale = Vector3.one * buttonCurveScale.Evaluate(x);
                trs.eulerAngles = buttonRotateEuler * buttonCurveRotate.Evaluate(x);
            })
            .AddTo(trs.gameObject);
    }

    public MotionHandle AnimButton(Transform trs)
    {
        return LMotion.Create(0f, 1f, .25f)
            .Bind(x =>
            {
                trs.localScale = Vector3.one * buttonCurveScale.Evaluate(x);
                trs.eulerAngles = buttonRotateEuler * buttonCurveRotate.Evaluate(x);
            })
            .AddTo(trs.gameObject);
    }
}