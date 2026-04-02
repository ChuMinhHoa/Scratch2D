using System;
using System.Collections.Generic;
using System.Linq;
using Core.UI.Activities;
using Core.UI.Modals;
using Core.UI.Screens;
using CoreData;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using TW.Utility.DesignPattern;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class Level : Singleton<Level>
{
    public Reactive<int> levelIndex = new(0);
    public Reactive<int> levelChange = new(-1);
    public int realLevel;
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
        InitData().Forget();
    }

    private async UniTask InitData()
    {
        await UniTask.WaitUntil(() => PlayerInfoManager.Instance.loadDone);

        levelIndex = PlayerInfoManager.Instance.playerLevel;
        levelChange = PlayerInfoManager.Instance.levelChange;

        GlobalEventManager.CheckToCallNextSticker = () => CallNextObjSticker();

        GlobalEventManager.OnRemoveSticker = OnRemoveSticker;

        GlobalEventManager.OnBoosterUsing += OnUsingBooster;
        GlobalEventManager.OnBoosterDone += OnBoosterDone;
    }

    private void OnDestroy()
    {
        GlobalEventManager.OnRemoveSticker -= OnRemoveSticker;

        GlobalEventManager.OnBoosterUsing -= OnUsingBooster;
        GlobalEventManager.OnBoosterDone -= OnBoosterDone;
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

    /// <summary>
    /// CODE NHƯ CỨT. LÀM VỘI NÊN MỚI PHẢI CHỐNG CHẾ THẾ NÀY
    /// </summary>
    [Button(ButtonSizes.Gigantic)]
    public void LoadDataClean()
    {
        //Debug.Log("Clean Data");
        var e = FindObjectsByType<ScratchObject>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        var e1 = FindObjectsByType<Card>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        var e2 = FindObjectsByType<Sticker>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        var e3 = FindObjectsByType<StickerDone>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        var e4 = FindObjectsByType<FolderHaveSticker>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        var e5 = FindObjectsByType<SlotFolder>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        var e6 = FindObjectsByType<SpaceSticker>(FindObjectsInactive.Include, FindObjectsSortMode.None);

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

        oSController.SlotFolders[^1].ChangeFolderType(SlotFolderType.Ads);
    }

    [Button]
    public async UniTask LoadData()
    {
        GamePlayManager.Instance.ChangeGameState(GameState.Loading);
        var e = levelIndex.Value >= LevelGlobalConfig.Instance.levelConfigs.Length && levelChange.Value == -1;
        if (e)
        {
            levelChange.Value = LevelGlobalConfig.Instance.GetRandomLevel(levelIndex.Value);
            PlayerInfoDataSave.Instance.SaveData();
        }

        realLevel = levelChange.Value != -1 ? levelChange.Value : levelIndex.Value;

        levelConfig = LevelGlobalConfig.Instance.GetLevelConfig(realLevel);

        levelTextAsset = levelConfig.levelAsset;
        LevelData = DataSerializer.Deserialize<LevelData>(levelTextAsset.text);
        ShuffleID();
        oSController.LoadData(LevelData.objHaveStickers);
        await layerController.LoadData(LevelData.layerCards);
        await UniTask.WaitUntil(() => oSController.loadDone && layerController.loadDone);

        IngameFirebaseAnalystic.Instance.StartTimePlayLevel();
        IngameFirebaseAnalystic.Instance.SetLevel(levelIndex.Value);
        IngameFirebaseAnalystic.Instance.TrackLevelStart();
    }

    [Button]
    public async UniTask AnimFirstSpawn()
    {
        //Debug.Log("Anim First Spawn");
        layerController.AnimFirstSpawn();
        CallNextObjSticker(true);
        var totalTimeWait = 0.75f;
        await UniTask.WaitForSeconds(totalTimeWait);
        await UIManager.Instance.OpenActivityAsync<ActivityFirstShowOnGamePlay>();
        await UniTask.WaitForSeconds(1.5f);
        GamePlayManager.Instance.ChangeGameState(GameState.Playing);
        GlobalEventManager.OnHaveCardDone?.Invoke();
    }

    private Dictionary<int, int> mappingID = new();

    private void ShuffleID()
    {
        mappingID.Clear();
        var totalStickerId = SpriteGlobalConfig.Instance.iconStickerBgConfigs.Length;
        CreateNewCanUse(totalStickerId);
        var stickers = LevelData.objHaveStickers;

        // Assign mapped ids for objHaveStickers
        for (var i = 0; i < stickers.Length; i++)
        {
            var originalId = stickers[i].stickerId;
            if (!mappingID.TryGetValue(originalId, out var mappedId))
            {
                mappedId = GetRandomId(originalId);
                mappingID[originalId] = mappedId;
            }

            stickers[i].stickerId = mappedId;
        }

        // Apply mapping to layer cards
        foreach (var layer in LevelData.layerCards)
        {
            foreach (var card in layer.cards)
            {
                foreach (var t in card.stickers)
                {
                    var oldId = t.stickerID;
                    t.stickerID = mappingID.GetValueOrDefault(oldId, 0);
                }
            }
        }
    }

    private List<int> canUse = new();

    private void CreateNewCanUse(int totalStickerId)
    {
        canUse.Clear();
        for (var i = 0; i < totalStickerId; i++)
        {
            canUse.Add(i);
        }
    }

    private int GetRandomId(int idIgnore)
    {
        canUse.Remove(idIgnore);

        var randomIndex = Random.Range(0, canUse.Count);
        var idReturn = canUse[randomIndex];

        canUse.Remove(idReturn);
        canUse.Add(idIgnore);

        return idReturn;
    }

#if UNITY_EDITOR
    public void LoadOnlyData()
    {
        var assetsPath = "Assets/BaseGame/TextAssets/LevelData/";
        var fileName = $"Level_{levelIndex.Value}.txt";
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

    public void RegisterStickerDone(Sticker sticker, Vector3 rot, bool forceScratch)
    {
        var posSpawn = sticker.transform.position;
        posSpawn.z = forceScratch ? -2f : posSpawn.z;
        //Debug.Log("Call Spawn Sticker Done ");
        var stD = PoolManager.Instance.SpawnStickerDone(posSpawn);
        stD.SetActionCallBackOnMoveToNote(sticker.SetStickerIsDone);
        stD.InitStickerMove(sticker.stickerData.stickerID, rot);
        sticker.DisAbleIcon();
        stickerDone.Add(stD);
    }

    [Button]
    public void ResetLevel()
    {
        LoadDataClean();
        //Debug.Log("Reset Level");
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
        CheckToCloseAllUI();
        ResetLevel();
        isEndGame = true;
        levelIndex.Value++;
#if !UNITY_EDITOR
        
        IngameFirebaseAnalystic.Instance.SetLevelUserProperty();
#endif
        if (levelChange.Value != -1)
        {
            levelChange.Value = -1;
            PlayerInfoDataSave.Instance.SaveData();
        }

        PlayerInfoDataSave.Instance.SaveData();
        GamePlayManager.Instance.ChangeGameState(GameState.Normal);
#if !UNITY_EDITOR
        IngameFirebaseAnalystic.Instance.TrackLevelComplete(); 
#endif
        UIManager.Instance.OpenActivity<ActivityWinGame>();
    }

    private void CheckToCloseAllUI()
    {
        if (UIManager.Instance.IsHaveScreenDefaultOpen())
            _ = UIManager.Instance.CloseScreenDefaultAsync();

        if (UIManager.Instance.IsHaveModalOpen())
            _ = UIManager.Instance.CloseModalAsync();

        if (UIManager.Instance.IsHaveActivityOpen())
            _ = UIManager.Instance.CloseAllActivity();
    }

    public void MoveFolderOut(FolderHaveSticker folder)
    {
        _ = oSController.MoveFolderOut(folder);
    }

    public bool IsHaveStickerWait()
    {
        return fSpaceController.IsHaveStickerWait();
    }

    public void CheckStickerDone()
    {
        fSpaceController.CheckStickerDone();
    }

    [Button]
    public async UniTask CheckLoseGame()
    {
        
        if (UnitEventManager.Instance.IsHaveEvent())
        {
            Debug.Log("have event, wait to check lose game!");
            await UniTask.WaitUntil(() => !UnitEventManager.Instance.IsHaveEvent());
        }

        if (isEndGame)
            return;
        Debug.Log("Check lose game!");
        
        var isFreeSlot = fSpaceController.IsHaveFreeSlot();
        var isHaveNoteMoveIn = oSController.IsHaveNoteMoveIn();
        var noteDontHaveStickerOnMove = CheckAllNoteDontHaveStickerOnMove();
        var noteHaveStickerDoneOnWait = CheckNoteHaveStickerDoneOnWait();
        var noteHaveStickerOnCard = CheckAllCardActiveHaveStickerSameIdWithNote();
        var noteHaveStickerOnSpace = CheckAllNoteHaveStickerOnFreeSpace();
        var noteHaveStickerOnListDone = CheckAllNoteHaveStickerOnListDone();

        // Debug.Log($" is free slot {isFreeSlot}");
        // Debug.Log("Is have note move in: " + isHaveNoteMoveIn);
        // Debug.Log("note dont have sticker on move: " + noteDontHaveStickerOnMove);
        // Debug.Log("note have sticker done on wait: " + noteHaveStickerDoneOnWait);
        // Debug.Log("note have sticker done on card: " + noteHaveStickerOnCard);
        // Debug.Log("note have sticker done on free space : " + noteHaveStickerOnSpace);
        // Debug.Log("note have sticker done on list : " + noteHaveStickerOnListDone);

        if (!noteHaveStickerOnListDone && !noteHaveStickerOnSpace && !isFreeSlot && !isHaveNoteMoveIn &&
            noteDontHaveStickerOnMove && !noteHaveStickerDoneOnWait && !noteHaveStickerOnCard)
        {
            _ = EndGame();
        }
    }

    private bool CheckAllNoteHaveStickerOnListDone()
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
                        //Debug.Log(noteId + $" is have sticker done {stickerDone[j]} {j}");
                        return true;
                    }
                }
            }
        }

        return false;
    }

    private bool CheckAllNoteHaveStickerOnFreeSpace()
    {
        var slotFolders = oSController.SlotFolders;
        var spaceStickers = fSpaceController.spaceStickers;
        for (var i = 0; i < slotFolders.Length; i++)
        {
            var noteId = slotFolders[i].GetNoteId();
            if (noteId == -1) continue;
            for (var j = 0; j < spaceStickers.Count; j++)
            {
                var st = spaceStickers[j].stickerPos.obj;
                if (!st) continue;
                var isSame = st.IsHaveSticker(noteId);
                if (isSame)
                    return true;
            }
        }

        return false;
    }

    private bool CheckAllCardActiveHaveStickerSameIdWithNote()
    {
        var slotFolders = oSController.SlotFolders;
        var card = layerController.cards;
        for (var i = 0; i < slotFolders.Length; i++)
        {
            var noteId = slotFolders[i].GetNoteId();
//            Debug.Log($"id check {noteId}");
            if (noteId == -1) continue;
            for (var j = 0; j < card.Count; j++)
            {
                if (!card[j].isShowed) continue;
                if (card[j].IsHaveSticker(noteId))
                    return true;
            }
        }

        return false;
    }
    // public void CheckLoseGame()
    // {
    //     if (!fSpaceController.IsHaveStickerWait()) return;
    //     if(UnitEventManager.Instance.IsHaveEvent()) return;
    //     var isHaveAllNoteOnSlot = oSController.IsHaveAllNoteOnSlot();
    //
    //     //Nếu tất cả các ô trống đều có note tức là thua hoặc
    //     // Cần kiểm tra xem có Card nào chứa sticker chưa bóc mà trùng với Note không?
    //     // Nếu có thì vẫn còn cơ hội để chơi tiếp => sửa thành thua luôn
    //     Debug.Log($"Have all note on slot: {isHaveAllNoteOnSlot}");
    //     if (isHaveAllNoteOnSlot)
    //     {
    //         //var e = CheckAllCardOnLayerHaveStickerSameIdWithNote();
    //         var e1 = CheckNoteHaveStickerDoneOnWait();
    //         Debug.Log($"note Have Sticker Done: {e1}");
    //         if (/*!e && */!e1)
    //         {
    //             Debug.Log("call end game form here");
    //             //Time.timeScale = 0f;
    //             _ = EndGame();
    //             return;
    //         }
    //     }
    //
    //     //Nếu có ít nhất 1 note trên slot thì vẫn còn cơ hội để thắng
    //     //Nếu có note nào đang chuẩn bị vào thì vẫn có thể chơi tiếp
    //
    //     var isHaveAtLeastOne = oSController.IsHaveAtLeastOneNote();
    //     if (!isHaveAtLeastOne)
    //     {
    //         Debug.Log("call end game form here");
    //         _ = EndGame();
    //     }
    // }

    private bool CheckNoteHaveStickerDoneOnWait()
    {
        var slotFolders = oSController.SlotFolders;
        var stickerWait = fSpaceController.stickerDoneWait;
        for (var i = 0; i < slotFolders.Length; i++)
        {
            var noteId = slotFolders[i].GetNoteId();
            if (noteId != -1)
            {
                for (var j = 0; j < stickerWait.Count; j++)
                {
                    if (stickerWait[j].IsHaveSticker(noteId))
                    {
                        Debug.Log(noteId + $" is have sticker done {stickerWait[j]} {j}");
                        return true;
                    }
                }
            }
        }

        return false;
    }

    private bool CheckAllNoteDontHaveStickerOnMove()
    {
        var slotFolders = oSController.SlotFolders;
        for (var i = 0; i < slotFolders.Length; i++)
        {
            var note = slotFolders[i].folderPos.obj;
            if (note != null)
            {
                var isHaveStickerOnMove = note.IsHaveStickerOnMove();
                if (isHaveStickerOnMove)
                    return false;
            }
        }

        return true;
    }

    private async UniTask EndGame()
    {
        GamePlayManager.Instance.ChangeGameState(GameState.LoseGame);
        await UniTask.WaitForFixedUpdate();
        if (isEndGame)
            return;
        isEndGame = true;
        Debug.Log("game over");
        await UniTask.WaitForSeconds(1f);
        IngameFirebaseAnalystic.Instance.SetNoteFail(GetNoteFail());
        IngameFirebaseAnalystic.Instance.SetLoseType(LoseType.OutSlot);
        IngameFirebaseAnalystic.Instance.TrackLevelFail();

        await UIManager.Instance.OpenModalAsync<ModalRevive>();
        //await UIManager.Instance.OpenActivityAsync<ActivityLoseGame>();
    }

    public void RemoveStickerDone(StickerDone stD)
    {
        stickerDone.Remove(stD);
    }

    public void AddSlot()
    {
        fSpaceController.AddSlot();
    }

    public Difficulty GetLevelDifficult()
    {
        return MyCache.GetDifficultByLevel(levelIndex.Value + 1);
    }

    public bool CheckCanUsingBooster(BoosterType boosterType)
    {
        return boosterType switch
        {
            BoosterType.BoosterMagnet => CheckCanUsingMagnet(),
            BoosterType.BoosterAddSlot => CheckCanUsingAddSlot(),
            BoosterType.BoosterHammer => CheckCanUsingHammer(),
            _ => false
        };
    }

    private bool CheckCanUsingHammer() => layerController.CheckCanUsingHammer();
    private bool CheckCanUsingAddSlot() => fSpaceController.IsCanAddSlot();
    private bool CheckCanUsingMagnet() => oSController.IsHaveNoteMoveIn();

    public Sticker GetRandomStickerTransform() => layerController.GetRandomStickerTransform();

    public int GetNoteFail() => oSController.objHaveStickers.Count;

    public void AddSlotNote() => oSController.AddSlotNote();
}