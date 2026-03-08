using System;
using TW.Utility.DesignPattern.UniTaskState;
using UnityEngine;

public partial class SlotFolder : MonoBehaviour
{
    public SlotFolderType slotFolderType;
    public FolderPos folderPos;
    public StateMachine stateMachine;
    public SlotFolderGraphic slotFolderGraphic;
    public ButtonGameObject btnWatchAds;
    public ButtonGameObject btnBuy;
    
    private void Start()
    {
        stateMachine.RequestTransition(SlotFolderInitState);
        stateMachine.Run();
        
        btnWatchAds.AddListeners(CallWatchAds);
        btnBuy.AddListeners(CallBuy);
    }

    private void CallBuy()
    {
        ChangeFolderType(SlotFolderType.Normal);
    }

    private void CallWatchAds()
    {
        ChangeFolderType(SlotFolderType.Normal);
    }

    public bool IsHaveObject()
    {
        if (slotFolderType == SlotFolderType.Ads)
            return true;
        return folderPos.IsHaveObj();
    }

    public void SetFolder(FolderHaveSticker folder)
    {
        folderPos.RegisterObj(folder);
    }

    private void ChangeFolderType(SlotFolderType folderType)
    {
        slotFolderType = folderType;
        stateMachine.RequestTransition(SlotFolderInitState);
        stateMachine.Run();
    }

    public bool IsAbleFolder()
    {
        return slotFolderType == SlotFolderType.Normal && !IsHaveObject();
    }

    public void ResetSlotFolder()
    {
        folderPos.ResetPos();
    }

    public int GetNoteId()
    {
        if (!IsHaveObject()) return -1;
        if (slotFolderType != SlotFolderType.Normal) return -1;
        return folderPos.obj.data.stickerId;
    }
}

public enum SlotFolderType
{
    Normal,
    Coin,
    Ads,
}

