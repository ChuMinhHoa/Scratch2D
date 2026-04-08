using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using LitMotion;
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
        cartBooster.ResetCart();
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
    private float spaceCardWidth = 1.75f;

    [Button]
    public void SetPositionSpaceSticker()
    {
        var cartActive = cartBooster.IsActiveBooster();
        var spaceStickerCount = cartActive ? spaceStickers.Count : spaceStickers.Count - 1;
        for (var i = 0; i < spaceStickers.Count; i++)
        {
            var offset = (i - spaceStickerCount / 2f) * spaceWidth;
            var newPos = new Vector3(offset, 0f, 0f);
            spaceStickers[i].MoveFreeSpaceSticker(newPos);
            //spaceStickers[i].transform.localPosition = new Vector3(offset, 0, 0);

            var stickerDone = spaceStickers[i].stickerPos.obj;
            if (stickerDone)
            {
                var parents = spaceStickers[i].transform.parent;
                var worldPos = parents.TransformPoint(newPos);
                stickerDone.MoveToFreeSpaceOnUseBooster(worldPos);
            }
        }

        var cartOffset = (spaceStickerCount / 2f - 1) * spaceWidth + spaceCardWidth;
        var newPosCart = new Vector3(cartOffset, 0f, 0f);
        cartBooster.MoveCartObj(newPosCart);
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

    public async UniTask UseBoosterCart()
    {
        //await UniTask.WaitUntil(() => !UnitEventManager.Instance.IsHaveEvent());
        if (GamePlayManager.Instance.gameState == GameState.LoseGame)
        {
            var price = BoosterGlobalConfig.Instance.GetBoosterConfig(BoosterType.BoosterCart).price;
            PlayerResourceManager.Instance.ChangeResource(GameResource.Type.Money, price);
            cartBooster.ResetCart();
            SetPositionSpaceSticker();
            return;
        }
        var totalTimeWait = 0f;
        for (var i = 0; i < spaceStickers.Count; i++)
        {
            if (!spaceStickers[i].stickerPos.IsHaveObj()) continue;
            var stickerDone = spaceStickers[i].stickerPos.obj;
            _ = cartBooster.AddStickerDone(stickerDone, i);
            totalTimeWait = 0.1f * i + 0.3f;
        }
        await UniTask.WaitForSeconds(totalTimeWait);
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

    public bool IsCanUseBoosterCart()
    {
        var countStickerDone = 0;
        
        for (var i = 0; i < spaceStickers.Count; i++)
        {
            if (spaceStickers[i].stickerPos.IsHaveObj())
                countStickerDone++;
        }

        return countStickerDone > 0;
    }
}