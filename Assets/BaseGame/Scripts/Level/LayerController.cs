using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

[Serializable]
public class LayerController
{
    public Transform trsLayerParents;
    public List<Card> cards;
    [field: SerializeField] public Reactive<int> layerActive { get; set; } = new(0);
    public bool loadDone;
    private int totalCards;
    public async UniTask LoadData(LayerCardData[] data)
    {
        totalCards = 0;
        for (var i = data.Length - 1; i >= 0 ; i--)
        {
            LoadCardInLayer(i, data[i].cards);
        }

        loadDone = true;
    }

    private void LoadCardInLayer(int layerIndex, CardData[] data)
    {
        for (var i = 0; i < data.Length; i++)
        {
            var card = PoolManager.Instance.SpawnCard(data[i].cardType, data[i].position);
            card.InitData(data[i], layerIndex);
            card.AnimFirstSpawn(totalCards);
            cards.Add(card);
            totalCards++;
        }
    }
    
    public void RemoveCard(Card card)
    {
        cards.Remove(card);
    }

    public void ResetController()
    {
        Debug.Log("reset layer controller");
        layerActive.Value = 0;
        loadDone = false;
    }

    public void NextLayer()
    {
        for (var i = cards.Count - 1; i >= 0 ; i--)
        {
            if (cards[i].layerIndex == layerActive.Value && !cards[i].IsDone())
            {
                return;
            }

            if (cards[i].layerIndex > layerActive.Value)
                break;
        }

        layerActive.Value++;
        Debug.Log($"Change layer {layerActive.Value}");
    }

    public void OnRemoveSticker(int stickerId, int countRemove)
    {
        var count = countRemove;
        for (var i = 0; i < cards.Count; i++)
        {
            count = cards[i].IsHaveSticker(stickerId, count);

            if (count == 0)
                break;

        }
    }
}
