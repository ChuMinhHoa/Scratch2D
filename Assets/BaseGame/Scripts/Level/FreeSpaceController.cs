using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

[Serializable]
public class FreeSpaceController : SpaceForSticker
{
    public List<SpaceSticker> spaceStickers;
    public SpaceSticker spaceStickerPitch;
    [ShowInInspector] public List<StickerDone> stickerDoneWait = new();

    public bool IsHaveStickerWait()
    {
        return stickerDoneWait.Count > 0;
    }

    public StickerPos GetFreeSpacePos(StickerDone stickerDone)
    {
        for (var i = 0; i < spaceStickers.Count; i++)
        {
            if (!spaceStickers[i].stickerPos.IsHaveObj())
            {
                spaceStickers[i].stickerPos.RegisterObj(stickerDone);
                return spaceStickers[i].stickerPos;
            }
        }

        return null;
    }

    public async UniTask CheckStickerDone()
    {
        for (var i = 0; i < spaceStickers.Count; i++)
        {
            var stickerPos = spaceStickers[i].stickerPos;
            if (stickerPos.IsHaveObj() && stickerPos.IsMoveDone())
            {
                var stickerDone = stickerPos.obj;
                if (stickerDone == null) continue;
                StickerDoneManager.Instance.AddStickerDone(stickerDone);
                //stickerDone.CheckMoveToFolder();
            }
        }

        for (var i = stickerDoneWait.Count - 1; i >= 0; i--)
        {
            if (i >= stickerDoneWait.Count) continue;
            StickerDoneManager.Instance.AddStickerDone(stickerDoneWait[i]);
        }

        //Level.Instance.CheckToCallChangeLayerIndex();

        // if (stickerDoneWait.Count > 0) 
        //     Level.Instance.CheckLoseGame();
    }

    public void RegisterStickerDoneWait(StickerDone stickerDone)
    {
        stickerDoneWait.Add(stickerDone);
        //GamePlayManager.Instance.ChangeGameState(GameState.WaitingCheckLoseGame);
        //await UniTask.WaitForSeconds(0.75f);
        // if (!StickerDoneManager.Instance.IsHaveStickerDoneMoveToNote())
             //Level.Instance.CheckLoseGame();
    }

    public void RemoveStickerDoneFromNoWhere(StickerDone e)
    {
        stickerDoneWait.Remove(e);
        if (stickerDoneWait.Count == 0)
        {
            GlobalEventManager.OnHaveCardDone?.Invoke();
        }
    }

    public bool IsCanAddSlot()
    {
        return spaceStickers.Count < 5;
    }

    public void AddSlot()
    {
        spaceStickers.Add(spaceStickerPitch);
        spaceStickerPitch.gameObject.SetActive(true);
        SetPositionSpaceSticker();
    }

    public override void ResetController()
    {
        spaceStickers.Remove(spaceStickerPitch);
        spaceStickerPitch.gameObject.SetActive(false);
        SetPositionSpaceSticker();
        for (int i = 0; i < spaceStickers.Count; i++)
        {
            spaceStickers[i].ResetSpace();
        }
        for (var i = 0; i < stickerDoneWait.Count; i++)
        {
            PoolManager.Instance.DespawnStickerMove(stickerDoneWait[i]);
        }
        stickerDoneWait.Clear();
    }

    private float spaceWidth = 1.5f;
    
    [Button]
    private void SetPositionSpaceSticker()
    {   
        for (var i = 0; i < spaceStickers.Count; i++)
        {
            var offset = (i - (spaceStickers.Count - 1) / 2f) * spaceWidth;
            spaceStickers[i].transform.localPosition = new Vector3(offset, 0, 0);
        }
    }

    public bool IsFromNoWhere(StickerDone stickerDone)
    {
        return stickerDoneWait.Contains(stickerDone);
    }

    public bool IsFromFreeSpace(StickerDone stickerDone)
    {
        for (var i = 0; i < spaceStickers.Count; i++)
        {
            if (spaceStickers[i].stickerPos.obj == stickerDone)
                return true;
        }

        return false;
    }

    public bool IsHaveFreeSlot()
    {
        for (var i = 0; i < spaceStickers.Count; i++)
        {
            if (!spaceStickers[i].stickerPos.IsHaveObj())
                return true;
        }

        return false;
    }
}