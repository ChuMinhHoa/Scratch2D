using Cysharp.Text;
using R3;
using TW.Utility.CustomType;
using UniRx;
using UnityEngine;

public class UIResourceEnergy : UIResource
{
    public Reactive<bool> energyInfinite;
    public override void Start()
    {
        base.Start();
        energyInfinite = EnergyManager.Instance.isOnInfiniteEnergy;
        energyInfinite.Subscribe(OnEnergyInfiniteChange).AddTo(this);
    }

    private void OnEnergyInfiniteChange(bool active)
    {
        if (active)
        {
            txtAmount.SetTextFormat(MyCache.strDefault, "<sprite=4>");
        }
    }

    public override void ChangeValue(BigNumber value)
    {
        var e = EnergyManager.Instance.isOnInfiniteEnergy;
        if (!e)
            txtAmount.SetTextFormat(MyCache.strDefault, value.ToStringUIFloor());
        var e1 = conditionActiveAddButton?.GetConditionActive() ?? false;
           // Debug.Log(e1);
        btnAdd.interactable = e1;
        objAdd.SetActive(e1);
    }
}
