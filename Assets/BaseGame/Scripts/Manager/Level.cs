using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

public class Level : MonoBehaviour
{
    public ObjHaveSticker currentObjHaveSticker;
    public SpaceSticker[] spaceStickers;

    private void Start()
    {
        GamePlayManager.Instance.level = this;
    }

    public void RegisterStickerDone(Sticker sticker)
    {
        if(RegisterStickerToObj(sticker)) return;
        if (RegisterStickerToFreeSpace(sticker)) return;
        //CheckGameOver();
    }

    private bool RegisterStickerToObj(Sticker sticker)
    {
        if (currentObjHaveSticker != null)
            if (currentObjHaveSticker.IsSameSticker(sticker.stickerID, out var stickerPos))
            {
                _ = SpawnStickerDone(stickerPos, sticker);
                return true;
            }

        return false;
    }

    private bool RegisterStickerDoneToObject(StickerDone stickerDone)
    {
        if (currentObjHaveSticker != null)
            if (currentObjHaveSticker.IsSameSticker(stickerDone.stickerId, out var stickerPos))
            {
                _ = MoveStickerDoneToObj(stickerPos, stickerDone);
                return true;
            }

        return false;
    }
    
    private bool RegisterStickerToFreeSpace(Sticker sticker)
    {
        for (var i = 0; i < spaceStickers.Length; i++)
        {
            if (!spaceStickers[i].IsFreeSpace(out var stickerPos)) continue;
            _ = SpawnStickerDone(stickerPos, sticker, true);
            return true;
        }

        return false;
    }

    private async UniTask SpawnStickerDone(StickerPos stickerPos, Sticker sticker, bool moveToFreSpace = false)
    {
        var position = sticker.transform.position;
        position.z = -1;
        var e = PoolManager.Instance.SpawnStickerDone(stickerPos.trsStickerPos);
        stickerPos.RegisterSticker(e);
        e.transform.position = position;
        sticker.DisAbleIcon();
        e.PlayAnimRemove();
        await UniTask.WaitForSeconds(1f); // wait anim Remove
        if (moveToFreSpace)
        {
            await e.PlayMoveAnimToFreeSpace(stickerPos.trsStickerPos);
        }
        else
        {
            await e.PlayMoveAnim(stickerPos.trsStickerPos);
        }
    }

    private async UniTask MoveStickerDoneToObj(StickerPos stickerPos, StickerDone stickerDone)
    {
        stickerDone.PlayAnimRemove();
        stickerPos.RegisterSticker(stickerDone);
        await UniTask.WaitForSeconds(1f);// wait anim Remove
        await stickerDone.PlayMoveAnim(stickerPos.trsStickerPos);
    }

    private void CheckGameOver()
    {
    }
    
    [Button]
    public void SetCurrentObjHaveSticker(ObjHaveSticker objHaveSticker)
    {
        currentObjHaveSticker = objHaveSticker;
        CheckAllStickerOnFreeSpace();
    }

    private void CheckAllStickerOnFreeSpace()
    {
        for (var i = 0; i < spaceStickers.Length; i++)
        {
            if (!spaceStickers[i].stickerPos.IsHaveSticker())
                continue;
            if (RegisterStickerDoneToObject(spaceStickers[i].stickerPos.sticker))
            {
                spaceStickers[i].stickerPos.RegisterSticker(null);
            }
        }
    }
}