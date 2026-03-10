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
    }

    private async UniTask OpenFirstLoading()
    {
        await UIManager.Instance.OpenActivityAsync<ActivityLoading>();
        objFakeLoading.SetActive(false);
    }
}
