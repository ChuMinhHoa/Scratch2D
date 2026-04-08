using System;
using Cysharp.Threading.Tasks;
using LitMotion;
using UnityEngine;

public class AnimOnBooster : MonoBehaviour
{
    [SerializeField] private float delayTime = 3f;
    [SerializeField] private int objIndex;
    [SerializeField] private float delayAddTime = 0.1f;
    [SerializeField] private float duration = 0.15f;
    [SerializeField] private AnimationCurve curvePos;
    [SerializeField] private AnimationCurve curvePosMagnitude;
    [SerializeField] private float magnitude = 10f;
    private Vector3 posTarget;

    public async UniTask AnimLoop()
    {
        posTarget = transform.localPosition;
        var posStart = transform.localPosition;
        await LMotion.Create(0f, 1f, duration).WithDelay(delayAddTime * objIndex).Bind(x =>
        {
            var magnitudeY = curvePosMagnitude.Evaluate(x);
            var pos = Vector3.Lerp(posStart, posTarget, curvePos.Evaluate(x));
            pos.y += magnitudeY * magnitude;
            transform.localPosition = pos;
        }).AddTo(this);
    }
}
