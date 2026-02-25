using System.Collections.Generic;
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
        if (obj == null) return null;
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
        if (obj == null) return null;
        obj.transform.position = parent.position;
        return obj;
    }
    
    public void DespawnStickerMove(StickerDone obj)
    {
        poolStickerMoveEffect.Despawn(obj);
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
