using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using Object = UnityEngine.Object;

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
    [field: SerializeField] private SlotFolder[] SlotFolders { get; set; }

    public void LoadData(Span<ObjHaveStickerData> objSticker)
    {
        isEndGame = false;
        for (var i = 0; i < objSticker.Length; i++)
        {
            var ot = PoolManager.Instance.SpawnObjHaveSticker();
            ot.LoadData(objSticker[i]);
            ot.transform.localPosition = Vector3.zero;
            objHaveStickers.Enqueue(ot);
        }

        loadDone = true;
        _ = CallNextObjSticker(true);
    }


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
                if (SlotFolders[i].IsHaveObject()) return;
            }

            isEndGame = true;

            Debug.Log("End game!");
            GamePlayManager.Instance.level.ResetLevel();
        }
    }

    public override void ResetController()
    {
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
}