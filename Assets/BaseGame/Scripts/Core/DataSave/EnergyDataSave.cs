using System;

[Serializable]
public class EnergyDataSave : IDataSave<EnergyDataSave>
{
    public static EnergyDataSave Instance => InGameDataManager.Instance.InGameData.EnergyDataSave;
    public bool IsDirty { get; set; }
    public EnergyDataSave DefaultData()
    {
        return this;
    }
    
    public int currentEnergy = -1;
    public Reactive<string> timeToAddEnergy = new("");
    public Reactive<string> timeToAddOneEnergy = new("");
}
