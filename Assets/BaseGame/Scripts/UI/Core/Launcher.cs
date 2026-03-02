using Core.UI.Screens;
using UnityEngine;
using ZBase.UnityScreenNavigator.Core;

public class Launcher : UnityScreenNavigatorLauncher
{
    protected override void Start()
    {
        base.Start();
        _ = UIManager.Instance.OpenActivityAsync<ActivityLoading>();
    }
}
