using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using Object = UnityEngine.Object;

[Serializable]
public class FreeSpaceController : SpaceForSticker
{
    public List<SpaceSticker> spaceStickers;
    public SpaceSticker spaceStickerPitch;
    [ShowInInspector] public List<StickerDone> stickerDoneWait = new();
    public GameObject objEffectSpawn;
    public GameObject prefEffectSpawn;
    public CartObjBooster cartBooster;

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

    [Button]
    public void CheckStickerDone()
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

        cartBooster.CheckStickerDone();
    }

    public void RegisterStickerDoneWait(StickerDone stickerDone)
    {
        stickerDoneWait.Add(stickerDone);
        CheckStickerDone();
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
        if (!objEffectSpawn)
        {
            objEffectSpawn = Object.Instantiate(prefEffectSpawn);
            objEffectSpawn.transform.position = spaceStickerPitch.transform.position;
        }

        objEffectSpawn.SetActive(true);
        //if(!UnitEventManager.Instance.IsHaveEvent())
        CheckStickerDone();
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
        cartBooster.ResetCart();
    }

    private float spaceWidth = 1.5f;

    [Button]
    private void SetPositionSpaceSticker()
    {
        for (var i = 0; i < spaceStickers.Count; i++)
        {
            var offset = (i - (spaceStickers.Count - 1) / 2f) * spaceWidth;
            spaceStickers[i].transform.localPosition = new Vector3(offset, 0, 0);
            if (spaceStickers[i].stickerPos.obj)
            {
                spaceStickers[i].stickerPos.obj.transform.position = spaceStickers[i].stickerPos.trsPos.position;
            }
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

    public bool IsCanUseBoosterAddSlot()
    {
        return spaceStickers.Count < 5;
    }

    public void UseBoosterCart()
    {
        for (var i = 0; i < spaceStickers.Count; i++)
        {
            if (spaceStickers[i].stickerPos.IsHaveObj())
            {
                var stickerDone = spaceStickers[i].stickerPos.obj;
                //RegisterStickerDoneWait(stickerDone);
                //spaceStickers[i].stickerPos.ResetPos();
                _ = cartBooster.AddStickerDone(stickerDone, i);
            }
        }
    }

    public void RemoveStickerDoneFromCart(StickerDone stickerDone)
    {
        cartBooster.RemoveStickerDoneFromCart(stickerDone);
    }

    public void ResetPos(StickerPos stickerPos)
    {
        for (var i = 0; i < spaceStickers.Count; i++)
        {
            if (spaceStickers[i].stickerPos != stickerPos) continue;
            _ = spaceStickers[i].ResetPos();
            break;
        }
    }
}