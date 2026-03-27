using System;
using System.Collections.Generic;
using Core.UI.Modals;
using Cysharp.Threading.Tasks;
using R3;
using Sirenix.OdinInspector;
using TW.Utility.DesignPattern;
using UniRx;
using UnityEngine;

public class TutorialManager : Singleton<TutorialManager>
{
    public List<int> listIDTutorialDone;
    public List<BoosterType> boosterUnlock;
    public TutorialConfig currentTutorialConfig;
    public Transform trsHand;
    public Animator handAnim;
    public SpriteRenderer sprHand;
    public Sprite[] sprHands;

    private void Start()
    {
        listIDTutorialDone = TutorialDataSave.Instance.tutorialIDComplete;
        boosterUnlock = TutorialDataSave.Instance.boosterUnlock;
    }
    
    public bool IsUnLockBooster(BoosterType boosterType)
    {
        TutorialDataSave.Instance.SaveData();
        return boosterUnlock.Contains(boosterType);
    }

    private void AddBooster(BoosterType boosterType)
    {
        if (IsUnLockBooster(boosterType)) return;
        boosterUnlock.Add(boosterType);
        TutorialDataSave.Instance.SaveData();
    }

    [Button]
    public void OnCheckShowLevel()
    {
        var level = Level.Instance.levelIndex.Value;
        var e = TutorialGlobalConfig.Instance.GetTutorialConfig(level);
        if (e != null)
        {
            Debug.Log($"Show tutorial {level}");
            ShowTutorial(e);
        }
    }

    private void ShowTutorial(TutorialConfig tutorialConfig)
    {
        currentTutorialConfig = tutorialConfig;
        if (tutorialConfig.handAction != null)
        {
            SetTutorialConfigHandActionData();
            tutorialConfig.handAction.TutorialAction();
        }
        else
        {
            ShowTutorialModal();
        }
    }

    private void ShowTutorialModal()
    {
        Debug.Log("Show tutorial modal");
        _ = UIManager.Instance.OpenModalAsync<ModalTutorial>(currentTutorialConfig);
    }

    private void SetTutorialConfigHandActionData()
    {
        currentTutorialConfig.handAction.handAnim = handAnim;
        currentTutorialConfig.handAction.srHand = sprHand;
        currentTutorialConfig.handAction.trsHand = trsHand;
        currentTutorialConfig.handAction.sprHands = sprHands;
        currentTutorialConfig.handAction.actionCallBack = DoneTutorial;
    }

    public void DoneTutorial()
    {
        if(listIDTutorialDone.Contains(currentTutorialConfig.id)) return;
        listIDTutorialDone.Add(currentTutorialConfig.id);
        CheckAddBoosterUnlock(currentTutorialConfig.tutorialType);
        currentTutorialConfig = null;
        OnCheckShowLevel();
        TutorialDataSave.Instance.SaveData();
    }

    private void CheckAddBoosterUnlock(TutorialType tutorialType)
    {
        switch (tutorialType)
        {
            case TutorialType.BoosterAddSlot:
                GlobalEventManager.OnUnlockBooster?.Invoke(BoosterType.AddSlot);
                AddBooster(BoosterType.AddSlot);
                return;
            case TutorialType.BoosterHammer:
                GlobalEventManager.OnUnlockBooster?.Invoke(BoosterType.Hammer);
                AddBooster(BoosterType.Hammer);
                return;
            case TutorialType.BoosterMagnet:
                GlobalEventManager.OnUnlockBooster?.Invoke(BoosterType.Magnet);
                AddBooster(BoosterType.Magnet);
                return;
            case TutorialType.None:
            case TutorialType.Scratch:
            case TutorialType.BoosterAddNote:
            default:
                return;
        }
    }

    public bool IsShowedThatTutorial(int id)
    {
        for (var i = 0; i < listIDTutorialDone.Count; i++)
        {
            if (listIDTutorialDone[i] == id)
            {
                return true;
            }
        }

        return false;
    }
}

public class TutorialHandAction
{
    public Transform trsHand;
    public SpriteRenderer srHand;
    public Animator handAnim;
    public Sprite[] sprHands;
    public Action actionCallBack;

    public virtual void TutorialAction()
    {
    }
}

[Serializable]
public class TutorialHandScratchAction : TutorialHandAction
{
    public override void TutorialAction()
    {
        var e = Level.Instance.GetRandomStickerTransform();
        if (!e)
        {
            actionCallBack?.Invoke();
            return;
        }
        var reactive = e.isCallDone;
        reactive.Subscribe(ChangeDone).AddTo(e);
        var pos = e.transform.position;
        pos.z = -5;
        trsHand.transform.position = pos;
        trsHand.gameObject.SetActive(true);
        handAnim.Play("Scratch");
    }

    private void ChangeDone(bool doneSticker)
    {
        if (!doneSticker) return;
        trsHand.gameObject.SetActive(false);
        TutorialAction();
    }
}

[Serializable]
public class TutorialHandAddNote : TutorialHandAction
{
    public override void TutorialAction()
    {
        var slotNote = Level.Instance.oSController.SlotFolders[^1];
        var pos = slotNote.transform.position;
        pos.z = -5;
        trsHand.transform.position = pos;
        trsHand.gameObject.SetActive(true);
        handAnim.Play("Press");
        var e = slotNote.slotFolderType;
        e.Subscribe(ChangeDone).AddTo(slotNote);
    }

    public void ChangeDone(SlotFolderType type)
    {
        if (type != SlotFolderType.Normal) return;
        trsHand.gameObject.SetActive(false);
        actionCallBack?.Invoke();
    }
}