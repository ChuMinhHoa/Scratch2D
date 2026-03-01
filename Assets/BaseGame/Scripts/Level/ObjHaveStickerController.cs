using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using LitMotion;
using Sirenix.OdinInspector;
using UnityEngine;

[Serializable]
public class ObjHaveStickerController : SpaceForSticker
{
    [ShowInInspector]
    [field: SerializeField]
    public Queue<FolderHaveSticker> objHaveStickers { get; set; } = new();

    public Transform posParents;
    public Transform posFirstSpawn;
    public Transform posOut;
    public Vector3 offSet;
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

    public async UniTask CallNextObjSticker()
    {
        await UniTask.WaitForSeconds(0.5f);
        if (objHaveStickers.Count > 0)
        {
            for (var i = 0; i < SlotFolders.Length; i++)
            {
                if (objHaveStickers.Count == 0)
                    break;
                if (SlotFolders[i].IsHaveObject()) continue;
                var folder = objHaveStickers.Dequeue();
                _ = folder.MoveToTarget(SlotFolders[i].folderPos.trsPos.position);
                SlotFolders[i].SetFolder(folder);
            }
        }
        else
        {
            for (var i = 0; i < SlotFolders.Length; i++)
            {
                if (SlotFolders[i].IsHaveObject()) return;
            }
            Debug.Log("End game!");
            ResetController();
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
}