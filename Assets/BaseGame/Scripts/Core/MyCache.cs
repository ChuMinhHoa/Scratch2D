using System;
using System.Collections.Generic;
using Cysharp.Text;
using TMPro;
using UnityEngine;

public static class MyCache
{
    public static string strActive = "animation";
    public static string textFormatFloat = "0:F2";
    public static string strProgress = "{0}/{1}";
    public static string strDefault = "{0}";
    public static string strAdd = "+{0}";
    public static string strMultiple = "x{0}";
    public static string strEnergy = "+{0}h";
    public static string strLevel = "Level {0}";
    public static string strLock = "lock";
    public static string strUnlockNormal = "unlock_normal";
    public static string strUnlockHammer = "unlock_hammer";
    public static string strBasePackID = "com.abi.vn.";
    private static Dictionary<GameResource.Type, TMP_Style> resourceStyleCache = new();
    public static string warningPrice = "Not enough currency!";
    public static string warningSlotAdded = "Slots has reached its limit!";
    public static string warningNoteOnMove = "Wait note move!";
    public static string warningNoStickerOnFS = "No sticker wait!";
    public static string warningHammer = "No cards are locked or frozen!";

    public static GameResource.Type ConvertBoosterToResourceType(BoosterType boosterType)
    {
        return boosterType switch
        {
            BoosterType.BoosterMagnet => GameResource.Type.BoosterMagnet,
            BoosterType.BoosterAddSlot => GameResource.Type.BoosterAddSlot,
            BoosterType.BoosterHammer => GameResource.Type.BoosterHammer,
            BoosterType.BoosterCart => GameResource.Type.BoosterCart,
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
            SlotTabType.Settings => ZString.Concat("Settings"),
            _ => ""
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

    public static Difficulty GetDifficultByLevel(int realLevel)
    {
        //Debug.Log("real level: " + realLevel);
        var levelDifficulty = realLevel % 10 == 0 ? Difficulty.Hard :
            realLevel % 10 == 5 ? Difficulty.Medium : Difficulty.Easy;
        return levelDifficulty;
    }

    public static string GetPackageIdByPackageName(PackageName slotDataPackageName)
    {
        return strBasePackID + slotDataPackageName;
    }

    public static string GetFormat(GameResource.Type dataResourceType)
    {
        switch (dataResourceType)
        {
            case GameResource.Type.BoosterAddSlot:
            case GameResource.Type.BoosterHammer:
            case GameResource.Type.BoosterMagnet:
            case GameResource.Type.BoosterCart:
                return strMultiple;
            case GameResource.Type.Gem:
            case GameResource.Type.Money:
                return strAdd;
            case GameResource.Type.Energy:
                return strEnergy;
            default:
                return strAdd;
            }
        }
    }