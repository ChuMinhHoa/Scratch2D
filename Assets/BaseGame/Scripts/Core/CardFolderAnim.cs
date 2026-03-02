using Cysharp.Threading.Tasks;
using LitMotion;
using Sirenix.OdinInspector;
using UnityEngine;

public class CardFolderAnim : MonoBehaviour
{
    public Vector3 posDefault;
    public Vector3 posOpen;
    public Vector3 posShowUp;
    public Vector3 posWait;
    public Vector3 rotateDefault;
    public AnimationCurve curveShowUp;

    public float timeShowUp;
    public float timeOpen;
    public float timeWait;
    public float timeDelayShowUp;
    public float timeDelayOpen;
    public float timeDelayWait;

    [Button]
    public async UniTask AnimOpen()
    {
        var currentPos = transform.localPosition;
        await LMotion.Create(currentPos, posShowUp, timeShowUp).WithDelay(transform.GetSiblingIndex() * timeDelayShowUp).WithEase(curveShowUp).Bind(x => transform.localPosition = x).AddTo(this);
        currentPos = transform.localPosition;
        await LMotion.Create(currentPos, posOpen, timeOpen).WithDelay(timeDelayOpen).WithEase(curveShowUp).Bind(x => transform.localPosition = x).AddTo(this);
        currentPos = transform.localPosition;
        var currentRotation = transform.localEulerAngles;
        LMotion.Create(currentRotation, Vector3.zero, timeDelayWait).WithDelay(timeDelayWait).Bind(x => transform.localEulerAngles = x).AddTo(this);
        await LMotion.Create(currentPos, posWait, timeWait).WithDelay(timeDelayWait).Bind(x => transform.localPosition = x).AddTo(this);
    }
    [Button]
    public async UniTask AnimClose()
    {
        var currentPos = transform.localPosition;
        var currentRotation = transform.localEulerAngles;
        await UniTask.WaitForSeconds(0.1f * transform.GetSiblingIndex());
        LMotion.Create(currentRotation, rotateDefault, timeWait).Bind(x => transform.localEulerAngles = x).AddTo(this);
        await LMotion.Create(currentPos, posOpen, timeShowUp).Bind(x => transform.localPosition = x).AddTo(this);
        currentPos = transform.localPosition;
        await LMotion.Create(currentPos, posShowUp, timeShowUp).Bind(x => transform.localPosition = x).AddTo(this);
        currentPos = transform.localPosition;
        await LMotion.Create(currentPos, posDefault, timeShowUp).WithDelay(timeDelayWait).Bind(x => transform.localPosition = x).AddTo(this);
    }
}
