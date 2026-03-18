using Cysharp.Threading.Tasks;
using LitMotion;
using Spine.Unity;
using UnityEngine;

public class BoosterGraphicControl : MonoBehaviour
{
    public Transform trsBooster;
    public SkeletonAnimation skAnimBooster;
    public float timeAnim;
    public BoosterType bType;

    public async UniTask MoveBoosterTo(Transform target)
    {
        var currentPos = trsBooster.position;
        var targetPos = target.position;
        await LMotion.Create(currentPos, targetPos, 0.15f).Bind(x => trsBooster.position = x).AddTo(trsBooster);
        PlayAnim().Forget();
    }

    private async UniTask PlayAnim()
    {
        skAnimBooster.AnimationState.SetAnimation(0, MyCache.strActive, false);
        await UniTask.WaitForSeconds(timeAnim);
        GlobalEventManager.OnBoosterDone?.Invoke();
    }
}