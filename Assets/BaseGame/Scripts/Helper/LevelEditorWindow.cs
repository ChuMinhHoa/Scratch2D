#if UNITY_EDITOR
using System.Linq;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

public class LevelEditorWindow : OdinMenuEditorWindow
{
    protected override OdinMenuTree BuildMenuTree()
    {
        OdinMenuTree tree = new(true)
        {
            DefaultMenuStyle =
            {
                IconSize = 28.00f
            },
            Config =
            {
                DrawSearchToolbar = true
            }
        };
        tree.Add("LevelDesign", LevelDesignGlobalConfig.Instance);
        // tree.Add("Items", EditorSkewerSelect.Instance);
        // tree.Add($"Items/Stone", EditorSkewerSelect.Instance.StoneSprite).ForEach(AddDragHandles);
        tree.EnumerateTree().AddIcons<Sprite>(x => x);
            

        return tree;
    }
    
    private void AddDragHandles(OdinMenuItem menuItem)
    {
        menuItem.OnDrawItem += x => DragAndDropUtilities.DragZone(menuItem.Rect, menuItem.Value, false, false);
    }
    protected override void OnBeginDrawEditors()
    {
        OdinMenuItem selected = MenuTree.Selection.AsEnumerable().FirstOrDefault();
        int toolbarHeight = MenuTree.Config.SearchToolbarHeight;

        // Draws a toolbar with the name of the currently selected menu item.
        SirenixEditorGUI.BeginHorizontalToolbar(toolbarHeight);
        {
            if (selected != null)
            {
                GUILayout.Label(selected.Name);
            }

            if (SirenixEditorGUI.ToolbarButton(new GUIContent("Check Level")))
            {
                if (LevelDesignGlobalConfig.Instance.CurrentLevel.LevelData.IsValid(out IGrouping<int, int>[] error))
                {
                    EditorUtility.DisplayDialog("Success", "Level is valid.", "OK");
                }
                else
                {
                    string errorMessage = "";
                    for (int i = 0; i < error.Length; i++)
                    {
                        IGrouping<int, int> grouping = error[i];
                        errorMessage += $"\nSkewer {error[i].Key} has {error[i].Count()}";
                    }
                    EditorUtility.DisplayDialog("Error", $"Level is not valid. : {errorMessage}", "OK");
                }
            }
            if (SirenixEditorGUI.ToolbarButton(new GUIContent("Try Fill Conveyor Data")))
            {
                //LevelDesignGlobalConfig.Instance.CurrentLevel.LevelData.TryFillConveyorData();
            }
        }
        SirenixEditorGUI.EndHorizontalToolbar();
    }
}

#endif