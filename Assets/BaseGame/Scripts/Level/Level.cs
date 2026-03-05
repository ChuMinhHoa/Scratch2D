using System;
using CoreData;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

public class Level : MonoBehaviour
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
        if (RegisterStickerToObj(sticker))
        {
            return;
        }
        if (RegisterStickerToFreeSpace(sticker)) return;

        _ = GamePlayManager.Instance.level.fSpaceController.SpawnStickerDoneNotMove(sticker);
    }

    private bool IsHaveFolderOnMove(out FolderPos folder)
    {
        return oSController.IsHaveFolderOnMove(out folder);
    }

    private bool RegisterStickerToObj(Sticker sticker)
    {
        return oSController.RegisterSticker(sticker);
    }
    
    private bool RegisterStickerToFreeSpace(Sticker sticker)
    {
        return fSpaceController.RegisterSticker(sticker);
    }

    public void CheckAllStickerOnFreeSpace()
    {
        fSpaceController.CheckAllStickerOnFreeSpace();
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
}