using System;
using UnityEngine;

public static class LevelDesignHelper
{

    public static int GetTotalStickerOnCard(CardType cardType)
    {
        var cardTypeValue = (int)cardType;

        for (var i = 0; i < Enum.GetNames(typeof(CardType)).Length; i++)
        {
            if (cardTypeValue < (i + 1) * 100)
            {
                return i + 1;
            }
        }
        
        return -1;
    }
}
