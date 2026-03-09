using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using TW.Utility.DesignPattern.UniTaskState;
using UnityEngine;

public class CardInitState : IState
{
    public interface IHandler
    {
        UniTask OnEnterInitState();
        UniTask OnUpdateInitState();
        UniTask OnExitInitState();
    }

    private IHandler handler;

    public CardInitState(IHandler owner)
    {
        handler = owner;
    }

    public UniTask OnEnter(CancellationToken ct)
    {
        return handler.OnEnterInitState();
    }

    public UniTask OnUpdate(CancellationToken ct)
    {
        return handler.OnUpdateInitState();
    }

    public UniTask OnExit(CancellationToken ct)
    {
        return handler.OnExitInitState();
    }
}
public partial class Card : CardInitState.IHandler
{
    private CardInitState CardInitStateCache { get; set; }
    public CardInitState CardInitState => CardInitStateCache ??= new CardInitState(this);
    
    public CardData data;
    
    public UniTask OnEnterInitState()
    {
        LoadData();
        return UniTask.CompletedTask;
    }

    public UniTask OnUpdateInitState()
    {
        return UniTask.CompletedTask;
    }

    public UniTask OnExitInitState()
    {
        return UniTask.CompletedTask;
    }
    
    public void InitData(CardData cardData, int layer)
    {
        data = cardData;
        layerIndex = layer;
        stateMachine.RequestTransition(CardInitState);
        var pos = transform.position;
        pos.z = layerIndex;
        transform.position = pos;
        transform.eulerAngles = data.rotation;
        countSticker = cardData.stickers.Length;
        cardGraphic.InitData();
        scratchObject = PoolManager.Instance.SpawnScratchManager();
    }

    private void LoadData()
    {
        for (var i = 0; i < data.stickers.Length; i++)
        {
            var sticker = PoolManager.Instance.SpawnSticker();
            sticker.InitData(data.stickers[i], data.rotation);
            sticker.SetParents(stickerPoints[i]);
            stickers.Add(sticker);
            stickerSubscriptions.Add(sticker.isDone.Skip(1).Subscribe(ChangeStickerScratchDone));
        }
        stateMachine.RequestTransition(CardWaitState);
    }
}