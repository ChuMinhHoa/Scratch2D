using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

[Serializable]
public class LayerController
{
    public Transform trsLayerParents;
    public List<Card> cards;
    [field: SerializeField] public Reactive<int> layerActive { get; set; } = new(0);
    public bool loadDone;
    public void LoadData(Span<LayerCardData> data)
    {
        for (var i = 0; i < data.Length; i++)
        {
            LoadCardInLayer(i, data[i].cards);
        }

        loadDone = true;
    }

    private void LoadCardInLayer(int layerIndex, Span<CardData> data)
    {
        for (var i = 0; i < data.Length; i++)
        {
            var card = PoolManager.Instance.SpawnCard(data[i].position);
            card.LoadData(data[i], layerIndex);
            card.AnimFirstSpawn(i);
            cards.Add(card);
        }
    }

    public void ResetController()
    {
        layerActive.Value = 0;
    }

    public void NextLayer()
    {
        for (var i = 0; i < cards.Count; i++)
        {
            if (cards[i].layerIndex == layerActive.Value && !cards[i].IsDone())
            {
                return;
            }

            if (cards[i].layerIndex > layerActive.Value)
                break;
        }

        layerActive.Value++;
    }
}
