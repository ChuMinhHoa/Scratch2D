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

   public void CheckMoveToFolder(bool fromNoWhere = false)
   {
       var e = Level.Instance.oSController.GetFolderPos(stickerId);
       if (e != null)
       {
           stickerPos?.ResetPos();
           stickerPos = e;
           Level.Instance.fSpaceController.RemoveStickerDoneFromNoWhere(this);
           stateMachine.RequestTransition(StickerDoneMoveToObjHaveStickerState);
           return;
       }

       if (!fromNoWhere) return;
       e = Level.Instance.fSpaceController.GetFreeSpacePos();
       if (e != null)
       {
           Level.Instance.fSpaceController.RemoveStickerDoneFromNoWhere(this);
           stickerPos = e;
           stateMachine.RequestTransition(StickerDoneMoveFreeSpaceState);
       }
   }

   public bool IsHaveSticker(int noteId)
   {
       return noteId == stickerId;
   }
}
