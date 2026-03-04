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
        var totalSticker = LevelData.objHaveStickers.Length * 3;
        
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
