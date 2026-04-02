#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

public class LevelGenerateFunction : MonoBehaviour
{
    public LevelGenerateText levelGenerateText;
    public LevelData LevelData => levelGenerateText.levelData;
    public int totalSticker = 10;

    [Button("Add Obj Have Sticker", ButtonSizes.Gigantic)]
    private void AddObjHaveSticker()
    {
        var listObjHaveSticker = LevelData.objHaveStickers.ToList();
        for (var i = 0; i < totalSticker; i++)
        {
            var newObjHaveSticker = DefaultDataCreator.CreateDefaultObjHaveStickerData();
            newObjHaveSticker.stickerId = SpriteGlobalConfig.Instance.GetRandomStickerId();
            listObjHaveSticker.Add(newObjHaveSticker);
        }

        levelGenerateText.levelData.objHaveStickers = listObjHaveSticker.ToArray();
    }

    [Button("Remove Obj Have Sticker", ButtonSizes.Gigantic)]
    private void RemoveObjHaveSticker()
    {
        var listObjHaveSticker = LevelData.objHaveStickers.ToList();
        if (listObjHaveSticker.Count >= 0) listObjHaveSticker.Remove(listObjHaveSticker[^1]);
        levelGenerateText.levelData.objHaveStickers = listObjHaveSticker.ToArray();
    }

    public int totalCardAdd = 10;
    public int layerIndex = 0;

    [Button("Add Card", ButtonSizes.Gigantic)]
    private void AddCard()
    {
        if (layerIndex >= LevelData.layerCards.Length)
        {
            AddLayer();
        }

        var listCard = LevelData.layerCards[layerIndex].cards.ToList();
        for (var i = 0; i < totalCardAdd; i++)
        {
            var newCard = DefaultDataCreator.CreateDefaultCardData();
            listCard.Add(newCard);
        }

        LevelData.layerCards[layerIndex].cards = listCard.ToArray();
    }

    private void AddLayer()
    {
        var layerCard = LevelData.layerCards.ToList();
        var newLayerCard = DefaultDataCreator.CreateDefaultLayerCardData(LevelData.layerCards.Length);
        layerCard.Add(newLayerCard);
        LevelData.layerCards = layerCard.ToArray();
    }

    public Percent<CardType> percentCartType = new();
    [Button("Try Add Card", ButtonSizes.Gigantic)]
    private void TryAddCard()
    {
        var objHaveSticker = LevelData.objHaveStickers;
        var layerData = LevelData.layerCards.ToList();
        var layerCost = objHaveSticker.Length / layerData.Count +
                        ((float)objHaveSticker.Length % (float)layerData.Count > 0 ? 1 : 0);
        var currentLayer = 0;
        var countForNextLayer = 0;
        for (var i = 0; i < objHaveSticker.Length; i++)
        {
            //var randomCardForSticker = Random.Range(0, 3000);

            if (countForNextLayer >= layerCost)
            {
                currentLayer++;
                countForNextLayer = 0;
            }

            //var randomLayer = Random.Range(0, layerData.Count);
            var randomCardType = percentCartType.GetRandomType();
            if (randomCardType == CardType.Card3)
            {
                AddCard(CardType.Card3, currentLayer, 1, objHaveSticker[i].stickerId);
            }
            else if (randomCardType == CardType.Card2)
            {
                AddCard(CardType.Card2, currentLayer, 1, objHaveSticker[i].stickerId);
                AddCard(CardType.Card1, currentLayer, 1, objHaveSticker[i].stickerId);
            }
            else
            {
                AddCard(CardType.Card1, currentLayer, 3, objHaveSticker[i].stickerId);
            }

            countForNextLayer++;
        }
    }

    private void AddCard(CardType cardType, int randomLayer, int count, int stickerId)
    {
        var layerData = LevelData.layerCards.ToList();

        for (var i = 0; i < count; i++)
        {
            var cards = layerData[randomLayer].cards.ToList();
            var newCard = DefaultDataCreator.CreateDefaultCardData();
            newCard.cardType = cardType;
            var stickers = new List<StickerData>();
            for (var j = 0; j < LevelDesignHelper.GetTotalStickerOnCard(cardType); j++)
            {
                var newSticker = DefaultDataCreator.CreateDefaultStickerData();
                newSticker.stickerID = stickerId;
                stickers.Add(newSticker);
            }

            newCard.stickers = stickers.ToArray();
            cards.Add(newCard);
            layerData[randomLayer].cards = cards.ToArray();
        }

        LevelData.layerCards = layerData.ToArray();
    }

    [Button]
    public void RandomPosCard()
    {
        if (LevelData == null || LevelData.layerCards == null || LevelData.layerCards.Length == 0)
        {
            Debug.LogWarning("No level data to randomize positions");
            return;
        }

        var rng = new System.Random();
        float minDistance = 2f; // Minimum distance between cards
        int maxAttempts = 100; // Maximum attempts to find a valid position

        for (int i = 0; i < LevelData.layerCards.Length; i++)
        {
            var layer = LevelData.layerCards[i];
            if (layer.cards == null || layer.cards.Length == 0) continue;

            var positions = new List<Vector3>();

            foreach (var card in layer.cards)
            {
                Vector3 newPos = Vector3.zero;
                bool validPosition = false;
            
                for (int attempt = 0; attempt < maxAttempts; attempt++)
                {
                    newPos = new Vector3(
                        (float)(rng.NextDouble() * 10 - 4),
                        (float)(rng.NextDouble() * 10 - 4),
                        0
                    );

                    validPosition = true;
                    foreach (var existingPos in positions)
                    {
                        if (Vector3.Distance(newPos, existingPos) < minDistance)
                        {
                            validPosition = false;
                            break;
                        }
                    }

                    if (validPosition) break;
                }

                card.position = newPos;
                positions.Add(newPos);
            }
        }

        UnityEditor.EditorUtility.SetDirty(levelGenerateText);
    }


    [Button]
    private void CheckLevel()
    {
        for (var i = 0; i < LevelData.layerCards.Length; i++)
        {
            if (!CheckCards(i, LevelData.layerCards[i].cards)) return;
        }

        Debug.Log("Level is valid");
    }

    private bool CheckCards(int layer, CardData[] card)
    {
        for (var i = 0; i < card.Length; i++)
        {
            if (card[i].stickers.Length == 0)
            {
                Debug.LogError($"Card {i} in layer {layer} has no sticker");
                return false;
            }

            if (LevelDesignHelper.GetTotalStickerOnCard(card[i].cardType) != card[i].stickers.Length)
            {
                Debug.LogError($"Card {i} in layer {layer} has wrong number of stickers");
                return false;
            }
        }

        return true;
    }

    public int shuffleLayerIndexStart;
    public int shuffleLayerIndexEnd;

    [Button]
    private void Shuffle()
    {
        if (LevelData == null || LevelData.layerCards == null || LevelData.layerCards.Length == 0)
        {
            Debug.LogWarning("No level data to shuffle");
            return;
        }

        int start = Mathf.Clamp(shuffleLayerIndexStart, 0, LevelData.layerCards.Length - 1);
        int end = Mathf.Clamp(shuffleLayerIndexEnd, 0, LevelData.layerCards.Length - 1);
        if (start > end)
        {
            (start, end) = (end, start);
        }

        var stickerIds = new List<int>();
        var stickerRefs = new List<StickerData>();

        for (int li = start; li <= end; li++)
        {
            var layer = LevelData.layerCards[li];
            if (layer.cards == null) continue;

            for (int ci = 0; ci < layer.cards.Length; ci++)
            {
                var card = layer.cards[ci];
                if (card.stickers == null) continue;

                foreach (var s in card.stickers)
                {
                    stickerRefs.Add(s);
                    stickerIds.Add(s.stickerID);
                }
            }
        }

        if (stickerIds.Count <= 1)
        {
            Debug.Log("Not enough stickers to shuffle");
            return;
        }

        var rng = new System.Random();
        for (int i = stickerIds.Count - 1; i > 0; i--)
        {
            int j = rng.Next(i + 1);
            (stickerIds[i], stickerIds[j]) = (stickerIds[j], stickerIds[i]);
        }

        for (int i = 0; i < stickerRefs.Count; i++)
        {
            stickerRefs[i].stickerID = stickerIds[i];
        }

        UnityEditor.EditorUtility.SetDirty(levelGenerateText);
        Debug.Log($"Shuffled {stickerIds.Count} stickers between layers {start} and {end}");
    }
}

#endif

[Serializable]
public class Percent<T>
{
    [HideLabel]
    public List<PercentElement<T>> elements;

    public T GetRandomType()
    {
        if (elements == null || elements.Count == 0)
        {
            Debug.LogWarning("Percent list is empty");
            return default;
        }

        var totalPercent = elements.Sum(e => e.percent);
        var randomValue = Random.Range(0, totalPercent);
        var cumulativePercent = 0f;

        foreach (var element in elements)
        {
            cumulativePercent += element.percent;
            if (randomValue <= cumulativePercent)
            {
                return element.type;
            }
        }

        // Fallback in case of rounding errors
        return elements.Last().type;
    }
}
[Serializable]
public class PercentElement<T>
{
    public T type;
    public float percent;
}
