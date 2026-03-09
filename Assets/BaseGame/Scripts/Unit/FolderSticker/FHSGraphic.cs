using Cysharp.Threading.Tasks;
using LitMotion;
using Sirenix.OdinInspector;
using UnityEngine;

public class FHSGraphic : MonoBehaviour
{
    [SerializeField] private SpriteRenderer sprIcon;
    [SerializeField] private SpriteRenderer[] sprStickerIcons;

    public void InitData(int dataStickerId, int objId)
    {
        sprIcon.sprite = SpriteGlobalConfig.Instance.GetIconObjectHaveSticker(objId);
        for (var i = 0; i < sprStickerIcons.Length; i++)
        {
            var spriteIcon = SpriteGlobalConfig.Instance.GetStickerBg(dataStickerId);
            if (spriteIcon)
            {
                sprStickerIcons[i].sprite = spriteIcon;
            }
        }
    }
}