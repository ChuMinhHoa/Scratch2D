using System.Collections.Generic;
using TW.Utility.DesignPattern;
using UnityEngine;

public class UIPoolManager : Singleton<UIPoolManager>
{
    private void Start()
    {
        for (var i = 0; i < slotBasePool.Length; i++)
        {
            slotBasePool[i].SpawnOnInit();
        }
    }

    public UIPool<MonoBehaviour>[] slotBasePool;
    public TSlot SpawnUISlot<TSlot, TData>(Transform trsParents) where TSlot : SlotBase<TData>
    {
        for (var i = 0; i < slotBasePool.Length; i++)
        {
            if (slotBasePool[i].prefab is not TSlot) continue;
            var s = slotBasePool[i].Spawn() as TSlot;
            if (s == null) continue;
            s.transform.SetParent(trsParents);
            s.transform.localScale = Vector3.one;
            return s;
        }
        return null;
    }
    
    public void DeSpawnUISlot<TSlot>(TSlot slotDeSpawn)
    {
        if (slotBasePool == null)
            return;
        for (var i = 0; i < slotBasePool.Length; i++)
        {
            if (slotBasePool[i].prefab is not TSlot) continue;
            slotBasePool[i].Despawn(slotDeSpawn as MonoBehaviour);
            return;
        }
    }
}

[System.Serializable]
public class UIPool<T> where T : MonoBehaviour
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
