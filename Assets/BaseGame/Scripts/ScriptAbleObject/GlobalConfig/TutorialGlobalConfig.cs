using System;
using System.Collections.Generic;
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
            var e = TutorialManager.Instance.IsShowThatTutorial(listTutorialConfig[i].levelShow);
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
    public int levelShow;
    public string tutorialName;
    public string tutorialDes;
    public Sprite spriteIcon;
    public SkeletonData skeletonData;
    [SerializeReference] public TutorialHandAction handAction;
}