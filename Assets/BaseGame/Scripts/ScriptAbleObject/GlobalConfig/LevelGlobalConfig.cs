using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Sirenix.Utilities;
using UnityEditor;
using UnityEngine.AddressableAssets;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "LevelGlobalConfig", menuName = "GlobalConfigs/LevelGlobalConfig")]
[GlobalConfig("Assets/Resources/GlobalConfig/")]
public class LevelGlobalConfig : GlobalConfig<LevelGlobalConfig>
{
    public LevelConfig[] levelConfigs;

    public LevelConfig GetLevelConfig(int level)
    {
        if (level >= levelConfigs.Length)
        {
            var levelDifficulty = level % 10 == 0 ? Difficulty.Hard :
                level % 10 == 5 ? Difficulty.Medium : Difficulty.Easy;
            return GetRandomLevelConfig(level, levelDifficulty);
        }

        return levelConfigs[level];
    }

    private LevelConfig GetRandomLevelConfig(int currentLevel, Difficulty levelDifficulty)
    {
        var limitMin = Mathf.Clamp(currentLevel - 50, 0, levelConfigs.Length);
        var limitMax = Mathf.Clamp(currentLevel + 50, 0, levelConfigs.Length);
        var levelRandom = -1;
        Span<int> levelCandidates = stackalloc int[100];
        var index = 0;
        switch (levelDifficulty)
        {
            case Difficulty.Easy:
                levelRandom = Random.Range(limitMin, limitMax);
                break;
            case Difficulty.Medium:
                levelCandidates.Clear();
                for (var i = limitMin; i < limitMax; i++)
                {
                    if (i % 5 == 0 && i % 10 != 0)
                    {
                        levelCandidates[index] = i;
                        index++;
                    }
                }
                levelRandom = levelCandidates.Length > 0
                    ? levelCandidates[Random.Range(0, index)]
                    : Random.Range(limitMin, limitMax);
                break;
            case Difficulty.Hard:
                for (var i = limitMin; i < limitMax; i++)
                {
                    if (i % 10 == 0)
                    {
                        index++;
                        levelCandidates[index] = i;
                    }
                }
                levelRandom = levelCandidates.Length > 0
                    ? levelCandidates[Random.Range(0, index)]
                    : Random.Range(limitMin, limitMax);
                break;
            default:
                break;
        }

        return levelConfigs[levelRandom];
    }

#if UNITY_EDITOR
    [Button]
    private void GetLevelConfig()
    {
        var path = @"Assets\BaseGame\TextAssets\LevelData\";
        for (var i = 0; i < levelConfigs.Length; i++)
        {
            levelConfigs[i].level = i;
            var fullPath = path + "Level_" + levelConfigs[i].level + ".txt";
            var asset = AssetDatabase.LoadAssetAtPath(fullPath, typeof(TextAsset));
            if (asset != null)
            {
                levelConfigs[i].levelAsset = asset as TextAsset;
            }
            else
            {
                Debug.Log($"Null level asset for level{levelConfigs[i].level}");
            }
        }
    }
    
#endif
}

[Serializable]
public class LevelConfig
{
    public int level;
    public Difficulty difficulty;
    public TextAsset levelAsset;
}

public enum Difficulty
{
    Easy,
    Medium,
    Hard
}