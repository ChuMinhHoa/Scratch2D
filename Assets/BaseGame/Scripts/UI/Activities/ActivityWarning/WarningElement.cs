using Cysharp.Text;
using Cysharp.Threading.Tasks;
using LitMotion;
using TMPro;
using UnityEngine;

public class WarningElement : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI txtDes;
    [SerializeField] private Transform trsAnim;
    [SerializeField] private Vector3 offSetPos;
    [SerializeField] private CanvasGroup canvasGroup;
    
    public void SetText(string des)
    {
        txtDes.SetTextFormat(MyCache.strDefault, des);
        transform.localPosition = Vector3.zero - offSetPos;
    }

    public async UniTask PlayAnim()
    {
        var currentPos = transform.localPosition;

        LMotion.Create(0f, 1f, 0.25f).Bind(x => canvasGroup.alpha = x).AddTo(this);
        
        LMotion.Create(currentPos, Vector3.zero, 0.25f).Bind(x=>
        {
            transform.localPosition = x;
        }).AddTo(this);

        await UniTask.WaitForSeconds(1.5f);
        LMotion.Create(1f, 0f, 0.25f).WithOnComplete(CompleteAnim).Bind(x => canvasGroup.alpha = x).AddTo(this);
    }

    private void CompleteAnim()
    {
        UIPoolManager.Instance.DeSpawnWarningElement(this);
    }
}
