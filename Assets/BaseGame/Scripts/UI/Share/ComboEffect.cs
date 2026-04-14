using Cysharp.Text;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

public class ComboEffect : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI txtCombo;
    [SerializeField] private TextMeshProUGUI txtComboOutline;
    
    public void SetCombo(int combo)
    {
        txtCombo.SetTextFormat(MyCache.combo, combo);
        txtComboOutline.SetTextFormat(MyCache.combo, combo);

        _ = AnimCombo();
    }

    private async UniTask AnimCombo()
    {
        await UniTask.WaitForSeconds(0.75f);
        UIPoolManager.Instance.DeSpawnComboEffect(this);
    }
}
