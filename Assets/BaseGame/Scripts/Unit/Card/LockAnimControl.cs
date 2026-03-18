using Spine.Unity;
using UnityEngine;

public class LockAnimControl : MonoBehaviour
{
    public SkeletonAnimation lockAnim;

    public GameObject objText;

    public void PlayAnimLock()
    {
        objText.SetActive(true);
        lockAnim.AnimationState.SetAnimation(0, MyCache.strLock, false);
    }

    public void PlayAnimUnlock()
    {
        objText.SetActive(false);
        lockAnim.AnimationState.SetAnimation(0,  MyCache.strUnlockNormal, false);
    }

    public void PlayAnimUnlockByBooster()
    {
        objText.SetActive(false);
        lockAnim.AnimationState.SetAnimation(0,  MyCache.strUnlockHammer, false);
    }

}
