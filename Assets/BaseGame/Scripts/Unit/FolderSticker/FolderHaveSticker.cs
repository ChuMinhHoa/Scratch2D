using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using LitMotion;
using R3;
using Sirenix.OdinInspector;
using TW.Utility.DesignPattern.UniTaskState;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;

public partial class FolderHaveSticker : MonoBehaviour
{
    public int objId;
    public StickerPos[] trsStickerPos;
    public UnitAnimation unitAnim;

    public MaterialPropertyBlock propertyBlock;
    public GameObject effectDone;

    private static readonly int DirectionalAlphaFadeFade = Shader.PropertyToID("_DirectionalAlphaFadeFade");

    public Renderer _renderer;

    public FHSGraphic fhsGraphic;
    public StateMachine stateMachine;

    public SelectAbleOnBooster selectAbleOnBooster;
    private bool onSlot = false;

    private void Start()
    {
        propertyBlock = new MaterialPropertyBlock();
        _renderer.GetPropertyBlock(propertyBlock);

        stateMachine.RequestTransition(FhsWaitState);
        stateMachine.Run();

        for (var i = 0; i < trsStickerPos.Length; i++)
        {
            trsStickerPos[i].moveDone.Skip(1).Subscribe(StickerMoveDone).AddTo(this);
        }

        selectAbleOnBooster.SetConditionToSelect(ConditionToSelect);
    }

    private bool ConditionToSelect()
    {
        if (!onSlot)
            return false;
        var e = IsAllSlotNotNull();
        if (e)
            return false;
        return stateMachine.CurrentState != FhsDoneState;
    }

    private bool IsAllSlotNotNull()
    {
        for (var i = 0; i < trsStickerPos.Length; i++)
        {
            if (!trsStickerPos[i].IsHaveObj()) return false;
        }

        return true;
    }

    private void StickerMoveDone(bool stickerMoveDone)
    {
        for (var i = 0; i < trsStickerPos.Length; i++)
        {
            if (!trsStickerPos[i].moveDone)
            {
                return;
            }
        }

        Level.Instance.MoveFolderOut(this);
    }

    public bool IsSameSticker(int id, out StickerPos stickerPos)
    {
        stickerPos = null;
        if (data.stickerId != id) return false;

        foreach (var pos in trsStickerPos)
        {
            if (pos.IsHaveObj()) continue;
            stickerPos = pos;
            return true;
        }

        return false;
    }

    public void ResetFolderSticker()
    {
        for (var i = 0; i < trsStickerPos.Length; i++)
        {
            if (!trsStickerPos[i].obj) continue;
            trsStickerPos[i].obj.ResetStickerDone();
            PoolManager.Instance.DespawnStickerMove(trsStickerPos[i].obj);
            trsStickerPos[i].ResetPos();
        }

        PoolManager.Instance.DespawnObjHaveSticker(this);
    }

    [Button]
    public async UniTask MoveOut(Transform posOut)
    {
        onSlot = false;
        stateMachine.RequestTransition(FhsDoneState);
        var id = UnitEventManager.Instance.RegisterEvent();
        var currentPos = transform.position;
        effectDone.SetActive(true);
        SoundManager.Instance.PlaySoundSfx(AudioKey.Sfx_NoteDone);
        await LMotion.Create(1f, -10f, 0.25f).Bind(x =>
        {
            _renderer.GetPropertyBlock(propertyBlock);
            propertyBlock.SetFloat(DirectionalAlphaFadeFade, x);
            _renderer.SetPropertyBlock(propertyBlock);
        }).AddTo(this);
        
        await unitAnim.PlayScaleAnimation();

        await LMotion.Create(currentPos, posOut.position, 0.25f).Bind(x => transform.position = x).AddTo(this);
        UnitEventManager.Instance.RemoveEventId(id);
        ResetFolderSticker();
        Level.Instance.oSController.OnNoteDone();
    }

    public async UniTask MoveToTarget(Vector3 target)
    {
        var id = UnitEventManager.Instance.RegisterEvent();
        //await unitAnim.PlayMoveAnim(target);
        transform.position = target;
        transform.localScale = Vector3.zero;
        await LMotion.Create(0f, 1f, 0.25f).WithEase(Ease.OutBack).Bind(x => transform.localScale = Vector3.one * x).AddTo(this);
        UnitEventManager.Instance.RemoveEventId(id);
        await UniTask.WaitForSeconds(0.1f);
        Level.Instance.CheckStickerDone();
        
        Level.Instance.CheckLoseGame();

        onSlot = true;

        if (GamePlayManager.Instance.gameState == GameState.OnBooster)
            if(selectAbleOnBooster.CheckCondition)
                selectAbleOnBooster.OnBoosterUsing(BoosterManager.Instance.currentBoosterType, BoosterManager.Instance.currentIBooster);
    }

    public bool IsHaveStickerOnMove()
    {
        for (var i = 0; i < trsStickerPos.Length; i++)
        {
            if (trsStickerPos[i].obj && !trsStickerPos[i].IsMoveDone())
            {
                return true;
            }
        }

        return false;
    }
}

[Serializable]
public class StickerPos : ObjPos<StickerDone>
{
    public int id;
    public Reactive<bool> moveDone;

    public void MoveDone()
    {
        moveDone.Value = true;
    }

    public bool IsMoveDone() => moveDone;

    public override void ResetPos()
    {
        id = -1;
        obj = null;
        moveDone.Value = false;
        lock (_lock)
        {
            Owner = null;
        }
    }

    private readonly object _lock = new object();
    public StickerDone Owner { get; private set; }

    public bool IsOccupied => Owner != null;

    public bool TryReserve(StickerDone requester)
    {
        lock (_lock)
        {
            if (Owner == null)
            {
                Owner = requester;
                return true;
            }

            return false;
        }
    }

    public void Release(StickerDone requester)
    {
        lock (_lock)
        {
            if (Owner == requester)
                Owner = null;
        }
    }
}

[Serializable]
public class FolderPos : ObjPos<FolderHaveSticker>
{
    public int id;
    public bool moveDone;

    public void MoveDone()
    {
        moveDone = true;
    }

    public bool IsMoveDone() => moveDone;

    public override void ResetPos()
    {
        id = -1;
        obj = null;
        moveDone = false;
    }
}

[Serializable]
public class ObjPos<T>
{
    [FormerlySerializedAs("trsStickerPos")]
    public Transform trsPos;

    public T obj;

    public void RegisterObj(T objChange)
    {
        obj = objChange;
    }

    public bool IsHaveObj()
    {
        return obj != null;
    }

    public virtual void ResetPos()
    {
        obj = default;
    }
}