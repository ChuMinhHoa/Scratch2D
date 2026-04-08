using Cysharp.Threading.Tasks;
using SDK;
using UnityEngine;
using Sirenix.Utilities;
using TW.Utility.CustomType;

[CreateAssetMenu(fileName = "DefaultGlobalConfig", menuName = "GlobalConfigs/DefaultGlobalConfig")]
[GlobalConfig("Assets/Resources/GlobalConfig/")]
public class DefaultGlobalConfig : GlobalConfig<DefaultGlobalConfig>
{
    public int maxEnergy = 5;
    public float defaultMinutesForEnergy = 5; // 5 minutes for 1 energy
    public int defaultCoinWinGame = 10;
    public int priceRevive = 200;
    public int levelShowInter = 9;
    public int boosterUnlockAmount = 3;

    public async UniTask InitRemoteConfig()
    {
        await UniTask.WaitUntil(() => FirebaseManager.Instance.IsFirebaseRemoteFetchingSuccess);
        levelShowInter = (int)FirebaseManager.Instance.GetConfigDouble(Keys.key_remote_interstitial_level);
        defaultCoinWinGame = (int)FirebaseManager.Instance.GetConfigDouble(Keys.key_remote_coin_win_game);
    }
}