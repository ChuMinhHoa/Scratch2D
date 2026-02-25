using LitMotion;
using Sirenix.OdinInspector;
using UnityEngine;

public class StickerGraphic : MonoBehaviour
{
    public SpriteRenderer sprIcon;
    public int stickerId;
    public UnitAnimation unitAnimation;
    public GameObject objGlow;
    public void InitData(int id)
    {
        stickerId = id;
    }

    [Button]
    public void OnDoneMode()
    {
        objGlow.SetActive(true);
        LMotion.Create(0f, 1f, 0.5f).WithOnComplete(() =>
        {
            objGlow.SetActive(false);
        }).RunWithoutBinding().AddTo(this);
        _ = unitAnimation.PlayScaleAnimation();
    }

    public void MoveSticker(Transform targetPos)
    {
        unitAnimation.PlayMoveAnim(targetPos.position);
    }
}
