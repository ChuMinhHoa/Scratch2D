using System;
using R3;
using UniRx;
using UnityEngine;

public class LineSlotLevel : MonoBehaviour
{
    public SlotLevel[] slotLevels;

    private Reactive<int> levelIndex = new(0);

    private void Start()
    {
        levelIndex = Level.Instance.levelIndex;
        levelIndex.Subscribe(ChangeLevel).AddTo(this);
    }

    private void ChangeLevel(int levelChange)
    {
        for (var i = 0; i < slotLevels.Length; i++)
        {
            slotLevels[i].InitData(levelIndex + 1 + i);
        }
    }

}
