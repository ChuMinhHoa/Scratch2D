using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using Object = UnityEngine.Object;

[Serializable]
public class FreeSpaceController : SpaceForSticker
{
    public SpaceSticker[] spaceStickers;
    [ShowInInspector] public List<StickerDone> stickerCantMoveAnyWhere = new();

    private void CheckGameOver()
    {
        for (var i = 0; i < spaceStickers.Length; i++)
        {
            if (!spaceStickers[i].stickerPos.IsHaveObj())
                return;
        }

        if (stickerCantMoveAnyWhere.Count == 0)
            return;
        GamePlayManager.Instance.level.GameOver();
    }

    public bool IsHaveStickerWait()
    {
        return stickerCantMoveAnyWhere.Count > 0;
    }

    public StickerPos GetFreeSpacePos()
    {
        for (var i = 0; i < spaceStickers.Length; i++)
        {
            if (!spaceStickers[i].stickerPos.IsHaveObj())
            {
                return spaceStickers[i].stickerPos;
            }
        }

        return null;
    }

    public async UniTask CheckStickerDone()
    {
        for (var i = 0; i < spaceStickers.Length; i++)
        {
            var stickerPos = spaceStickers[i].stickerPos;
            if (stickerPos.IsHaveObj() && stickerPos.IsMoveDone())
            {
                var stickerDone = stickerPos.obj;
                if (stickerDone == null) continue;

                await UniTask.Yield();
                stickerDone.CheckMoveToFolder();
            }
        }

        for (var i = stickerCantMoveAnyWhere.Count - 1; i >= 0; i--)
        {
            if (i >= stickerCantMoveAnyWhere.Count) continue;

            await UniTask.Yield();
            stickerCantMoveAnyWhere[i].CheckMoveToFolder(true);
        }
    }

    public void RegisterStickerDone(StickerDone stickerDone)
    {
        stickerCantMoveAnyWhere.Add(stickerDone);
    }

    public void RemoveStickerDoneFromNoWhere(StickerDone e)
    {
        stickerCantMoveAnyWhere.Remove(e);
    }
}