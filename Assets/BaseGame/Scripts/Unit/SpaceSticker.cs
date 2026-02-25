using UnityEngine;

public class SpaceSticker : MonoBehaviour
{
    public StickerPos stickerPos;
    public bool watchedAds;
    
    public bool IsFreeSpace(out StickerPos stickerTrs)
    {
        if (!stickerPos.IsHaveSticker())
        {
            stickerTrs = stickerPos;
            return true;
        }
        stickerTrs = null;
        return false;
    }
    
    public void RegisterSticker(StickerDone sticker)
    {
        stickerPos.RegisterSticker(sticker);
    }
}
