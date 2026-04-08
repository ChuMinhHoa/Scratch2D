using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using TW.Utility.DesignPattern.UniTaskState;
using UnityEngine;

public class CardChainState : IState
{
    public interface IHandler
    {
        UniTask OnEnterChainState();
        UniTask OnUpdateChainState();
        UniTask OnExitChainState();
    }
    
    private IHandler handler;
    public CardChainState(IHandler owner)
    {
        handler = owner;
    }
    
    public UniTask OnEnter(CancellationToken ct)
    {
        return handler.OnEnterChainState();
    }

    public UniTask OnUpdate(CancellationToken ct)
    {
        return handler.OnUpdateChainState();
    }

    public UniTask OnExit(CancellationToken ct)
    {
        return handler.OnExitChainState();
    }
}

public partial class Card : CardChainState.IHandler
{
    private CardChainState CardChainStateCache { get; set; }
    public CardChainState CardChainState => CardChainStateCache ??= new CardChainState(this);
    
    public UniTask OnEnterChainState()
    {
        HashSet<StickerData> stickerSet = data.stickers.ToHashSet();
        for (var i = 0; i < stickerSet.Count; i++)
        {
            if (data.stickers[i].stickerIndexChain != 0)
            {
                var point1 = stickerPoints[i].position;
                var point2 = stickerPoints[data.stickers[i].stickerIndexChain].position;
                stickerSet.Remove(data.stickers[i]);
                cardGraphic.SetupChainLine(point1, point2);
            }
        }
        
        return UniTask.CompletedTask;
    }

    public UniTask OnUpdateChainState()
    {
        return UniTask.CompletedTask;
    }

    public UniTask OnExitChainState()
    {
        return UniTask.CompletedTask;
    }
}
