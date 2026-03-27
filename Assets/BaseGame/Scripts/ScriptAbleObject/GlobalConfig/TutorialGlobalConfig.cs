using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Sirenix.Utilities;
using Spine;

[CreateAssetMenu(fileName = "TutorialGlobalConfig", menuName = "GlobalConfigs/TutorialGlobalConfig")]
[GlobalConfig("Assets/Resources/GlobalConfig/")]
public class TutorialGlobalConfig : GlobalConfig<TutorialGlobalConfig>
{
    public TutorialConfig[] listTutorialConfig;

    public TutorialConfig GetTutorialConfig(int level)
    {
        for (var i = 0; i < listTutorialConfig.Length; i++)
        {
            var e = TutorialManager.Instance.IsShowedThatTutorial(listTutorialConfig[i].id);
            if (!e && listTutorialConfig[i].levelShow <= level)
            {
                return listTutorialConfig[i];
            }
        }

        return null;
    }
}

[Serializable]
public class TutorialConfig
{
    [PreviewField, HideLabel]
    public Sprite spriteIcon;
    public TutorialType tutorialType;
    public Sprite[] sprSubIcon;
    public int id;
    public int levelShow;
    public string tutorialName;
    [TextArea]
    public string tutorialDes;
    public SkeletonData skeletonData;
    [SerializeReference] public TutorialHandAction handAction;
}

public enum TutorialType
{
    None,
    Scratch,
    BoosterAddSlot,
    BoosterAddNote,
    BoosterHammer,
    BoosterMagnet,
}