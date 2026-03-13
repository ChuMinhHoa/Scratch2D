using UnityEngine;

public static class MyCache
{
    public static string strProgress = "{0}/{1}";
    public static string strDefault = "{0}";
    public static string strLevel = "Level {0}";
    
    public static GameResource.Type ConvertBoosterToResourceType(BoosterType boosterType)
    {
        return boosterType switch
        {
            BoosterType.Magnet => GameResource.Type.BoosterMagnet,
            BoosterType.AddSlot => GameResource.Type.BoosterAddSlot,
            _ => GameResource.Type.None
        };
    }
}
