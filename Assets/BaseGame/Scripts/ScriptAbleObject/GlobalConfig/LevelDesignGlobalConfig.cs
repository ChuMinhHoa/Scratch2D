using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelDesignGlobalConfig", menuName = "GlobalConfigs/LevelDesignGlobalConfig")]
[GlobalConfig("Assets/Resources/GlobalConfig/")]
public class LevelDesignGlobalConfig : GlobalConfig<LevelDesignGlobalConfig>
{
    [field: SerializeField, LevelDataEditor]
    public Level CurrentLevel { get; set; }
}

[System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
public sealed class LevelDataEditor : System.Attribute
{
}

#if UNITY_EDITOR
public sealed class LevelDataEditorAttributeDrawer : OdinAttributeDrawer<LevelDataEditor, Level>
{
    private static LevelData LevelData { get; set; }
    
    private static float defaultHeightButtonHeader = 25f;
    private float defaultStickerHeight = 70f;
    private float defaultStickerWidth = 90f;
    private float defaultSpace = 5;
    private float vectorSpace = 20;
    private int totalStickerInRow = 3;
    private int totalCardInRow = 3;

    protected override void DrawPropertyLayout(GUIContent label)
    {
        Rect rect1 = EditorGUILayout.GetControlRect();
        ValueEntry.SmartValue =
            SirenixEditorFields.UnityObjectField(rect1, ValueEntry.SmartValue, typeof(Level), true) as Level;
        if (!ValueEntry.SmartValue) return;
        LevelData = ValueEntry.SmartValue.LevelData;
        EditorUtility.SetDirty(ValueEntry.SmartValue);

        float availableWidth = EditorGUIUtility.currentViewWidth - 30;
        int columns = Mathf.Max(1, Mathf.FloorToInt(availableWidth / defaultStickerWidth));

        ObjHaveStickerData[] objHaveStickerData = LevelData.objHaveStickers;
        int rows = Mathf.CeilToInt((float)objHaveStickerData.Length / columns);
        float height = rows * (defaultStickerHeight + 5) + defaultHeightButtonHeader + 10;
        Rect objHaveStickerRect = EditorGUILayout.GetControlRect(false, height);
        DrawObjHaveSticker(objHaveStickerRect, objHaveStickerData);

        //LayerCardData[] layerData = LevelData.layerCards;
        //rows = Mathf.CeilToInt((float)layerData.Length / columns);
        //height = rows * (SlotSize.y + 5) + defaultWidthHeight + 10;
        var heightTotal = GetHeightTotalLayer() + 25;
        Rect layerStickerRect = EditorGUILayout.GetControlRect(false, heightTotal);
        //layerStickerRect.width = SlotSize.x * 9 + 80;
        DrawLayerStickerRect(layerStickerRect);
    }

    private void DrawLayerStickerRect(Rect rect)
    {
        SirenixEditorGUI.DrawSolidRect(rect, new Color(0.4f, 0.5f, 0.77f, 1.0f));
        SirenixEditorGUI.DrawBorders(rect, 1);
        var rect1 = new Rect(rect.x + 5, rect.y + 5, rect.width - 10, defaultHeightButtonHeader);
        if (GUI.Button(rect1.AlignRight(40).AlignLeft(20).SetSize(20), "+"))
        {
            var layerDataList = LevelData.layerCards.ToList();
            layerDataList.Add(DefaultDataCreator.CreateDefaultLayerData());
            LevelData.layerCards = layerDataList.ToArray();
        }

        if (GUI.Button(rect1.AlignRight(20).AlignLeft(20).SetSize(20), "-"))
        {
            var layerDataList = LevelData.layerCards.ToList();
            layerDataList.RemoveAt(layerDataList.Count - 1);
            LevelData.layerCards = layerDataList.ToArray();
        }

        var currentY = rect.y + 25;
        var currentX = rect.x + 5;

        var layerData = LevelData.layerCards;
        for (var i = 0; i < layerData.Length; i++)
        {
            var layerHeight = GetHeightLayer(i);
            Rect layerStickerRect = new Rect(currentX, currentY, rect.width - 10, layerHeight);
            DrawLayerCard(layerStickerRect, i);
            currentY += layerHeight;
        }
    }

    private int currentLayerIndex = 0;
    private int currentCardIndex = 0;
    private int currentStickerIndex = 0;
    private void DrawLayerCard(Rect rect, int layerIndex)
    {
        currentLayerIndex = layerIndex;

        SirenixEditorGUI.DrawSolidRect(rect, Color.gray);
        SirenixEditorGUI.DrawBorders(rect, 1);
        var rect1 = new Rect(rect.x + 5, rect.y + 5, rect.width - 10, defaultHeightButtonHeader);

        if (GUI.Button(rect1.AlignRight(40).AlignLeft(20).SetSize(20), "+"))
        {
            var cardList = LevelData.layerCards[layerIndex].cards.ToList();
            cardList.Add(DefaultDataCreator.CreateDefaultCardData());
            LevelData.layerCards[layerIndex].cards = cardList.ToArray();
        }

        if (GUI.Button(rect1.AlignRight(20).AlignLeft(20).SetSize(20), "-"))
        {
            var cardList = LevelData.layerCards[layerIndex].cards.ToList();
            cardList.RemoveAt(cardList.Count - 1);
            LevelData.layerCards[layerIndex].cards = cardList.ToArray();
        }

        var currentX = rect.x + 5;
        var currentY = rect.y + defaultHeightButtonHeader;
        var cardsData = LevelData.layerCards[layerIndex].cards;

        var cardIndex = 0;
        var rowIndex = 0;
        for (var i = 0; i < cardsData.Length; i++)
        {
            if (cardIndex % 3==0 && cardIndex != 0)
            {
                currentX = rect.x + 5;
                var maxHeightCard = GetMaxHeightCard(layerIndex, rowIndex);
                currentY += maxHeightCard + defaultSpace;
                rowIndex++;
                cardIndex = 0;
            }
            var totalStickerOnCard = cardsData[i].stickers.Length;
            var totalLineSticker = Mathf.CeilToInt((float)totalStickerOnCard / totalStickerInRow);
            var cardWidth = defaultStickerWidth * totalStickerInRow + defaultSpace * 2;
            var cardHeight = defaultStickerHeight * totalLineSticker + defaultHeightButtonHeader + defaultSpace + defaultSpace + vectorSpace*2;
            
            Rect cardRect = new Rect(currentX, currentY, cardWidth, cardHeight);
            DrawCard(cardRect, i);
            currentX += cardWidth+5;
            cardIndex++;
        }
    }

    private void DrawCard(Rect rect, int cardIndex)
    {
        currentCardIndex = cardIndex;
        var stickerData = LevelData.layerCards[currentLayerIndex].cards[cardIndex].stickers;
        SirenixEditorGUI.DrawSolidRect(rect, Color.gray2);
        SirenixEditorGUI.DrawBorders(rect, 1);
        var rect1 = new Rect(rect.x + 5, rect.y + 5, rect.width - 10, defaultHeightButtonHeader);
        if (GUI.Button(rect1.AlignRight(40).AlignLeft(20).SetSize(20), "+"))
        {
            var stickerList = stickerData.ToList();
            stickerList.Add(DefaultDataCreator.CreateDefaultSticker());
            LevelData.layerCards[currentLayerIndex].cards[cardIndex].stickers = stickerList.ToArray();
        }

        if (GUI.Button(rect1.AlignRight(20).AlignLeft(20).SetSize(20), "-"))
        {
            var stickerList = stickerData.ToList();
            stickerList.RemoveAt(stickerList.Count - 1);
            LevelData.layerCards[currentLayerIndex].cards[cardIndex].stickers = stickerList.ToArray();
        }
        
        var currentY = rect.y + 5;
        
        EditorGUI.BeginChangeCheck();
        Rect rectCardType = new Rect(rect.x + 5, currentY, rect.width - 50, 20);
        LevelData.layerCards[currentLayerIndex].cards[cardIndex].cardType = (CardType)EditorGUI.EnumPopup(rectCardType, LevelData.layerCards[currentLayerIndex].cards[cardIndex].cardType);
        if (EditorGUI.EndChangeCheck())
        {
            var newCardType = LevelData.layerCards[currentLayerIndex].cards[cardIndex].cardType;
            var totalSticker = LevelDesignHelper.GetTotalStickerOnCard(newCardType);
    
            var newStickers = new StickerData[totalSticker];
            for (var i = 0; i < totalSticker; i++)
            {
                if (i < stickerData.Length)
                {
                    newStickers[i] = stickerData[i];
                }
                else
                {
                    newStickers[i] = DefaultDataCreator.CreateDefaultSticker();
                }
            }
    
            LevelData.layerCards[currentLayerIndex].cards[cardIndex].stickers = newStickers;
        }
        
        currentY = rect.y + defaultHeightButtonHeader;
        var currentX = rect.x + 5;
        for (var i = 0; i < LevelData.layerCards[currentLayerIndex].cards[cardIndex].stickers.Length; i++)
        {
            if (i%totalStickerInRow==0 && i != 0)
            {
                currentX = rect.x + 5;
                currentY += defaultStickerHeight;
            }
            Rect stickerRect = new Rect(currentX, currentY, defaultStickerWidth, defaultStickerHeight);
            DrawSticker(stickerRect, i);
            currentX += defaultStickerWidth;
            
        }
        var totalRow = Mathf.CeilToInt((float)LevelData.layerCards[currentLayerIndex].cards[cardIndex].stickers.Length / totalStickerInRow);
        currentY = rect.y + defaultHeightButtonHeader + defaultStickerHeight * totalRow;
        currentX = rect.x + 5;

        float fieldWidth = rect.width - 10;
        float labelW = EditorGUIUtility.labelWidth;
        EditorGUIUtility.labelWidth = 30;

        Rect posRect = new Rect(currentX, currentY, fieldWidth, 20);
        Vector3 pos = LevelData.layerCards[currentLayerIndex].cards[cardIndex].position;
        pos = EditorGUI.Vector3Field(posRect, "Pos", pos);
        LevelData.layerCards[currentLayerIndex].cards[cardIndex].position = pos;

        currentY += 25;
        Rect rotRect = new Rect(currentX, currentY, fieldWidth, 20);
        Vector3 rot = LevelData.layerCards[currentLayerIndex].cards[cardIndex].rotation;
        rot = EditorGUI.Vector3Field(rotRect, "Rot", rot);
        LevelData.layerCards[currentLayerIndex].cards[cardIndex].rotation = rot;

        EditorGUIUtility.labelWidth = labelW;
    }

    private void DrawSticker(Rect rect, int stickerIndex)
    {
        currentStickerIndex = stickerIndex;
        var stickerData = LevelData.layerCards[currentLayerIndex].cards[currentCardIndex].stickers[stickerIndex];
        var color = Color.gray3;
        var isChain = stickerData.stickerType != StickerType.Normal;
        color = isChain ? Color.yellow : color;
        SirenixEditorGUI.DrawSolidRect(rect, color);
        SirenixEditorGUI.DrawBorders(rect, 1);
        float currentY = rect.y;
        currentY += 5;
        Rect rect1 = new Rect(rect.x + 5, currentY, rect.width - 10, defaultHeightButtonHeader);
        float defaultLabelWidth = EditorGUIUtility.labelWidth;
        if (GUI.Button(new Rect(rect1.xMax - 20, rect1.y, 20, 20), "x"))
        {
            stickerData.stickerID = -1;
            stickerData.stickerType = StickerType.Normal;
        }
        
        
        var rectChain = new Rect(rect1.xMax - 20, rect1.y+20, 20, 20);
        SirenixEditorGUI.DrawSolidRect(rectChain, Color.yellow);
        SirenixEditorGUI.DrawBorders(rectChain, 1);
        rectChain.x += 3;
        var newValue = EditorGUI.Toggle(rectChain, isChain);
        if (newValue != isChain)
        {
            stickerData.stickerType = newValue ? StickerType.Chain : StickerType.Normal;
        }

        EditorGUIUtility.labelWidth = defaultLabelWidth;
        //currentY += 25;
        rect1 = new Rect(rect.x + 5, currentY, 60, 60);
        stickerData.stickerID = DrawStickerSelect(rect1, stickerData.stickerID);
    }

    private void DrawObjHaveSticker(Rect rect, ObjHaveStickerData[] objHaveStickerData)
    {
        SirenixEditorGUI.DrawSolidRect(rect, new Color(0.4f, 0.5f, 0.77f, 1.0f));
        SirenixEditorGUI.DrawBorders(rect, 1);
        var rect1 = new Rect(rect.x + 5, rect.y + 5, rect.width - 10, defaultHeightButtonHeader);
        if (GUI.Button(rect1.AlignRight(40).AlignLeft(20).SetSize(20), "+"))
        {
            var layerDataList = objHaveStickerData.ToList();
            layerDataList.Add(DefaultDataCreator.CreateDefaultObjHaveStickerData());
            LevelData.objHaveStickers = layerDataList.ToArray();
        }

        if (GUI.Button(rect1.AlignRight(20).AlignLeft(20).SetSize(20), "-"))
        {
            var layerDataList = objHaveStickerData.ToList();
            layerDataList.RemoveAt(layerDataList.Count - 1);
            LevelData.objHaveStickers = layerDataList.ToArray();
        }

        var currentY = rect.y + 5 + defaultHeightButtonHeader;
        var currentX = rect.x + 5;
        for (var i = 0; i < objHaveStickerData.Length; i++)
        {
            if (currentX + defaultStickerWidth > rect.xMax)
            {
                currentX = rect.x + 5;
                currentY += defaultStickerHeight + 5;
            }

            Rect objHaveStickerRect = new Rect(currentX, currentY, defaultStickerWidth, defaultStickerHeight);
            DrawObjHaveStickerData(objHaveStickerRect, objHaveStickerData[i]);
            currentX += defaultStickerWidth;
        }
    }

    private static void DrawObjHaveStickerData(Rect rect, ObjHaveStickerData objHaveStickerData)
    {
        SirenixEditorGUI.DrawSolidRect(rect, Color.darkGray);
        SirenixEditorGUI.DrawBorders(rect, 1);
        float currentY = rect.y;
        currentY += 5;
        Rect rect1 = new Rect(rect.x + 5, currentY, rect.width - 10, defaultHeightButtonHeader);
        float defaultLabelWidth = EditorGUIUtility.labelWidth;
        if (GUI.Button(new Rect(rect1.xMax - 20, rect1.y, 20, 20), "x"))
        {
            objHaveStickerData.objID = -1;
            objHaveStickerData.stickerId = -1;
        }

        EditorGUIUtility.labelWidth = defaultLabelWidth;
        rect1 = new Rect(rect.x + 5, currentY, 60, 60);
        objHaveStickerData.stickerId = DrawStickerSelect(rect1, objHaveStickerData.stickerId);
    }

    private static int DrawStickerSelect(Rect rect, int stickerId)
    {
        Sprite sprite = SpriteGlobalConfig.Instance.GetStickerIcon(stickerId);
        sprite = SirenixEditorFields.PreviewObjectField(rect, sprite);
        return SpriteGlobalConfig.Instance.GetSpriteID(sprite);
    }

    private float GetHeightTotalLayer()
    {
        var totalHeight = 0f;
        for (var i = 0; i < LevelData.layerCards.Length; i++)
        {
            totalHeight += GetHeightLayer(i);
        }
        return totalHeight + defaultSpace;
    }

    private float GetHeightLayer(int layerIndex)
    {
        var layerData = LevelData.layerCards[layerIndex];
        
        var listMaxY = new List<float>();
        
        var maxY = 0f;
        var totalRow = 0;
        for (var i = 0; i < layerData.cards.Length; i++)
        {
            var rectCard = GetHeightCard(layerData.cards[i]);
            
            if (maxY < rectCard)
            {
                maxY = rectCard;
            }

            if (i % 3 == 2 || i == layerData.cards.Length - 1)
            {
                listMaxY.Add(maxY);
                maxY = 0f;
                totalRow++;
            }
        }

        var totalHeight = listMaxY.Sum();
        return totalHeight + defaultHeightButtonHeader + defaultSpace + (totalRow - 1) * defaultSpace;
    }

    private float GetMaxHeightCard(int layerIndex, int rowIndex)
    {
        var layerData = LevelData.layerCards[layerIndex];
        
        var maxY = 0f;
        var start = rowIndex * 3;
        for (var i = start; i < start+3; i++)
        {
            var rectCard = GetHeightCard(layerData.cards[i]);
            
            if (maxY < rectCard)
            {
                maxY = rectCard;
            }
        }

        return maxY;
    }

    private float GetHeightCard(CardData cardData)
    {
        var totalSticker = cardData.stickers.Length;
        var totalRowSticker = Mathf.CeilToInt((float)totalSticker / totalStickerInRow);
        var height = defaultHeightButtonHeader + totalRowSticker * defaultStickerHeight + defaultSpace +
                     vectorSpace * 2 +
                     defaultSpace + GetHeightAddOnCard(cardData.cardState);
        return height;
    }

    private float GetHeightAddOnCard(CardState cardState)
    {
        switch (cardState)
        {
            case CardState.Lock:
            case CardState.Key:
                return 20;
            case CardState.Normal:
            case CardState.Freeze:
            default:
                return 0;
        }
    }
}

public static class DefaultDataCreator
{
    public static ObjHaveStickerData CreateDefaultObjHaveStickerData()
    {
        var e = new ObjHaveStickerData
        {
            objID = 0,
            stickerId = -1
        };
        return e;
    }

    public static LayerCardData CreateDefaultLayerData(int layer = 0)
    {
        var listCard = new List<CardData>();
        var card = CreateDefaultCardData();
        listCard.Add(card);
        var e = new LayerCardData
        {
            layerIndex = layer,
            cards = listCard.ToArray()
        };
        return e;
    }

    public static CardData CreateDefaultCardData()
    {
        var listSticker = new List<StickerData>();
        var sticker = CreateDefaultSticker();
        listSticker.Add(sticker);
        var e = new CardData()
        {
            cardType = CardType.Card1,
            stickers = listSticker.ToArray(),
            position = Vector3.zero
        };
        return e;
    }

    public static StickerData CreateDefaultSticker()
    {
        var e = new StickerData()
        {
            stickerID = -1,
            stickerType = StickerType.Normal
        };
        return e;
    }
}
#endif