using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using LitMotion;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;

[Serializable]
public class MainContentBase<TSlot, TData> where TSlot : SlotBase<TData>
{
    public Action<TSlot> actionSlotCallBack;
    public Transform trsContentParents;
    public List<TSlot> slots;
    [HideInInspector] public TSlot currentSlotOnChoose;

    [HideInInspector] public int totalSlotUsing;

    public virtual void InitData(Span<TData> data)
    {
        totalSlotUsing = 0;
        for (var i = 0; i < slots.Count; i++)
        {
            if (slots[i].gameObject.activeSelf)
            {
                slots[i].gameObject.SetActive(false);
            }
        }
        
        for (var i = 0; i < data.Length; i++)
        {
            if (data[i]!=null)
            {
                AddSlot(data[i]);
            }   
        }

        DeActiveSlotOut();
    }
    
    public virtual void InitDataWithSlotExist(Span<TData> data, int startIndex = 0)
    {
        totalSlotUsing = 0;

//        Debug.Log($"{startIndex} + {startIndex + data.Length}");
        
        for (var i = startIndex; i < data.Length + startIndex; i++)
        {
            if (i < slots.Count)
            {
                slots[i].gameObject.SetActive(true);
                slots[i].InitData(data[totalSlotUsing]);
                slots[i].SetActionChooseCallBack(slotBase => ActionSlotCallBack((TSlot)slotBase));
                totalSlotUsing++;
            }
            else
                break;
        }

        //DeActiveSlotOut();
    }

    public virtual void AddSlot(TData data)
    {
        for (var i = 0; i < slots.Count; i++)
        {
            if (slots[i].gameObject.activeSelf) continue;
            slots[i].gameObject.SetActive(true);
            slots[i].InitData(data);
            slots[i].SetActionChooseCallBack(slotBase => ActionSlotCallBack((TSlot)slotBase));
            totalSlotUsing++;
            return;
        }

        var newSlot = UIPoolManager.Instance.SpawnUISlot<TSlot, TData>(trsContentParents);
        slots.Add(newSlot);
        newSlot.InitData(data);
        newSlot.SetActionChooseCallBack(slotBase => ActionSlotCallBack((TSlot)slotBase));
        totalSlotUsing++;
    }

    public void ActionSlotCallBack(TSlot slotCallBack)
    {
        currentSlotOnChoose = slotCallBack;
        actionSlotCallBack?.Invoke(currentSlotOnChoose);
    }

    public virtual void DeActiveSlotOut()
    {
        for (var i = totalSlotUsing; i < slots.Count; i++)
        {
            if (slots[i].gameObject.activeSelf)
                slots[i].gameObject.SetActive(false);
        }
    }

    public virtual void SetActionSlotCallBack(Action<TSlot> actionCallBack)
    {
        actionSlotCallBack = actionCallBack;
    }

    private CancellationTokenSource cancellationToken;
    
    public virtual async UniTask AnimationSlot(int countSlotShow)
    {
        cancellationToken = new CancellationTokenSource();
        for (var i = 0; i < countSlotShow; i++)
        {
            if (i < slots.Count)
            {
                _ = slots[i].AnimOpen();
                await UniTask.Delay(150, cancellationToken: cancellationToken.Token);
            }
        }
    }
    
    public void DeSpawnAllSlot()
    {
        for (var i = 0; i < slots.Count; i++)
        {
            UIPoolManager.Instance.DeSpawnUISlot(slots[i]);
        }
        slots.Clear();
        totalSlotUsing = 0;
    }
}