using System;
using System.Collections.Generic;
using Cysharp.Text;
using TMPro;
using UnityEngine;

public static class MyCache
{
    public static string textFormatFloat = "0:F2";
    public static string strProgress = "{0}/{1}";
    public static string strDefault = "{0}";
    public static string strLevel = "Level {0}";
    public static string strLock = "lock";
    public static string strUnlockNormal = "unlock_normal";
    public static string strUnlockHammer = "unlock_hammer";
    private static Dictionary<GameResource.Type, TMP_Style> resourceStyleCache = new();
    public static GameResource.Type ConvertBoosterToResourceType(BoosterType boosterType)
    {
        return boosterType switch
        {
            BoosterType.Magnet => GameResource.Type.BoosterMagnet,
            BoosterType.AddSlot => GameResource.Type.BoosterAddSlot,
            BoosterType.Hammer => GameResource.Type.BoosterHammer,
            _ => GameResource.Type.None
        };
    }
    
    public static string ConvertBoosterToResourceType(SlotTabType tabType)
    {
        return tabType switch
        {
            SlotTabType.None => ZString.Concat("None"),
            SlotTabType.Shop => ZString.Concat("Shop"),
            SlotTabType.Home => ZString.Concat("Home"),
            SlotTabType.ComingSoon => ZString.Concat("Coming Soon"),
            _ =>""
        };
    }


    public static TMP_Style GetTextResourceStyle(GameResource.Type type)
    {
        if (resourceStyleCache.TryGetValue(type, out var style))
        {
            return style;
        }

        var tmpStyle = TMP_Settings.defaultStyleSheet.GetStyle($"{type.ToString()}");
        resourceStyleCache.Add(type, tmpStyle);
        return tmpStyle;
    }
}
