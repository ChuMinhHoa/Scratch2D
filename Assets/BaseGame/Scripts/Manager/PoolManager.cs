using System.Collections.Generic;
using ScratchCardAsset;
using TW.Utility.DesignPattern;
using UnityEngine;

public class PoolManager : Singleton<PoolManager>
{
    
    void Start()
    {
        poolEraserEffect.SpawnOnInit();
        poolStickerMoveEffect.SpawnOnInit();
    }

    #region Eraser effect

    public HPool<ParticleSystem> poolEraserEffect;
    
    public ParticleSystem SpawnEraserEffect(Transform parent)
    {
        var obj = poolEraserEffect.Spawn();
        if (!obj) return null;
        obj.transform.position = parent.position;
        return obj;
    }
    
    public void DespawnEraserEffect(ParticleSystem obj)
    {
        poolEraserEffect.Despawn(obj);
    }

    #endregion
    
    #region Sticker Move Effect

    public HPool<StickerDone> poolStickerMoveEffect;
    
    public StickerDone SpawnStickerDone(Transform parent)
    {
        var obj = poolStickerMoveEffect.Spawn();
        if (!obj) return null;
        obj.transform.position = parent.position;
        obj.transform.localScale = Vector3.one;
        return obj;
    }
    
    public void DespawnStickerMove(StickerDone obj)
    {
        poolStickerMoveEffect.Despawn(obj);
    }

    #endregion

    #region FolderHaveSticker
    
    public HPool<FolderHaveSticker> poolObjHaveSticker;
    public FolderHaveSticker SpawnObjHaveSticker()
    {
        var obj = poolObjHaveSticker.Spawn();
        return !obj ? null : obj;
    }
    
    public void DespawnObjHaveSticker(FolderHaveSticker folder)
    {
        poolObjHaveSticker.Despawn(folder);
    }

    #endregion

    #region Card
    public HPool<Card>[] poolCardData;

    public Card SpawnCard(CardType cardType, Vector3 position)
    {
        for (var i = 0; i < poolCardData.Length; i++)
        {
            if (poolCardData[i].prefab.cardType == cardType)
            {
                var e = poolCardData[i].Spawn();
                e.transform.position = position;
                return e;
            }
        }

        return null;
    }
    
    public void DespawnCard(Card obj)
    {
        for (int i = 0; i < poolCardData.Length; i++)
        {
            if (poolCardData[i].prefab.cardType == obj.cardType)
            {
                poolCardData[i].Despawn(obj);
                return;
            }
        }
    }

    // public HPool<Card> poolCard;
    //
    // public Card SpawnCard(Vector3 position)
    // {
    //     var obj = poolCard.Spawn();
    //     if (!obj) return null;
    //     obj.transform.position = position;
    //     return obj;
    // }
    //
    // public void DespawnCard(Card obj)
    // {
    //     poolCard.Despawn(obj);
    // }
    #endregion

    #region Sticker

    public HPool<Sticker> poolSticker;
    
    public Sticker SpawnSticker()
    {
        var obj = poolSticker.Spawn();
        if (!obj) return null;
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localScale = Vector3.one;
        return obj;
    }
    
    public void DespawnSticker(Sticker obj)
    {
        poolSticker.Despawn(obj);
    }

    #endregion
    #region Scratch Card

    public HPool<ScratchObject> poolScratchCard;
    
    public ScratchObject SpawnScratchManager()
    {
        var obj = poolScratchCard.Spawn();
        if (!obj) return null;
        //obj.transform.SetParent(parents);
        obj.transform.localScale = Vector3.one;
        return obj;
    }
    
    public void DespawnScratch(ScratchObject obj)
    {
        poolScratchCard.Despawn(obj);
    }

    #endregion
}

[System.Serializable]
public class HPool<T> where T : Component
{
    public T prefab;
    public int amount;
    public Transform parents;
    //public List<T> activePool = new();
    public List<T> deActivePool = new();

    public void SpawnOnInit()
    {
        for (var i = 0; i < amount; i++)
        {
            var obj = Object.Instantiate(prefab, parents);
            obj.gameObject.SetActive(false);
            deActivePool.Add(obj);
        }
    }
    
    public void SpawnOnInit(int am)
    {
        for (var i = 0; i < am; i++)
        {
            var obj = Object.Instantiate(prefab, parents);
            obj.gameObject.SetActive(true);
            deActivePool.Add(obj);
        }
    }

    public T Spawn()
    {
        for (var i = deActivePool.Count - 1; i >= 0 ; i--)
        {
            if (!deActivePool[i])
                deActivePool.RemoveAt(i);
        }
        if (deActivePool.Count > 0)
        {
            var e = deActivePool[0];
            deActivePool.Remove(e);
            //activePool.Add(e);
            if (!e.gameObject.activeSelf)
                e.gameObject.SetActive(true);
            return e;
        }

        if (!parents)
            return null;
        var obj = Object.Instantiate(prefab, parents);
        //activePool.Add(obj);
        return obj;
    }
    
    public void Despawn(T obj)
    {
        if (!obj) return;
        if (deActivePool.Contains(obj)) return;
        //activePool.Remove(obj);
        if (obj.transform.parent != parents)
            obj.transform.SetParent(parents);
        obj.gameObject.SetActive(false);
        deActivePool.Add(obj);
    }
}

[System.Serializable]
public class CPool<T> where T : MonoBehaviour
{
    public T prefab;
    public int amount;
    public Transform parents;
    //public List<T> activePool = new();
    public List<T> deActivePool = new();

    public void SpawnOnInit()
    {
        for (var i = 0; i < amount; i++)
        {
            var obj = Object.Instantiate(prefab, parents);
            obj.gameObject.SetActive(false);
            deActivePool.Add(obj);
        }
    }

    public T Spawn()
    {
        if (deActivePool.Count > 0)
        {
            var e = deActivePool[0];
            deActivePool.Remove(e);
            //activePool.Add(e);
            if (!e.gameObject.activeSelf)
                e.gameObject.SetActive(true);
            return e;
        }

        if (parents == null)
            return null;
        var obj = Object.Instantiate(prefab, parents);
        //activePool.Add(obj);
        return obj;
    }
    
    public void Despawn(T obj)
    {
        if (obj == null) return;
        //if (!activePool.Contains(obj)) return;
        //activePool.Remove(obj);
        if (obj.transform.parent != parents)
            obj.transform.SetParent(parents);
        obj.gameObject.SetActive(false);
        deActivePool.Add(obj);
    }
}
