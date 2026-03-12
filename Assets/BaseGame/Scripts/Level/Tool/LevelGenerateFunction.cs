#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

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

    [Button("Try Add Card", ButtonSizes.Gigantic)]
    private void TryAddCard()
    {
        var objHaveSticker = LevelData.objHaveStickers;
        var layerData = LevelData.layerCards.ToList();
        var layerCost = objHaveSticker.Length / layerData.Count + ((float)objHaveSticker.Length % (float)layerData.Count > 0 ? 1 : 0);
        var currentLayer = 0;
        var countForNextLayer = 0;
        for (var i = 0; i < objHaveSticker.Length; i++)
        {
            var randomCardForSticker = Random.Range(0, 3000);

            if (countForNextLayer >= layerCost)
            {
                currentLayer++;
                countForNextLayer = 0;
            }
            
            //var randomLayer = Random.Range(0, layerData.Count);
            if (randomCardForSticker < 1000)
            {
                AddCard(CardType.Card3, currentLayer, 1, objHaveSticker[i].stickerId);
            }
            else if (randomCardForSticker < 2000)
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
}

#endif