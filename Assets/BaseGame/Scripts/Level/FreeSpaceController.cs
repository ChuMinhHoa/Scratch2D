using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

[Serializable]
public class FreeSpaceController : SpaceForSticker
{
    public SpaceSticker[] spaceStickers;
    [ShowInInspector] public List<StickerDone> stickerDoneWait = new();

    public bool IsHaveStickerWait()
    {
        return stickerDoneWait.Count > 0;
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

        for (var i = stickerDoneWait.Count - 1; i >= 0; i--)
        {
            if (i >= stickerDoneWait.Count) continue;

            await UniTask.Yield();
            stickerDoneWait[i].CheckMoveToFolder(true);
        }

        //Level.Instance.CheckToCallChangeLayerIndex();

        // if (stickerDoneWait.Count > 0) 
        //     Level.Instance.CheckLoseGame();
    }

    public void RegisterStickerDoneWait(StickerDone stickerDone)
    {
        stickerDoneWait.Add(stickerDone);

        Level.Instance.CheckLoseGame();
    }

    public void RemoveStickerDoneFromNoWhere(StickerDone e)
    {
        stickerDoneWait.Remove(e);
        if (stickerDoneWait.Count == 0)
        {
            GamePlayManager.Instance.NextLayer();
        }
    }
}