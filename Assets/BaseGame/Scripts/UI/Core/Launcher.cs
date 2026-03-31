using Core.UI.Modals;
using Core.UI.Screens;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using TW.UGUI.Core.Activities;
using TW.UGUI.Core.Modals;
using TW.UGUI.Core.Screens;
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
        GlobalEventManager.HideUI += HideUI;
    }

    private bool activeUI = true;
    private void HideUI()
    {
        activeUI =! activeUI;
        var e1 = ActivityContainer.Find(ContainerKey.ActivitiesInGame);
        if (e1 != null)
        {
            e1.gameObject.SetActive(activeUI);
        }
        
        var e4 = ActivityContainer.Find(ContainerKey.Activities);
        if (e4 != null)
        {
            e4.gameObject.SetActive(activeUI);
        }

        var e2 = ScreenContainer.Find(ContainerKey.Screens);
        if (e2 != null)
        {
            e2.gameObject.SetActive(activeUI);
        }
        
        var e3 = ScreenContainer.Find(ContainerKey.ScreenDefault);
        if (e3 != null)
        {
            e3.gameObject.SetActive(activeUI);
        }
        
        var e5 = ModalContainer.Find(ContainerKey.Modals);
        if (e5 != null)
        {
            e5.gameObject.SetActive(activeUI);
        }
    }

    private void ShowWarning(string des)
    {
        Debug.Log(des);
        var e1 = ActivityContainer.Find(ContainerKey.ActivitiesInGame);
        var e = UIPoolManager.Instance.SpawnWarningElement(e1.transform);
        e.SetText(des);
        _ = e.PlayAnim();
    }

    private async UniTask OpenFirstLoading()
    {
        await UIManager.Instance.OpenActivityAsync<ActivityLoading>();
        objFakeLoading.SetActive(false);
    }

    [Button]
    private void OpenModalRevive()
    {
        _ = UIManager.Instance.OpenModalAsync<ModalRevive>();
    }
}