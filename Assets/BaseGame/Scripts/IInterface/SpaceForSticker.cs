using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public interface ISpaceForSticker
{
    bool RegisterStickerToFreeSpace();
    bool RegisterSticker(Sticker sticker);
    UniTask MoveStickerDoneToObj(FolderHaveSticker folder, StickerPos stickerPos, StickerDone stickerDone);
    UniTask SpawnStickerDone(StickerPos stickerPos, Sticker sticker, FolderHaveSticker folder = null);
    void ResetController();
}

[Serializable]
public class SpaceForSticker : ISpaceForSticker
{
    public virtual bool RegisterStickerToFreeSpace()
    {
        return false;
    }

    public virtual bool RegisterSticker(Sticker sticker)
    {
        return false;
    }


    public virtual async UniTask MoveStickerDoneToObj(FolderHaveSticker folder, StickerPos stickerPos,
        StickerDone stickerDone)
    {
        stickerDone.PlayAnimRemove();
        stickerPos.RegisterObj(stickerDone);
        await UniTask.WaitForSeconds(1f); // wait anim Remove
        await stickerDone.PlayMoveAnim(stickerPos.trsPos);
        stickerPos.MoveDone();
        var isLastSticker = folder.IsCompleteSticker();
        if (isLastSticker)
        {
            GamePlayManager.Instance.level.MoveFolderOut(folder);
            GlobalEventManager.CheckToCallNextSticker?.Invoke();
        }
    }

    public async UniTask SpawnStickerDone(StickerPos stickerPos, Sticker sticker, FolderHaveSticker folder = null)
    {
        var position = sticker.transform.position;
        position.z = -1;
        var e = PoolManager.Instance.SpawnStickerDone(stickerPos.trsPos);
        e.transform.eulerAngles = Vector3.zero;
        e.InitStickerMove(sticker.stickerData.stickerID);
        stickerPos.RegisterObj(e);
        e.transform.position = position;
        sticker.DisAbleIcon();
        e.PlayAnimRemove();
        await UniTask.WaitForSeconds(1f); // wait anim Remove
        if (!folder)
        {
            await e.PlayMoveAnimToFreeSpace(stickerPos.trsPos);
            stickerPos.MoveDone();
        }
        else
        {
            await UniTask.WaitUntil(() => folder.readyToMove);
            await e.PlayMoveAnim(stickerPos.trsPos);
            stickerPos.MoveDone();
            var isLastSticker = folder.IsCompleteSticker();
            if (isLastSticker)
            {
                GamePlayManager.Instance.level.MoveFolderOut(folder);
                GlobalEventManager.CheckToCallNextSticker?.Invoke();
            }
        }
    }

    public virtual void ResetController()
    {
        
    }
}