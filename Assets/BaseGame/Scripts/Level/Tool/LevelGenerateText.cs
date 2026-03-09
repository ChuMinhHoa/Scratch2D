#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using CoreData;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

public class LevelGenerateText : MonoBehaviour
{
    public Level level;
    public int levelIndex;
    public LevelData levelData => level.LevelData;

    string assetsPath = "Assets/BaseGame/TextAssets/LevelData/";
    string directoryPath = Application.dataPath + "/BaseGame/TextAssets/LevelData/";
    [field: SerializeField] public TextAsset LevelDataTextAsset { get; private set; }

    public List<Sprite> stickers;

    [Button]
    private void ChangeIndexName()
    {
        for (int i = 0; i < stickers.Count; i++)
        {
            var sprite = stickers[i];
            if (sprite == null) continue;

            string assetPath = AssetDatabase.GetAssetPath(sprite);
            string directory = Path.GetDirectoryName(assetPath);
            string extension = Path.GetExtension(assetPath);
            string originalName = Path.GetFileNameWithoutExtension(assetPath);

            // Remove all numeric characters
            string nameWithoutNumbers = System.Text.RegularExpressions.Regex.Replace(originalName, @"\d", "");

            // Prepend the index to the name
            string newName = $"{i}.{nameWithoutNumbers}";
            string newPath = Path.Combine(directory, newName + extension);

            string error = AssetDatabase.RenameAsset(assetPath, newName);
            if (!string.IsNullOrEmpty(error))
            {
                Debug.LogError($"Failed to rename {originalName} to {newName}: {error}");
            }
            else
            {
                Debug.Log($"Renamed {originalName} -> {newName}");
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    [Button]
    private void LoadLevelDataText()
    {
        var fileName = $"Level_{levelIndex}.txt";
        var assetPath = assetsPath + fileName;
        var fullPath = directoryPath + fileName;
        if (!File.Exists(assetPath))
        {
            Debug.Log(fileName);
            var newLevelData = new LevelData();
            var encryptJson = DataSerializer.Serialize(newLevelData);
            CreateTextAsset(encryptJson);
        }
        else LevelDataTextAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(assetPath);

        level.levelIndex.Value = levelIndex;
        level.LoadOnlyData();
    }

    [Button("Save level data",ButtonSizes.Large)]
    private void SaveData()
    {
        SaveLevelData();
    }
  
    private void SaveLevelData()
    {
        var encryptJson = DataSerializer.Serialize(levelData);
       
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        OnValidateTextAsset(encryptJson);
            
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private void OnValidateTextAsset(string encryptJson)
    {
        var fileName = $"Level_{levelIndex}.txt";
        var assetPath = assetsPath + fileName;
        var fullPath = directoryPath + fileName;
        
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
    
    private void CreateTextAsset(string encryptJson)
    {
        var fileName = $"Level_{levelIndex}.txt";
        var assetPath = assetsPath + fileName;
        var fullPath = directoryPath + fileName;
        
        File.WriteAllText(fullPath, encryptJson);
            
        AssetDatabase.ImportAsset(assetPath);
            
        LevelDataTextAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(assetPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    [Button("Open Level Editor")]
    private void OpenEditor()
    {
        LevelEditorWindow window = (LevelEditorWindow) EditorWindow.GetWindow( typeof(LevelEditorWindow), false, "Level Design Editor");
        LevelDesignGlobalConfig.Instance.CurrentLevel = level;
        window.Show();      
    }

}

#endif
