using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

public class StickerChain : Sticker
{
    [Title("STICKER CHAIN")] public Sticker stickerChain;
    
    public override async UniTask OnEnterDoneState()
    {
        Debug.Log("Done state");
        if (!stickerChain.IsOnDoneState)
            stickerChain.OnDoneProgress();
        await stickerGraphic.OnDoneMode();
        StickerMoveToTarget();
    }
}
