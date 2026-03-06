using System;
using CoreData;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using TW.Utility.DesignPattern;
using UnityEditor;
using UnityEngine;

public class Level : Singleton<Level>
{
    public int levelIndex;
    public LevelData LevelData;
    
    public ObjHaveStickerController oSController;
    public FreeSpaceController fSpaceController;
    public LayerController layerController;

    public GameObject objOnUsingBooster;
    
    private void Start()
    {
        GamePlayManager.Instance.level = this;
        GlobalEventManager.CheckToCallNextSticker = () => CallNextObjSticker();

        GlobalEventManager.OnRemoveSticker = OnRemoveSticker;

        GlobalEventManager.OnBoosterUsing += OnUsingBooster;
        GlobalEventManager.OnBoosterDone += OnBoosterDone;
    }

    private void OnBoosterDone()
    {
        objOnUsingBooster.gameObject.SetActive(false);
    }

    private void OnUsingBooster(BoosterType arg1, IBooster arg2)
    {
        objOnUsingBooster.gameObject.SetActive(true);
    }

    private void OnRemoveSticker(int stickerId, int countRemove)
    {
        layerController.OnRemoveSticker(stickerId, countRemove);
    }

    [Button(ButtonSizes.Gigantic)]
    private void LoadDataClean()
    {

        var e = FindObjectsByType<ScratchObject>(FindObjectsSortMode.None);
        var e1 = FindObjectsByType<Card>(FindObjectsSortMode.None);
        var e2 = FindObjectsByType<Sticker>(FindObjectsSortMode.None);
        var e3 = FindObjectsByType<StickerDone>(FindObjectsSortMode.None);
        var e4 = FindObjectsByType<FolderHaveSticker>(FindObjectsSortMode.None);
        var e5 = FindObjectsByType<SlotFolder>(FindObjectsSortMode.None);
        var e6 = FindObjectsByType<SpaceSticker>(FindObjectsSortMode.None);
        
        for (var i = 0; i < e.Length; i++)
        {
            Destroy(e[i].gameObject);
        }
        for (var i = 0; i < e1.Length; i++)
        {
            Destroy(e1[i].gameObject);
        }
        for (var i = 0; i < e2.Length; i++)
        {
            Destroy(e2[i].gameObject);
        }
        for (var i = 0; i < e3.Length; i++)
        {
            Destroy(e3[i].gameObject);
        }
        for (var i = 0; i < e4.Length; i++)
        {
            Destroy(e4[i].gameObject);
        }
        
        fSpaceController.stickerCantMoveAnyWhere.Clear();
        oSController.objHaveStickers.Clear();
        layerController.cards.Clear();

        for (var i = 0; i < e5.Length; i++)
        {
            e5[i].ResetSlotFolder();
        }
        
        for (var i = 0; i < e6.Length; i++)
        {
            e6[i].ResetSpace();
        }
        
        
        _ = LoadData();
    }

    [Button]
    public async UniTask LoadData()
    {
        var assetsPath = "Assets/BaseGame/TextAssets/LevelData/";
        var fileName = $"Level_{levelIndex}.txt";
        var assetPath = assetsPath + fileName;
        var levelDataTextAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(assetPath);
        LevelData = DataSerializer.Deserialize<LevelData>(levelDataTextAsset.text);
        oSController.LoadData(LevelData.objHaveStickers);
        layerController.LoadData(LevelData.layerCards.AsSpan());
        await UniTask.WaitUntil(() => oSController.loadDone && layerController.loadDone);
        CallNextObjSticker(true);
    }

    public void LoadOnlyData()
    {
        var assetsPath = "Assets/BaseGame/TextAssets/LevelData/";
        var fileName = $"Level_{levelIndex}.txt";
        var assetPath = assetsPath + fileName;
        var levelDataTextAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(assetPath);
        LevelData = DataSerializer.Deserialize<LevelData>(levelDataTextAsset.text);
    }


    [Button]
    private void CallNextObjSticker(bool callFromLoad = false)
    {
        _ = oSController.CallNextObjSticker(callFromLoad);
    }
    
    public void RegisterStickerDone(Sticker sticker)
    {
        var stickerDone = PoolManager.Instance.SpawnStickerDone(sticker.transform);
        stickerDone.InitStickerMove(sticker.stickerData.stickerID);
        sticker.DisAbleIcon();
    }

    public void NextLayer()
    {
        layerController.NextLayer();
    }
    
    [Button]
    public void ResetLevel()
    {
        Debug.Log("Reset Level");
        oSController.ResetController();
        fSpaceController.ResetController();
        layerController.ResetController();
        levelIndex++;
        _ = LoadData();
    }

    public void MoveFolderOut(FolderHaveSticker folder)
    {
       oSController.MoveFolderOut(folder);
    }

    public void GameOver()
    {
        Debug.Log("game over");
    }

    public bool IsHaveStickerWait()
    {
        return fSpaceController.IsHaveStickerWait();
    }

    public void CheckStickerDone()
    {
        _ = fSpaceController.CheckStickerDone();
    }
}