using Sirenix.OdinInspector.Editor;
using UnityEngine;
using Sirenix.Utilities;

[CreateAssetMenu(fileName = "CardDesignGlobalConfig", menuName = "GlobalConfigs/CardDesignGlobalConfig")]
[GlobalConfig("Assets/Resources/GlobalConfig/")]
public class CardDesignGlobalConfig : GlobalConfig<CardDesignGlobalConfig>
{
    public CardData cardData;
}

[System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
public sealed class CardDataEditor : System.Attribute
{
}

#if UNITY_EDITOR
public sealed class CardDataEditorAttributeDrawer : OdinAttributeDrawer<CardDataEditor, CardData>
{
    protected override void DrawPropertyLayout(GUIContent label)
    {
    }
}
#endif