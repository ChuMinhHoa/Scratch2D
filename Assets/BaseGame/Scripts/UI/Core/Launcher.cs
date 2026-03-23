using Core.UI.Screens;
using Cysharp.Threading.Tasks;
using UnityEngine;
using ZBase.UnityScreenNavigator.Core;

public class Launcher : UnityScreenNavigatorLauncher
{
    [SerializeField] private GameObject objFakeLoading;
    
    protected override void Start()
    {
        base.Start();
        OpenFirstLoading().Forget();
        GlobalEventManager.OnShowWarning += ShowWarning;
    }

    private void ShowWarning(string des)
    {
        var e = UIPoolManager.Instance.SpawnWarningElement(transform);
        e.SetText(des);
        _ = e.PlayAnim();
    }

    private async UniTask OpenFirstLoading()
    {
        await UIManager.Instance.OpenActivityAsync<ActivityLoading>();
        objFakeLoading.SetActive(false);
    }
}
