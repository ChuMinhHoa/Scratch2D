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
        await PlayAnim();
    }

    public async UniTask  MoveBoosterTo(Vector3 target)
    {
        trsBooster.position = target;
        await PlayAnimActive();
    }

    private async UniTask PlayAnimActive()
    {
        trsBooster.gameObject.SetActive(true);
        await UniTask.WaitForSeconds(timeAnim);
        trsBooster.gameObject.SetActive(false);
    }

    private async UniTask PlayAnim()
    {
        skAnimBooster.AnimationState.SetAnimation(0, MyCache.strActive, false);
        await UniTask.WaitForSeconds(timeAnim);
        GlobalEventManager.OnBoosterDone?.Invoke();
    }
}