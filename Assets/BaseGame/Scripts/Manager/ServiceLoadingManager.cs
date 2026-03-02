using Cysharp.Threading.Tasks;
using TW.Utility.DesignPattern;
using UnityEngine;

public class ServiceLoadingManager : Singleton<ServiceLoadingManager>
{
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }

    public async UniTask LoadService()
    {
        await UniTask.WaitForSeconds(0.1f);
    }
}
