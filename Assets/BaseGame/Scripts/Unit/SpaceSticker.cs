using UnityEngine;

public class SpaceSticker : MonoBehaviour
{
    public StickerPos stickerPos;
    public bool watchedAds;
    
    public bool IsFreeSpace(out StickerPos stickerTrs)
    {
        if (!stickerPos.IsHaveObj())
        {
            stickerTrs = stickerPos;
            return true;
        }
        stickerTrs = null;
        return false;
    }
}
