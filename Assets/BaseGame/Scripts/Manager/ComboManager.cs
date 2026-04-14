using Sirenix.OdinInspector;
using TW.UGUI.Core.Activities;
using TW.Utility.DesignPattern;
using UnityEngine;

public class ComboManager : Singleton<ComboManager>
{
    [SerializeField] private int currentCombo;
    [SerializeField] private int maxCombo;
    [SerializeField] private float timeCombo;
    [SerializeField] private float[] timeCooldownCombo;

    [Button]
    private void InitTimeCooldownCombo(float timeFirst)
    {
        timeCooldownCombo = new float[maxCombo];
        var timeDecrease = timeFirst / (maxCombo-1);
        for (var i = 0; i < maxCombo; i++)
        {
            timeCooldownCombo[i] = timeFirst - timeDecrease * i;
        }
    }

    private void Start()
    {
        GlobalEventManager.OnShowComboEffect += ShowComboEffect;
        
    }

    private void TimeChange()
    {
        if (!(timeCombo > 0)) return;
        
        timeCombo -= Time.deltaTime;

        if (timeCombo <= 0)
            ResetComo();
    }

    private void ResetComo()
    {
        currentCombo = 0;
    }

    private void OnDestroy()
    {
        GlobalEventManager.OnShowComboEffect -= ShowComboEffect;
       
    }
    
    private void StopCoolDownCombo()
    {
        TimeManager.OnTimeChange -= TimeChange;
    }

    private void ShowComboEffect(Transform noteTrs)
    {
        currentCombo++;
        if (currentCombo <= 1)
            return;
        StopCoolDownCombo();
        
        currentCombo = Mathf.Clamp(currentCombo, 0, maxCombo);
        
        var pos = CameraManager.Instance.WorldToScreenPoint(noteTrs.position);
        var e1 = ActivityContainer.Find(ContainerKey.ActivitiesInGame);
        var e = UIPoolManager.Instance.SpawnComboEffect(e1.transform);
        e.transform.position = pos;
        e.SetCombo(currentCombo);
        timeCombo = timeCooldownCombo[currentCombo - 2];
        
        TimeManager.OnTimeChange += TimeChange;
    }
}
