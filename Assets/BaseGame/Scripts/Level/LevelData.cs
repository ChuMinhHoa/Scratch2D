using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Unity.Mathematics;

[Serializable]
public class LevelData
{
    public int level;
    public ObjHaveStickerData[] objHaveStickers;
    public LayerCardData[] layerCards;

    public bool IsValid(out IGrouping<int, int>[] errors)
    {
        errors = new IGrouping<int, int>[] { };
        return true;
    }

}

[Serializable]
public class ObjHaveStickerData
{
    public int objID;
    public int stickerId;
}

[Serializable]
public class LayerCardData
{
    public int layerIndex;
    public CardData[] cards;
}

[Serializable]
public class CardData
{
    public CardType cardType;
    public CardState cardState;
    public StickerData[] stickers;
    public float3 position;
    public float3 rotation;
}

[Serializable]
public class StickerData
{
    public int stickerID;
    public StickerType stickerType;
    [ShowIf("@stickerType == StickerType.Chain")]
    public int stickerIndexChain;
}

public enum StickerType
{
    Normal,
    Chain
}

public enum CardType
{
    Card1 = 0,
    Card2 = 100,
    Card3 = 200,
    Card4 = 300,
    Card5 = 400,
    Card6 = 500,
    Card7 = 600,
    Card8 = 700,
    Card9 = 800,
    Card10 = 900
}

public enum CardState
{
    Normal,
    Lock,
    Freeze,
    Key
}