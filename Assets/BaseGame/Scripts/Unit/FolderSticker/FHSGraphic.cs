using Cysharp.Threading.Tasks;
using LitMotion;
using Sirenix.OdinInspector;
using UnityEngine;

public class FHSGraphic : MonoBehaviour
{
    [SerializeField] private SpriteRenderer sprIcon;
    //[SerializeField] private Animator anim;
    [SerializeField] private CardFolderAnim[] cardFolderAnim;
    [SerializeField] private Transform trsFolder;
    [SerializeField] private Vector3 rotateFolderOpen;
    [SerializeField] private Vector3 posFolderOpen;
    [SerializeField] private Vector3 posFolderDefault;
    [SerializeField] private float timeDelayAnimOpen;
    
    [Button]
    public async UniTask OnOpen()
    {
        var currentRotate = trsFolder.eulerAngles;
        var currentPos = trsFolder.localPosition;
        LMotion.Create(currentRotate, rotateFolderOpen, 0.15f).Bind(x=> trsFolder.eulerAngles = x).AddTo(this);
        LMotion.Create(currentPos, posFolderOpen, 0.15f).WithDelay(timeDelayAnimOpen).Bind(x => trsFolder.localPosition = x).AddTo(this);
        for (var i = 0; i < cardFolderAnim.Length; i++)
        {
            _ = cardFolderAnim[i].AnimOpen();
        }

        await UniTask.WaitForSeconds(2f);
        //anim.Play("Open");
    }

    [Button]
    public async UniTask OnClose()
    {
        //anim.Play("Close");
        var currentRotate = NormalizeAngle(trsFolder.localEulerAngles.x);
        var currentPos = trsFolder.localPosition;
        LMotion.Create(currentPos, posFolderDefault, 0.15f).WithDelay(0.25f).Bind(x => trsFolder.localPosition = x).AddTo(this);
        for (var i = 0; i < cardFolderAnim.Length; i++)
        {
            _ = cardFolderAnim[i].AnimClose();
        }
        await LMotion.Create(currentRotate, 0, 0.15f).WithDelay(1.5f).Bind(x => trsFolder.eulerAngles = new Vector3(x, 0, 0)).AddTo(this);
    }
    
    private float NormalizeAngle(float angle)
    {
        if (angle > 180f)
            return angle - 360f;
        return angle;
    }

    public void OnIdle()
    {
        //anim.Play("Idle");
    }
}
