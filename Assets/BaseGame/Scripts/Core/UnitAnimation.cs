using Cysharp.Threading.Tasks;
using LitMotion;
using Sirenix.OdinInspector;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class UnitAnimation : MonoBehaviour
{
    #region Scale Animation

    public AnimationCurve curveScaleX;
    public AnimationCurve curveScaleY;
    public AnimationCurve curveScaleZ;
    public float timeScale = 1f;
    private MotionHandle motionHandleScale;
    
    [Button]
    public async UniTask PlayScaleAnimation()
    {
        if (motionHandleScale.IsPlaying())
            motionHandleScale.TryCancel();
        
        motionHandleScale = LMotion.Create(0f, 1f, timeScale).Bind(x =>
        {
            var scaleX = curveScaleX.Evaluate(x);
            var scaleY = curveScaleY.Evaluate(x);
            var scaleZ = curveScaleZ.Evaluate(x);
            transform.localScale = new Vector3(scaleX, scaleY, scaleZ);
        }).AddTo(this);
        await motionHandleScale;
    }

    #endregion

    #region Move Animation
    
    public AnimationCurve curveMoveX;
    public AnimationCurve curveMoveY;
    public AnimationCurve curveMoveZ;
    public float timeMove = 1f;
    private MotionHandle motionHandleMove;
    
    public float2 magnitudeX;
    public float2 magnitudeY;
    public float2 magnitudeZ;

    [Button]
    public async UniTask PlayMoveAnim(Vector3 targetPos)
    {
        var currentX = transform.position.x;
        var currentY = transform.position.y;
        var currentZ = transform.position.z;
      
        var targetX = targetPos.x;
        var targetY = targetPos.y;
        var targetZ = targetPos.z;
        
        var mx = Random.Range(magnitudeX.x, magnitudeX.y);
        var my = Random.Range(magnitudeY.x, magnitudeY.y);
        var mz = Random.Range(magnitudeZ.x, magnitudeZ.y);
      
        var currentPos = transform.position;
        Debug.Log(currentPos);

        LMotion.Create(0f, 1f, timeMove).Bind(t =>
        {
            var x = Mathf.Lerp(currentX, targetX, t);
            var evaluateX = curveMoveX.Evaluate(t) * mx;
            currentPos.x = x + evaluateX;
            transform.position = currentPos;
        }).AddTo(this);
        
        LMotion.Create(0f, 1f, timeMove).Bind(t =>
        {
            var y = Mathf.Lerp(currentY, targetY, t);
            var evaluateY = curveMoveY.Evaluate(t) * my;
            currentPos.y = y + evaluateY;
            transform.position = currentPos;
        }).AddTo(this);
      
        await LMotion.Create(0f, 1f, timeMove).Bind(t =>
        {
            var z = Mathf.Lerp(currentZ, targetZ, t);
            var evaluateZ = curveMoveZ.Evaluate(t) * mz;
            currentPos.z = z + evaluateZ;
            transform.position = currentPos;
        }).AddTo(this);
    }

    #endregion

}
