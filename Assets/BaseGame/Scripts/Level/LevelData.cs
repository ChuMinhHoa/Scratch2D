using System;
using System.Collections.Generic;
using System.Linq;
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
    public int[] stickerIds;
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
    public StickerData[] stickers;
    public float3 position;
}

[Serializable]
public class StickerData
{
    public int stickerID;
    public StickerType stickerType;
}

public enum StickerType
{
    Normal
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