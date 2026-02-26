using System.IO;
using CoreData;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

public class LevelGenerateText : MonoBehaviour
{
    public Level level;
    public int levelIndex;
    public LevelData levelData;
    [field: SerializeField] public TextAsset LevelDataTextAsset { get; private set; }

    [Button("Save level data",ButtonSizes.Large)]
    private void SaveData()
    {
        SaveLevelData();
    }

    private void SaveLevelData()
    {
        var encryptJson = DataSerializer.Serialize(levelData);
        var assetsPath = "Assets/BaseGame/TextAssets/LevelData/";
        var directoryPath = Application.dataPath + "/BaseGame/TextAssets/LevelData/";
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }
        
        var fileName = $"Level_{levelIndex}.txt";
        var fullPath = directoryPath + fileName;
        var assetPath = assetsPath + fileName;
        
        LevelDataTextAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(assetPath);
        if (LevelDataTextAsset != null)
        {
            // If the file exists, update it by writing to the file system
            File.WriteAllText(fullPath, encryptJson);
                
            // Refresh the AssetDatabase to update the TextAsset
            AssetDatabase.ImportAsset(assetPath);
                
            Debug.Log($"Updated level data for Level {levelIndex}.");
        }
        else
        {
            // If the file does not exist, create a new one
            File.WriteAllText(fullPath, encryptJson);
                
            // Import the new asset
            AssetDatabase.ImportAsset(assetPath);
                
            // Load the newly created TextAsset
            LevelDataTextAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(assetPath);
                
            Debug.Log($"Created new level data for {gameObject.name}.");
        }
            
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

}
