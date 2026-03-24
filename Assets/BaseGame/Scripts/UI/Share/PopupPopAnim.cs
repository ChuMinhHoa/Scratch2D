using System;
using Cysharp.Threading.Tasks;
using LitMotion;
using TW.UGUI.Shared;
using UnityEngine;

public class PopupPopAnim : TransitionAnimationBehaviour
{
    public override void Setup()
    {
            
    }

    public override async UniTask PlayAsync(IProgress<float> progress = null)
    {
        await LMotion.Create(1f, 1.1f, 0.1f)
            .Bind(x => RectTransform.transform.localScale = Vector3.one * x)
            .AddTo(RectTransform);
        await LMotion.Create(1.1f, 0f, 0.1f)
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
