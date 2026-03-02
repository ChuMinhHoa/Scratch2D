using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

[Serializable]
public class ObjHaveStickerController : SpaceForSticker
{
    [ShowInInspector]
    [field: SerializeField]
    public Queue<FolderHaveSticker> objHaveStickers { get; set; } = new();

    public Transform posFirstSpawn;
    public Transform posOut;
    public bool loadDone;

    [field: SerializeField] private SlotFolder[] SlotFolders { get; set; }

    public void LoadData(Span<ObjHaveStickerData> objSticker)
    {
        for (var i = 0; i < objSticker.Length; i++)
        {
            var ot = PoolManager.Instance.SpawnObjHaveSticker();
            ot.LoadData(objSticker[i]);
            ot.transform.localPosition = Vector3.zero;
            objHaveStickers.Enqueue(ot);
        }

        loadDone = true;
    }

    public override bool RegisterSticker(Sticker sticker)
    {
        for (var i = 0; i < SlotFolders.Length; i++)
        {
            if (!SlotFolders[i].IsHaveObject()) continue;
            var folder = SlotFolders[i].folderPos.obj;
            if (!folder.IsSameSticker(sticker.stickerData.stickerID, out var stickerPos)) continue;
            _ = SpawnStickerDone(stickerPos, sticker, folder);
            return true;
        }

        return false;
    }

    public bool RegisterStickerDoneFromFreeSpace(StickerDone stickerDone)
    {
        for (var i = 0; i < SlotFolders.Length; i++)
        {
            if (!SlotFolders[i].IsHaveObject()) continue;
            var folder = SlotFolders[i].folderPos.obj;
            if (!folder.IsSameSticker(stickerDone.stickerId, out var stickerPos)) continue;
            _ = MoveStickerDoneToObj(folder, stickerPos, stickerDone);
            return true;
        }

        return false;
    }

    public async UniTask CallNextObjSticker(bool callFromLoad)
    {
        if (!callFromLoad) await UniTask.WaitForSeconds(2.5f);
        if (objHaveStickers.Count > 0)
        {
            for (var i = 0; i < SlotFolders.Length; i++)
            {
                if (objHaveStickers.Count == 0)
                    break;
                if (!SlotFolders[i].IsAbleFolder()) continue;
                var folder = objHaveStickers.Dequeue();
                SlotFolders[i].SetFolder(folder);
                await folder.MoveToTarget(SlotFolders[i].folderPos.trsPos.position);
                SlotFolders[i].folderPos.MoveDone();
            }
        }
        else
        {
            for (var i = 0; i < SlotFolders.Length; i++)
            {
                if (SlotFolders[i].IsHaveObject()) return;
            }
            Debug.Log("End game!");
            GamePlayManager.Instance.level.ResetLevel();
        }
    }

    public override void ResetController()
    {
        objHaveStickers.Clear();
    }

    public void MoveFolderOut(FolderHaveSticker folder)
    {
        _ = folder.MoveOut(posOut);
        for (var i = 0; i < SlotFolders.Length; i++)
        {
            if (SlotFolders[i].folderPos.obj != folder) continue;
            SlotFolders[i].folderPos.ResetPos();
        }
    }

    public bool IsHaveFolderOnMove(out FolderPos folder)
    {
        if (objHaveStickers.Count == 0)
        {
            folder = null;
            return false;
        }

        for (var i = 0; i < SlotFolders.Length; i++)
        {
            if (SlotFolders[i].folderPos.obj == null) continue;
            if (SlotFolders[i].folderPos.IsMoveDone()) continue;
            folder = SlotFolders[i].folderPos;
            return true;
        }
        
        folder = null;
        return false;
    }
}