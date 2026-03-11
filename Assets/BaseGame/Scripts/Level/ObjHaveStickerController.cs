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
    public bool isEndGame;
    [field: SerializeField] public SlotFolder[] SlotFolders { get; set; }

    public async UniTask LoadData(ObjHaveStickerData[] objSticker)
    {
        isEndGame = false;
        for (var i = 0; i < objSticker.Length; i++)
        {
            var ot = PoolManager.Instance.SpawnObjHaveSticker();
            ot.LoadData(objSticker[i]);
            ot.transform.localPosition = Vector3.zero;
            objHaveStickers.Enqueue(ot);
        }

        await CallNextObjSticker(true);
        loadDone = true;
    }

    private UniTask checkLoseTask;

    public async UniTask CallNextObjSticker(bool callFromLoad = false)
    {
        if (!callFromLoad) await UniTask.WaitForSeconds(0.5f);
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
            if (isEndGame) return;
            for (var i = 0; i < SlotFolders.Length; i++)
            {
                if (SlotFolders[i].slotFolderType == SlotFolderType.Ads)
                    continue;
                if (SlotFolders[i].IsHaveObject()) return;
            }

            isEndGame = true;

            Debug.Log("End game!");
            GamePlayManager.Instance.level.LevelUp();
        }
    }

    public override void ResetController()
    {
        var objCount = objHaveStickers.Count;
        for (var i = 0; i < objCount; i++)
        {
            var e = objHaveStickers.Dequeue();
            PoolManager.Instance.DespawnObjHaveSticker(e);
        }

        for (var i = 0; i < SlotFolders.Length; i++)
        {
            SlotFolders[i].ResetByLevel();
        }
        objHaveStickers.Clear();
        loadDone = false;
    }

    public void MoveFolderOut(FolderHaveSticker folder)
    {
        _ = folder.MoveOut(posOut);
        for (var i = 0; i < SlotFolders.Length; i++)
        {
            if (SlotFolders[i].folderPos.obj != folder) continue;
            SlotFolders[i].ResetSlotFolder();
        }
    }

    public StickerPos GetFolderPos(int stickerId)
    {
        for (var i = 0; i < SlotFolders.Length; i++)
        {
            if (!SlotFolders[i].folderPos.obj) continue;
            if (!SlotFolders[i].folderPos.IsMoveDone()) continue;
            if (SlotFolders[i].folderPos.obj.IsSameSticker(stickerId, out var stickerPos))
            {
                return stickerPos;
            }
        }

        return null;
    }

    public bool IsHaveAtLeastOneNote()
    {
        return objHaveStickers.Count > 0;
    }

    public bool IsHaveAllNoteOnSlot()
    {
        for (var i = 0; i < SlotFolders.Length; i++)
        {
            if (!SlotFolders[i].IsHaveObject())
            {
                return false;
            }
        }

        return true;
    }
}