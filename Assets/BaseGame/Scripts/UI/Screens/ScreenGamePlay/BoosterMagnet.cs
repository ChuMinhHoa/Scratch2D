using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BoosterMagnet : BoosterBase
{
    public int stickerID;
    
    Dictionary<SelectAbleOnBooster, FolderHaveSticker> dataHaveStickerDict = new Dictionary<SelectAbleOnBooster, FolderHaveSticker>();
    
    public override void UseBooster()
    {
        base.UseBooster();
        Debug.Log("Use Booster Magnet");
    }

    public override void ActiveBooster(bool active)
    {
        base.ActiveBooster(active);
        Debug.Log($"Active Booster {active}");
    }

    public override void UsedBooster(SelectAbleOnBooster data)
    {
        base.UsedBooster(data);
        var folder = GetFolderHaveSticker(data);
        var stickerId = folder.data.stickerId;
        Debug.Log("Sticker ID: " + stickerId);
        GlobalEventManager.OnRemoveSticker?.Invoke(stickerId, 3);
    }

    private FolderHaveSticker GetFolderHaveSticker(SelectAbleOnBooster data)
    {
        if (dataHaveStickerDict.TryGetValue(data, out FolderHaveSticker folderHaveSticker))
            return folderHaveSticker;
        var folder = data.GetComponent<FolderHaveSticker>();
        dataHaveStickerDict.Add(data, folder);
        return folder;
    }

    public override void OnChangeBoosterCount(int count)
    {
        base.OnChangeBoosterCount(count);
        Debug.Log($"Change Booster count: {count}");
    }
}
