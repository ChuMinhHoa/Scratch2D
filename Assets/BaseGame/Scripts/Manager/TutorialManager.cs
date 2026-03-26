using System;
using Cysharp.Threading.Tasks;
using R3;
using TW.Utility.DesignPattern;
using UniRx;
using UnityEngine;

public class TutorialManager : Singleton<TutorialManager>
{
    public Reactive<int> currentLevel = new(0);

    private void Start()
    {
        currentLevel = Level.Instance.levelIndex;
        currentLevel.Subscribe(OnChangeLevel).AddTo(this);
    }

    private void OnChangeLevel(int levelChange)
    {
        var e = TutorialGlobalConfig.Instance.GetTutorialConfig(levelChange);
        if (e != null)
        {
            //Debug.Log($"Show tutorial {levelChange}");
            ShowTutorial(e);
        }
    }

    private void ShowTutorial(TutorialConfig tutorialConfig)
    {
        
    }

    public bool IsShowThatTutorial(int levelShow)
    {
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
    public virtual void TutorialAction(){}

    public void HandDown()
    {
        srHand.sprite = sprHands[1];
    }

    public void HandUp()
    {
        srHand.sprite = sprHands[0];
    }

    public async UniTask HandPress(float timePress)
    {
        HandDown();
        await UniTask.WaitForSeconds(timePress);
        HandUp();
    }
}

[Serializable]
public class TutorialHandScratchAction : TutorialHandAction
{
    public override void TutorialAction()
    {
        var e = Level.Instance.GetRandomStickerTransform();
        if (!e)
            return;
        var reactive = e.isDone;
        reactive.Subscribe(ChangeDone).AddTo(e);
        trsHand.transform.position = e.transform.position;
        handAnim.Play("Scratch");
    }

    private void ChangeDone(bool doneSticker)
    {
        TutorialAction();
    }
}
