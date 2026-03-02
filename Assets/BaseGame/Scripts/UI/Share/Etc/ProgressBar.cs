using Cysharp.Text;
using Cysharp.Threading.Tasks;
using LitMotion;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class ProgressBar : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI txtProgress;
    [SerializeField] private RectTransform rectFill;
    [SerializeField] private RectTransform rectMask;
    [SerializeField] private Vector2 vectorSizeDelta;
    [SerializeField] private AnimationCurve curveTextScale;
    [SerializeField] private UnitAnimation animTextProgress;
    [SerializeField] private bool needAnim;

    private void Awake()
    {
        vectorSizeDelta = rectMask.sizeDelta;
        vectorSizeDelta.x = 0;
        rectFill.sizeDelta = vectorSizeDelta;
    }

    [Button]
    public async UniTask ChangeTextProgress(float strProgress)
    {
        if (needAnim) await animTextProgress.PlayScaleAnimation();
        txtProgress.SetTextFormat("{0}", strProgress);
    }

    [Button]
    public void ChangeProgress(float value)
    {
        if (value > 1)
            value = 1;

        vectorSizeDelta = new Vector2(rectMask.rect.size.x * value, rectMask.rect.height);

        var sizeCurrent = rectFill.rect.size.x;
        var fillValue = new Vector2(sizeCurrent, rectMask.rect.height);

        LMotion.Create(sizeCurrent, vectorSizeDelta.x, 0.25f).Bind(x =>
        {
            fillValue.x = x;
            rectFill.sizeDelta = fillValue;
        }).AddTo(this);
    }
}