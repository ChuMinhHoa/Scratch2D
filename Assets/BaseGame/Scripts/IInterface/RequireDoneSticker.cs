using UnityEngine;

public interface IRequireDoneSticker
{
    bool CheckDoneSticker();
}

[System.Serializable]
public class RequireDoneSticker : IRequireDoneSticker
{
    public Sticker sticker;
    public virtual bool CheckDoneSticker()
    {
        return true;
    }
}

[System.Serializable]
public class ChainRequireDoneSticker : RequireDoneSticker
{
    public override bool CheckDoneSticker()
    {
        return ((StickerChain)sticker).stickerChain.isDone;
    }
}
