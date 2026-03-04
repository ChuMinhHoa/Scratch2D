using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

public static class LevelDesignHelper
{
    public static int GetTotalStickerOnCard(CardType cardType)
    {
        var cardTypeValue = (int)cardType;

        for (var i = 0; i < Enum.GetNames(typeof(CardType)).Length; i++)
        {
            if (cardTypeValue < (i + 1) * 100)
            {
                return i + 1;
            }
        }

        return -1;
    }

    private static LevelData LevelData => LevelDesignGlobalConfig.Instance.CurrentLevel.LevelData;

    public static float defaultHeightButtonHeader = 25;
    public static float defaultStickerHeight = 70;
    public static float defaultStickerWidth = 90f;
    public static float defaultSpace = 5;
    public static float vectorSpace = 20;
    public static int totalStickerInRow = 3;
    public static int totalCardInRow = 3;

    #region Get Height

    public static float GetHeightTotalLayer()
    {
        var totalHeight = 0f;
        for (var i = 0; i < LevelData.layerCards.Length; i++)
        {
            totalHeight += GetHeightLayer(i);
        }

        return totalHeight + defaultSpace;
    }

    public static float GetHeightLayer(int layerIndex)
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

    public static float GetMaxHeightCard(int layerIndex, int rowIndex)
    {
        var layerData = LevelData.layerCards[layerIndex];

        var maxY = 0f;
        var start = rowIndex * 3;
        for (var i = start; i < start + 3; i++)
        {
            var rectCard = GetHeightCard(layerData.cards[i]);

            if (maxY < rectCard)
            {
                maxY = rectCard;
            }
        }

        return maxY;
    }

    private static float GetHeightCard(CardData cardData)
    {
        var totalSticker = cardData.stickers.Length;
        var totalRowSticker = Mathf.CeilToInt((float)totalSticker / totalStickerInRow);
        var height = defaultHeightButtonHeader + totalRowSticker * defaultStickerHeight + defaultSpace +
                     vectorSpace * 2 +
                     defaultSpace + GetHeightAddOnCard(cardData.cardState);
        return height;
    }

    private static float GetHeightAddOnCard(CardState cardState)
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

    #endregion

    #region Get Width

    public static float GetWidthCard(StickerData[] stickerData)
    {
        var totalWidth = 0f;
        var widthOfRow = 0f;
        var countSticker = 0;
        for (var i = 0; i < stickerData.Length; i++)
        {
            var stickerWidth = GetStickerWidth(stickerData[i]);

            widthOfRow += stickerWidth;
            countSticker++;
            if ((countSticker == totalStickerInRow) || i == stickerData.Length - 1)
            {
                if (countSticker < totalStickerInRow)
                {
                    widthOfRow += defaultStickerWidth * (totalStickerInRow - countSticker);
                }

                if (widthOfRow > totalWidth)
                {
                    totalWidth = widthOfRow;
                }

                widthOfRow = 0;
                countSticker = 0;
            }
        }

        return totalWidth + defaultSpace * 2;
    }

    public static float GetStickerWidth(StickerData stickerData)
    {
        var stickerWidth = defaultStickerWidth + GetWidthAddOnSticker(stickerData.stickerType);
        return stickerWidth;
    }

    private static float GetWidthAddOnSticker(StickerType stickerType)
    {
        switch (stickerType)
        {
            case StickerType.Chain:
                return 20;
            case StickerType.Normal:
            default:
                return 0;
        }
    }

    #endregion

    public static void DrawChainAddOnSticker(Rect rectAddOn, StickerData stickerData)
    {
        SirenixEditorGUI.DrawBorders(rectAddOn, 1);
        var rectLabel = new Rect(rectAddOn.x, rectAddOn.y, rectAddOn.width, 20);
        SirenixEditorGUI.DrawSolidRect(rectLabel, Color.gray5);
        EditorGUI.LabelField(rectLabel, "C");
        var stickerIndexChain = stickerData.stickerIndexChain;
        var rectIntField = new Rect(rectAddOn.x, rectAddOn.y + 20, rectAddOn.width, rectAddOn.height - 20);
        stickerIndexChain = EditorGUI.IntField(rectIntField, stickerIndexChain);
        stickerData.stickerIndexChain = stickerIndexChain;
    }


    private static int dragSourceLayerIndex = -1;
    private static int dragSourceCardIndex = -1;
    private static bool isDragging = false;

    public static void HandleCardDragAndDrop(Rect rect, int layerIndex, int cardIndex, LevelData levelData)
    {
        Event evt = Event.current;

        if (!rect.Contains(evt.mousePosition))
        {
            return;
        }

        switch (evt.type)
        {
            case EventType.MouseDrag:
                // Chỉ bắt đầu drag khi kéo chuột, không phải click
                if (evt.button == 0 && !isDragging)
                {
                    dragSourceLayerIndex = layerIndex;
                    dragSourceCardIndex = cardIndex;
                    isDragging = true;

                    DragAndDrop.PrepareStartDrag();
                    DragAndDrop.SetGenericData("CardDrag", true);
                    DragAndDrop.StartDrag("Dragging Card");
                    evt.Use();
                }

                break;

            case EventType.DragUpdated:
                if (isDragging)
                {
                    DragAndDrop.visualMode = DragAndDropVisualMode.Move;
                    evt.Use();
                }

                break;

            case EventType.DragPerform:
                if (isDragging)
                {
                    DragAndDrop.AcceptDrag();

                    if (dragSourceLayerIndex != layerIndex || dragSourceCardIndex != cardIndex)
                    {
                        var sourceCards = levelData.layerCards[dragSourceLayerIndex].cards.ToList();
                        var movedCard = sourceCards[dragSourceCardIndex];
                        sourceCards.RemoveAt(dragSourceCardIndex);
                        levelData.layerCards[dragSourceLayerIndex].cards = sourceCards.ToArray();

                        var targetCards = levelData.layerCards[layerIndex].cards.ToList();
                        int insertIndex = Mathf.Clamp(cardIndex, 0, targetCards.Count);
                        targetCards.Insert(insertIndex, movedCard);
                        levelData.layerCards[layerIndex].cards = targetCards.ToArray();
                    }

                    ResetDragState();
                    evt.Use();
                }

                break;

            case EventType.DragExited:
                ResetDragState();
                break;
        }

        // Highlight drop target
        if (isDragging && rect.Contains(evt.mousePosition) &&
            (dragSourceLayerIndex != layerIndex || dragSourceCardIndex != cardIndex))
        {
            SirenixEditorGUI.DrawSolidRect(rect, new Color(0.2f, 0.8f, 0.2f, 0.3f));
        }
    }

    public static void HandleLayerDrop(Rect rect, int layerIndex, LevelData levelData)
    {
        Event evt = Event.current;
        if (evt.type == EventType.DragUpdated && rect.Contains(evt.mousePosition) && isDragging)
        {
            DragAndDrop.visualMode = DragAndDropVisualMode.Move;
            evt.Use();
        }
        else if (evt.type == EventType.DragPerform && rect.Contains(evt.mousePosition) && isDragging)
        {
            DragAndDrop.AcceptDrag();
            if (dragSourceLayerIndex != layerIndex)
            {
                var sourceCards = levelData.layerCards[dragSourceLayerIndex].cards.ToList();
                var movedCard = sourceCards[dragSourceCardIndex];
                sourceCards.RemoveAt(dragSourceCardIndex);
                levelData.layerCards[dragSourceLayerIndex].cards = sourceCards.ToArray();

                var targetCards = levelData.layerCards[layerIndex].cards.ToList();
                targetCards.Add(movedCard);
                levelData.layerCards[layerIndex].cards = targetCards.ToArray();
            }

            ResetDragState();
            evt.Use();
        }
    }

    private static void ResetDragState()
    {
        isDragging = false;
        dragSourceLayerIndex = -1;
        dragSourceCardIndex = -1;
    }
}