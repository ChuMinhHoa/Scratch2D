using System;
using System.Collections.Generic;
using TW.Utility.DesignPattern;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class StickerDoneManager : Singleton<StickerDoneManager>
{
    private readonly List<StickerDone> stickerDoneStack = new ();

    public List<StickerDone> stickerDoneOnMoveToNote = new();
    public void AddStickerDone(StickerDone stickerDone)
    {
        if (stickerDoneStack.Contains(stickerDone))
            return;
        stickerDoneStack.Add(stickerDone);
    }
    
    private void Start()
    {
        this.UpdateAsObservable().Subscribe(ExecuteNextFrameEvents).AddTo(this);
    }

    private void ExecuteNextFrameEvents(Unit _)
    {
        if (stickerDoneStack.Count > 0)
        {
            var stickerDone = stickerDoneStack[0];
            stickerDoneStack.Remove(stickerDone);
            var isFromNoWhere = Level.Instance.fSpaceController.IsFromNoWhere(stickerDone);
            var isFromFreeSpace = Level.Instance.fSpaceController.IsFromFreeSpace(stickerDone);
            stickerDone.CheckMoveToFolder(isFromNoWhere, isFromFreeSpace);
            
        }
    }

    public void Clear()
    {
        stickerDoneStack.Clear();
    }

    #region StickerDone On Move To Note

    public void AddStickerDoneMoveToNote(StickerDone stickerDone)
    {
        if (stickerDoneOnMoveToNote.Contains(stickerDone))
            return;
        stickerDoneOnMoveToNote.Add(stickerDone);
    }
    
    public void RemoveStickerDoneMoveToNote(StickerDone stickerDone)
    {
        if (!stickerDoneOnMoveToNote.Contains(stickerDone))
            return;
        stickerDoneOnMoveToNote.Remove(stickerDone);
    }
    
    public bool IsHaveStickerDoneMoveToNote() => stickerDoneOnMoveToNote.Count > 0;

    #endregion
}
