using Cysharp.Threading.Tasks;
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
    public async UniTask OnDoneMode()
    {
        objGlow.SetActive(true);
        _ = unitAnimation.PlayScaleAnimation();
        await LMotion.Create(0f, 1f, 0.5f).WithOnComplete(() =>
        {
            objGlow.SetActive(false);
        }).RunWithoutBinding().AddTo(this);
    }

    public void DisAbleIcon()
    {
        sprIcon.gameObject.SetActive(false);
    }
    
    public void ResetGraphic()
    {
        sprIcon.gameObject.SetActive(true);
        objGlow.SetActive(false);
    }
}
