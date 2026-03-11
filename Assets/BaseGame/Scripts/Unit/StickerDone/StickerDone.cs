using System;
using Cysharp.Threading.Tasks;
using LitMotion;
using TW.Utility.DesignPattern.UniTaskState;
using UnityEngine;

public partial class StickerDone : MonoBehaviour
{
   public UnitAnimation unitAnim;
   public SpriteRenderer sprIcon;
   public Animation stickerDoneAnim;
   public SpriteRenderer stickerGlow;
   public StateMachine stateMachine = new();
   public int stickerId;

   private MotionHandle motionMoveHandle;
   private void Start()
   {
       stateMachine.Run();
       stateMachine.RequestTransition(StickerDoneInitState);
   }

   private void CheckToAbleStickerAnimAgain()
   {
       if (stickerDoneAnim.enabled == false)
       {
           stickerDoneAnim.enabled = true;
           stickerDoneAnim.Play("Idle");
       }
   }

   public void ResetStickerDone()
   {
       stickerGlow.gameObject.SetActive(false);
   }

   public void CheckMoveToFolder(bool fromNoWhere = false, bool fromFreeSpace = false)
   {
       var e = Level.Instance.oSController.GetFolderPos(this);
       if (e != null)
       {
           stickerPos?.ResetPos();
           stickerPos = e;
           StickerDoneManager.Instance.AddStickerDoneMoveToNote(this);
           Level.Instance.fSpaceController.RemoveStickerDoneFromNoWhere(this);
           Level.Instance.RemoveStickerDone(this);
           stateMachine.RequestTransition(StickerDoneMoveToObjHaveStickerState);
           return;
       }

       if (!fromFreeSpace)
       {
           e = Level.Instance.fSpaceController.GetFreeSpacePos(this);
           if (e != null)
           {
               Level.Instance.RemoveStickerDone(this);
               Level.Instance.fSpaceController.RemoveStickerDoneFromNoWhere(this);
               stickerPos = e;
               stateMachine.RequestTransition(StickerDoneMoveFreeSpaceState);
               return;
           }
       }

       if (!fromNoWhere && !fromFreeSpace)
            stateMachine.RequestTransition(StickerDoneMoveAround);
   }

   public bool IsHaveSticker(int noteId)
   {
       return noteId == stickerId;
   }
}
