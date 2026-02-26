using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LayerController
{
    public Transform trsLayerParents;
    public List<Card> cards;
    public Reactive<int> layerActive = new(0);
    
    public void LoadData(Span<LayerCardData> data)
    {
        for (var i = 0; i < data.Length; i++)
        {
            // var layer = PoolManager.Instance.SpawnLayerSticker(trsLayerParents);
            // layerStickers.Add(layer);
            // layer.LoadData(data[i].cards);

            LoadCardInLayer(i, data[i].cards);
        }
    }

    private void LoadCardInLayer(int layerIndex, Span<CardData> data)
    {
        for (var i = 0; i < data.Length; i++)
        {
            var card = PoolManager.Instance.SpawnCard(data[i].position);
            card.LoadData(data[i], layerIndex);
            cards.Add(card);
        }
    }

    public void ResetController()
    {
        layerActive.Value = 0;
    }
}
