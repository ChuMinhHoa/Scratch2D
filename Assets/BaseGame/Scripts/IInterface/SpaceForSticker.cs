
using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public interface ISpaceForSticker
{
   bool RegisterStickerToFreeSpace();
   bool RegisterSticker(Sticker sticker);
   bool RegisterStickerDoneToObject(StickerDone stickerDone);
   UniTask MoveStickerDoneToObj(StickerPos stickerPos, StickerDone stickerDone);
   UniTask SpawnStickerDone(StickerPos stickerPos, Sticker sticker, bool moveToFreSpace = false);
   void ResetController();
}

[Serializable]
public class SpaceForSticker : ISpaceForSticker
{
   public Reactive<ObjHaveSticker> currentObjHaveSticker = new(null);
   public virtual bool RegisterStickerToFreeSpace()
   {
      return false;
   }

   public virtual bool RegisterSticker(Sticker sticker)
   {
      return false;
   }

   public virtual bool RegisterStickerDoneToObject(StickerDone stickerDone)
   {
      if (currentObjHaveSticker != null)
         if (currentObjHaveSticker.Value.IsSameSticker(stickerDone.stickerId, out var stickerPos))
         {
            _ = MoveStickerDoneToObj(stickerPos, stickerDone);
            return true;
         }

      return false;
   }

   public virtual async UniTask MoveStickerDoneToObj(StickerPos stickerPos, StickerDone stickerDone)
   {
      stickerDone.PlayAnimRemove();
      stickerPos.RegisterSticker(stickerDone);
      await UniTask.WaitForSeconds(1f);// wait anim Remove
      await stickerDone.PlayMoveAnim(stickerPos.trsStickerPos);
      stickerPos.MoveDone();
      var isLastSticker = currentObjHaveSticker.Value.IsCompleteSticker();
      if (isLastSticker)
      {
         GlobalEventManager.CheckToCallNextSticker?.Invoke();
      }
   }

   public async UniTask SpawnStickerDone(StickerPos stickerPos, Sticker sticker, bool moveToFreSpace = false)
   {
      var position = sticker.transform.position;
      position.z = -1;
      var e = PoolManager.Instance.SpawnStickerDone(stickerPos.trsStickerPos);
      e.InitStickerMove(sticker.stickerData.stickerID);
      stickerPos.RegisterSticker(e);
      e.transform.position = position;
      sticker.DisAbleIcon();
      e.PlayAnimRemove();
      await UniTask.WaitForSeconds(1f); // wait anim Remove
      if (moveToFreSpace)
      {
         await e.PlayMoveAnimToFreeSpace(stickerPos.trsStickerPos);
         stickerPos.MoveDone();
      }
      else
      {
         await e.PlayMoveAnim(stickerPos.trsStickerPos);
         stickerPos.MoveDone();
         var isLastSticker = currentObjHaveSticker.Value.IsCompleteSticker();
         if (isLastSticker)
         {
            GlobalEventManager.CheckToCallNextSticker?.Invoke();
         }
      }

   }

   public virtual void ResetController()
   {
      currentObjHaveSticker.Value = null;
   }
}