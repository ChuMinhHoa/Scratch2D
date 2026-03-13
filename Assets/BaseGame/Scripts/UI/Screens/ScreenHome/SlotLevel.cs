using Cysharp.Text;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SlotLevel : SlotBase<int>
{
    [Title("Slot Level")]
    [SerializeField] private Image imgBg;
    [SerializeField] private TextMeshProUGUI txtLevel;

    public override void InitData(int data)
    {
        base.InitData(data);
        txtLevel.SetTextFormat(MyCache.strDefault, data);
    }
}
