using Cysharp.Threading.Tasks;
using LitMotion;
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

    public void ResetSpace()
    {
        var e = stickerPos.obj;
        if (e != null)
            PoolManager.Instance.DespawnStickerMove(e);

        stickerPos.ResetPos();
    }

    public async UniTask ResetPos()
    {
        stickerPos.ResetPos();
        await UniTask.WaitForSeconds(0.7f);
        Level.Instance.fSpaceController.CheckStickerDone();
    }

    private MotionHandle moveHandle;

    public void MoveFreeSpaceSticker(Vector3 targetPos)
    {
        if (moveHandle.IsActive())
            moveHandle.TryCancel();
        var currentPos = transform.localPosition;
        moveHandle = LMotion.Create(currentPos, targetPos, 0.1f)
            .Bind(x => transform.localPosition = x);
    }
}