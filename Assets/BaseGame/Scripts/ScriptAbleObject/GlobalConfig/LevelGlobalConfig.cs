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
        return levelConfigs[level];
    }
    
    public int GetRandomLevel(int levelIndexValue)
    {
        var realLevel = levelIndexValue + 1;
        var levelDifficulty = realLevel % 10 == 0 ? Difficulty.Hard :
            realLevel % 10 == 5 ? Difficulty.Medium : Difficulty.Easy;
        var limitMin = 10;
        var limitMax = levelConfigs.Length - 1;
        Span<int> levelCandidates = stackalloc int[100];
        var index = 0;
        var levelRandom = -1;
        switch (levelDifficulty)
        {
            case Difficulty.Easy:
                Debug.Log("get random level easy");
                levelRandom = Random.Range(limitMin, limitMax);
                break;
            case Difficulty.Medium:
                Debug.Log("get random level medium");
                levelCandidates.Clear();
                for (var i = limitMin; i < limitMax; i++)
                {
                    if (i % 5 == 0 && i % 10 != 0)
                    {
                        levelCandidates[index] = i;
                        index++;
                    }
                }
                levelRandom = index > 0
                    ? levelCandidates[Random.Range(0, index)]
                    : Random.Range(limitMin, limitMax);
                break;
            case Difficulty.Hard:
                Debug.Log("get random level hard");
                for (var i = limitMin; i < limitMax; i++)
                {
                    if (i % 10 == 0)
                    {
                        levelCandidates[index] = i;
                        index++;
                    }
                }

                var randomIndex = Random.Range(0, index);
                levelRandom = index > 0
                    ? levelCandidates[randomIndex]
                    : Random.Range(limitMin, limitMax);
                break;
        }
        Debug.Log($"levelRandom {levelRandom}");
        return levelRandom;
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