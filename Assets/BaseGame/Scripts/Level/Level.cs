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
    
    private void Start()
    {
        GamePlayManager.Instance.level = this;
        GlobalEventManager.CheckToCallNextSticker = () => _ = CallNextObjSticker();
    }

    [Button]
    private void LoadData()
    {
        var assetsPath = "Assets/BaseGame/TextAssets/LevelData/";
        var fileName = $"Level_{levelIndex}.txt";
        var assetPath = assetsPath + fileName;
        var levelDataTextAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(assetPath);
        LevelData = DataSerializer.Deserialize<LevelData>(levelDataTextAsset.text);
        oSController.LoadData(LevelData.objHaveStickers);
        layerController.LoadData(LevelData.layerCards.AsSpan());
        //await UniTask.WaitUntil(() => oSController.loadDone && layerController.loadDone);
        _ = CallNextObjSticker(true);
    }
    

    [Button]
    private async UniTask CallNextObjSticker(bool callFromLoad = false)
    {
        await oSController.CallNextObjSticker(callFromLoad);
        
    }

    public async UniTask RegisterStickerDone(Sticker sticker)
    {
        if (IsHaveFolderOnMove(out var folderPos))
        {
            await UniTask.WaitUntil(() => folderPos.moveDone);
        }
        if (RegisterStickerToObj(sticker))
        {
            return;
        }
        if (RegisterStickerToFreeSpace(sticker)) return;
        //CheckGameOver();
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
        //LoadData();
    }

    public void MoveFolderOut(FolderHaveSticker folder)
    {
       oSController.MoveFolderOut(folder);
    }
}