using System;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SlotBase<TData> : MonoBehaviour
{
    public TData slotData;
    public Image imgIcon;
    public Button btnChoose;
    public Transform trsContent;
    
    private Action<SlotBase<TData>> actionChooseCallBack;
    
    public void SetActionChooseCallBack(Action<SlotBase<TData>> actionCallBack) =>
        actionChooseCallBack = actionCallBack;

    public virtual void Awake() {
        if (btnChoose != null)
            btnChoose.onClick.AddListener(OnChoose);
    }

    public virtual void InitData(TData data) {
        slotData = data;
    }

    public virtual void ReloadData() { 
        InitData(slotData);
    }

    public virtual void OnChoose()
    {
        actionChooseCallBack?.Invoke(this);
    }

    public virtual async UniTask AnimOpen()
    {
        await UniTask.CompletedTask;
    }
    public virtual void AnimDone() { }
}