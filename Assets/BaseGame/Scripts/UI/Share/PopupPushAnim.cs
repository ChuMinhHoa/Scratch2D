using System;
using Cysharp.Threading.Tasks;
using LitMotion;
using TW.UGUI.Shared;
using UnityEngine;

public class PopupPushAnim : TransitionAnimationBehaviour
{
    public override void Setup()
    {
            
    }

    public override async UniTask PlayAsync(IProgress<float> progress = null)
    {
        RectTransform.transform.localScale = Vector3.zero;
        await LMotion.Create(0f, 1.2f, 0.15f)
            .Bind(x => RectTransform.transform.localScale = Vector3.one * x)
            .AddTo(RectTransform);
        await LMotion.Create(1.2f, 1f, 0.15f)
            .Bind(x => RectTransform.transform.localScale = Vector3.one * x)
            .AddTo(RectTransform);
    }

    public override void Play(IProgress<float> progress = null)
    {
    }

    public override void Stop()
    {
    }

    public override float TotalDuration { get; }
}
