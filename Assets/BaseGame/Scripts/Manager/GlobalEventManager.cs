using System;
using UnityEngine;

public static class GlobalEventManager
{
    public static Action CheckToCallNextSticker { get; set; }
    public static Action<BoosterType, IBooster> OnBoosterUsing { get; set; }
    public static Action<int, int> OnRemoveSticker { get; set; }
    public static Action OnBoosterDone { get; set; }
    public static Action OnHaveCardDone { get; set; }
}