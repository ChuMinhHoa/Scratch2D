using System.Collections.Generic;
using UnityEngine;

public class TutorialDataSave : IDataSave<TutorialDataSave>
{
    public static TutorialDataSave Instance => InGameDataManager.Instance.InGameData.TutorialDataSave;
    public bool IsDirty { get; set; }
    public TutorialDataSave DefaultData()
    {
        return this;
    }
    
    public List<int> tutorialIDComplete = new();
    public List<BoosterType> boosterUnlock = new();
}
