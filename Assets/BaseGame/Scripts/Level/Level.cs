using System;
using System.Collections.Generic;
using Core.UI.Activities;
using CoreData;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using TW.Utility.DesignPattern;
using UnityEditor;
using UnityEngine;
using Random = System.Random;

public class Level : Singleton<Level>
{
    public Reactive<int> levelIndex = new(0);
    public TextAsset levelTextAsset;
    public LevelConfig levelConfig;
    public LevelData LevelData;

    public ObjHaveStickerController oSController;
    public FreeSpaceController fSpaceController;
    public LayerController layerController;

    public GameObject objOnUsingBooster;

    public List<StickerDone> stickerDone = new();

    public bool isEndGame;

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

        fSpaceController.stickerDoneWait.Clear();
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


        //_ = LoadData();
    }

    [Button]
    public async UniTask LoadData()
    {
        GamePlayManager.Instance.ChangeGameState(GameState.Loading);
        levelIndex = PlayerInfoManager.Instance.playerLevel;
        levelConfig = LevelGlobalConfig.Instance.GetLevelConfig(levelIndex.Value);
        levelTextAsset = levelConfig.levelAsset;
        LevelData = DataSerializer.Deserialize<LevelData>(levelTextAsset.text);
        ShuffleID();
        await oSController.LoadData(LevelData.objHaveStickers);
        await layerController.LoadData(LevelData.layerCards);
        await UniTask.WaitUntil(() => oSController.loadDone && layerController.loadDone);
        CallNextObjSticker(true);
        WaitToCheckCard().Forget();
    }

    private async UniTask WaitToCheckCard()
    {
        GamePlayManager.Instance.ChangeGameState(GameState.Playing);
        var totalCard = layerController.cards.Count;
        var totalTimeWait = 0.15f * totalCard + 0.25f;
        await UniTask.WaitForSeconds(totalTimeWait);
        GlobalEventManager.OnHaveCardDone?.Invoke();
    }

    private Dictionary<int, int> mappingID = new();

    private void ShuffleID()
    {
        mappingID.Clear();
        int totalStickerId = SpriteGlobalConfig.Instance.iconStickerBgConfigs.Length;
        var stickers = LevelData.objHaveStickers;

        // Assign mapped ids for objHaveStickers
        for (int i = 0; i < stickers.Length; i++)
        {
            int originalId = stickers[i].stickerId;
            if (!mappingID.TryGetValue(originalId, out int mappedId))
            {
                mappedId = GetRandomId(totalStickerId);
                mappingID[originalId] = mappedId;
            }

            stickers[i].stickerId = mappedId;
        }

        // Apply mapping to layer cards
        foreach (var layer in LevelData.layerCards)
        {
            foreach (var card in layer.cards)
            {
                for (int s = 0; s < card.stickers.Length; s++)
                {
                    int oldId = card.stickers[s].stickerID;
                    card.stickers[s].stickerID = mappingID.GetValueOrDefault(oldId, 0);
                }
            }
        }
    }

    private static int GetRandomId(int totalStickerId)
    {
        return new Random().Next(0, totalStickerId);
    }
#if UNITY_EDITOR
    public void LoadOnlyData()
    {
        var assetsPath = "Assets/BaseGame/TextAssets/LevelData/";
        var fileName = $"Level_{levelIndex}.txt";
        var assetPath = assetsPath + fileName;
        var levelDataTextAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(assetPath);
        LevelData = DataSerializer.Deserialize<LevelData>(levelDataTextAsset.text);
    }
#endif

    [Button]
    private void CallNextObjSticker(bool callFromLoad = false)
    {
        _ = oSController.CallNextObjSticker(callFromLoad);
    }

    public void RegisterStickerDone(Sticker sticker, Vector3 rot)
    {
        var stD = PoolManager.Instance.SpawnStickerDone(sticker.transform);
        stD.InitStickerMove(sticker.stickerData.stickerID, rot);
        sticker.DisAbleIcon();
        stickerDone.Add(stD);
    }

    [Button]
    public void ResetLevel()
    {
        LoadDataClean();
        Debug.Log("Reset Level");
        isEndGame = false;
        StickerDoneManager.Instance.Clear();
        oSController.ResetController();
        fSpaceController.ResetController();
        layerController.ResetController();

        for (var i = 0; i < stickerDone.Count; i++)
        {
            stickerDone[i].ResetStickerDone();
            PoolManager.Instance.DespawnStickerMove(stickerDone[i]);
        }
        stickerDone.Clear();
    }

    public void LevelUp()
    {
        ResetLevel();
        levelIndex.Value++;
        PlayerInfoDataSave.Instance.SaveData();
        _ = UIManager.Instance.CloseScreenAsync();
    }

    public void MoveFolderOut(FolderHaveSticker folder)
    {
        oSController.MoveFolderOut(folder);
    }

    public bool IsHaveStickerWait()
    {
        return fSpaceController.IsHaveStickerWait();
    }

    public void CheckStickerDone()
    {
        _ = fSpaceController.CheckStickerDone();
    }

    public void CheckLoseGame()
    {
        if (!fSpaceController.IsHaveStickerWait()) return;
        
        var isHaveAllNoteOnSlot = oSController.IsHaveAllNoteOnSlot();

        //Nếu tất cả các ô trống đều có note tức là thua hoặc
        // Cần kiểm tra xem có Card nào chứa sticker chưa bóc mà trùng với Note không?
        // Nếu có thì vẫn còn cơ hội để chơi tiếp => sửa thành thua luôn
        Debug.Log($"Have all note on slot: {isHaveAllNoteOnSlot}");
        if (isHaveAllNoteOnSlot)
        {
            //var e = CheckAllCardOnLayerHaveStickerSameIdWithNote();
            var e1 = CheckAllStickerDone();
            Debug.Log($"note Have Sticker Done: {e1}");
            if (/*!e && */!e1)
            {
                Debug.Log("call end game form here");
                //Time.timeScale = 0f;
                _ = EndGame();
                return;
            }
        }

        //Nếu có ít nhất 1 note trên slot thì vẫn còn cơ hội để thắng
        //Nếu có note nào đang chuẩn bị vào thì vẫn có thể chơi tiếp

        var isHaveAtLeastOne = oSController.IsHaveAtLeastOneNote();
        if (!isHaveAtLeastOne)
        {
            Debug.Log("call end game form here");
            _ = EndGame();
        }
    }

    private bool CheckAllCardOnLayerHaveStickerSameIdWithNote()
    {
        var slotFolders = oSController.SlotFolders;
        var card = layerController.cards;
        for (var i = 0; i < slotFolders.Length; i++)
        {
            var noteId = slotFolders[i].GetNoteId();
            Debug.Log($"check note id: {noteId}");
            if (noteId != -1)
            {
                for (var j = 0; j < card.Count; j++)
                {
                    Debug.Log("Need Check");
                    //if (!card[j].CheckIsSameLayer()) continue;
                    if (card[j].IsHaveSticker(noteId))
                        return true;
                }
            }
        }

        return false;
    }

    private bool CheckAllStickerDone()
    {
        var slotFolders = oSController.SlotFolders;
        for (var i = 0; i < slotFolders.Length; i++)
        {
            var noteId = slotFolders[i].GetNoteId();
            if (noteId != -1)
            {
                for (var j = 0; j < stickerDone.Count; j++)
                {
                    if (stickerDone[j].IsHaveSticker(noteId))
                    {
                        Debug.Log(noteId + $" is have sticker done {stickerDone[j]} {i}");
                        return true;
                    }
                }
            }
        }

        return false;
    }

    private async UniTask EndGame()
    {
        await UniTask.WaitForFixedUpdate();
        if (isEndGame)
            return;
        isEndGame = true;
        Debug.Log("game over");
        GamePlayManager.Instance.ChangeGameState(GameState.Normal);
        await UniTask.WaitForSeconds(1f);
        await UIManager.Instance.OpenActivityAsync<ActivityLoseGame>();
    }

    public void RemoveSticker(StickerDone stD)
    {
        stickerDone.Remove(stD);
    }

    public void AddSlot()
    {
        fSpaceController.AddSlot();
    }
}