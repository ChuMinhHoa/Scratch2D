using Cysharp.Threading.Tasks;
using LitMotion;
using UnityEngine;

public class StickerDone : MonoBehaviour
{
   public UnitAnimation unitAnim;
   public SpriteRenderer sprIcon;
   public Animation stickerDoneAnim;
   public SpriteRenderer stickerGlow;
   
   public int stickerId;
   
   public void InitStickerMove(int id)
   {
       stickerId = id;
       
       // sprIcon.Sprite =
       // stickerGlow.Sprite =
   }
   
   public async UniTask PlayMoveAnim(Transform targetPos)
   {
       CheckToAbleStickerAnimAgain();
       var currentScale = transform.localScale;
       var currentEulerAngle = transform.eulerAngles;
       LMotion.Create(currentScale, targetPos.localScale, .25f).Bind(x => transform.localScale = x);
       LMotion.Create(currentEulerAngle, targetPos.eulerAngles, .25f).Bind(x => transform.eulerAngles = x);
       await unitAnim.PlayMoveAnim(targetPos.position);
       stickerDoneAnim.Play("StickerAdd");
       await UniTask.WaitForSeconds(0.5f);
       stickerGlow.gameObject.SetActive(true);
   }

   public async UniTask PlayMoveAnimToFreeSpace(Transform targetPos)
   {
       stickerDoneAnim.enabled = false;
       LMotion.Create(sprIcon.transform.localPosition, Vector3.zero, .25f).Bind(x => sprIcon.transform.localPosition = x);
       await unitAnim.PlayMoveAnim(targetPos.position);
   }

   public void PlayAnimRemove()
   {
       CheckToAbleStickerAnimAgain();
       stickerDoneAnim.Play("StickerRemove");
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
}
