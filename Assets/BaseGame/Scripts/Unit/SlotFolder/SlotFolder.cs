using System;
using SDK;
using TW.Utility.DesignPattern.UniTaskState;
using UnityEngine;

public partial class SlotFolder : MonoBehaviour
{
    public Reactive<SlotFolderType> slotFolderType;
    public FolderPos folderPos;
    public StateMachine stateMachine;
    public SlotFolderGraphic slotFolderGraphic;
    public ButtonGameObject btnWatchAds;
    public ButtonGameObject btnBuy;
    public GameObject objEffectSpawn;
    public GameObject prefabEffectSpawn;
    private void Start()
    {
        stateMachine.RequestTransition(SlotFolderInitState);
        stateMachine.Run();

        btnWatchAds.AddListeners(CallWatchAds);
        btnBuy.AddListeners(CallBuy);
    }

    private void CallBuy()
    {
        ChangeFolderType(SlotFolderType.Normal);
    }

    private void CallWatchAds()
    {
#if UNITY_EDITOR
        ChangeFolderType(SlotFolderType.Normal);
#endif
        //AdsManager.Instance.ShowRewardVideo(PlacementType.InGame.ToString(), "ads_reward_slot_folder", () => ChangeFolderType(SlotFolderType.Normal));
#if !UNITY_EDITOR
        if (ShopManager.Instance.NoAds.Value) ChangeFolderType(SlotFolderType.Normal);
        else
        {
            Debug.Log("Show Ads Reward");
            IngameFirebaseAnalystic.Instance.SetAdsRewardInfo("ads_reward_slot_folder", 1);
            AdsManager.Instance.ShowRewardVideo(PlacementType.InGame.ToString(), "ads_reward_slot_folder", () => ChangeFolderType(SlotFolderType.Normal));
        }
#endif
    }

    public bool IsHaveObject()
    {
        if (slotFolderType.Value == SlotFolderType.Ads)
            return true;
        return folderPos.IsHaveObj();
    }

    public void SetFolder(FolderHaveSticker folder)
    {
        folderPos.RegisterObj(folder);
    }

    public void ChangeFolderType(SlotFolderType folderType)
    {
        slotFolderType.Value = folderType;
        if (slotFolderType.Value == SlotFolderType.Normal)
        {
            if (!objEffectSpawn)
            {
                objEffectSpawn = Instantiate(prefabEffectSpawn, transform);
                objEffectSpawn.transform.localPosition = Vector3.zero;
            }

            objEffectSpawn.SetActive(true);
            SoundManager.Instance.PlaySoundSfx(AudioKey.Sfx_BoosterAddSlot);
            GlobalEventManager.CheckToCallNextSticker?.Invoke();
        }

        stateMachine.RequestTransition(SlotFolderInitState);
        stateMachine.Run();
    }

    public bool IsAbleFolder()
    {
        return slotFolderType.Value == SlotFolderType.Normal && !IsHaveObject();
    }

    public void ResetSlotFolder()
    {
        folderPos.ResetPos();
    }

    public void ResetByLevel()
    {
        var e = folderPos.obj;
        if (e != null)
        {
            e.ResetFolderSticker();
            PoolManager.Instance.DespawnObjHaveSticker(e);
        }

        folderPos.ResetPos();
    }

    public int GetNoteId()
    {
        if (!IsHaveObject()) return -1;
        if (slotFolderType.Value != SlotFolderType.Normal) return -1;
        return folderPos.obj.data.stickerId;
    }
}

public enum SlotFolderType
{
    Normal,
    Coin,
    Ads,
}