using System;
using System.Collections.Generic;
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
        fSpaceController.SetOSController(oSController.currentObjHaveSticker);
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
        oSController.LoadData(LevelData.objHaveStickers.AsSpan());
        layerController.LoadData(LevelData.layerCards.AsSpan());
        //await UniTask.WaitUntil(() => oSController.loadDone && layerController.loadDone);
        _ = CallNextObjSticker();
    }

    [Button]
    private async UniTask CallNextObjSticker()
    {
        await oSController.CallNextObjSticker();
        if (oSController.currentObjHaveSticker.Value)
        {
            CheckAllStickerOnFreeSpace();
        }
    }

    public void RegisterStickerDone(Sticker sticker)
    {
        if (RegisterStickerToObj(sticker))
        {
            return;
        }
        if (RegisterStickerToFreeSpace(sticker)) return;
        //CheckGameOver();
    }

    private bool RegisterStickerToObj(Sticker sticker)
    {
        return oSController.RegisterSticker(sticker);
    }
    
    private bool RegisterStickerToFreeSpace(Sticker sticker)
    {
        return fSpaceController.RegisterSticker(sticker);
    }

    private void CheckAllStickerOnFreeSpace()
    {
        fSpaceController.CheckAllStickerOnFreeSpace();
    }

    public void NextLayer()
    {
        layerController.NextLayer();
    }
    
    public void ResetLevel()
    {
        oSController.ResetController();
        fSpaceController.ResetController();
        layerController.ResetController();
        LoadData();
    }
}