// csharp
using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

public enum TileMapState
{
    None,
    Center,
    Dirty
}

public class TileMapIce : MonoBehaviour
{
    [TileMap] [HideLabel] [ShowInInspector]
    public TileMapState[,] posActive = new TileMapState[3, 3];
}

[System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
public sealed class TileMap : System.Attribute
{
}

public sealed class TileMapAttributeDrawer : OdinAttributeDrawer<TileMap, TileMapState[,] >
{
    protected override void DrawPropertyLayout(GUIContent label)
    {
        var tileMap = ValueEntry.SmartValue;
        if (tileMap == null)
        {
            EditorGUILayout.HelpBox("This attribute can only be applied to a 2D array of TileMapState.", MessageType.Error);
            return;
        }

        int rows = tileMap.GetLength(0);
        int cols = tileMap.GetLength(1);

        for (int i = 0; i < rows; i++)
        {
            EditorGUILayout.BeginHorizontal();
            Rect rect = new Rect(0, 0, 50, 50);
            for (int j = 0; j < cols; j++)
            {
                var state = tileMap[i, j];
                var prevBg = GUI.backgroundColor;
                GUI.backgroundColor = GetColorForState(state);

                if (GUILayout.Button(state.ToString(), GUILayout.Width(80), GUILayout.Height(80)))
                {
                    tileMap[i, j] = GetNextState(state);
                    ValueEntry.SmartValue = tileMap; // write back the modified array
                }

                GUI.backgroundColor = prevBg;
            }
            EditorGUILayout.EndHorizontal();
        }
    }

    private TileMapState GetNextState(TileMapState state)
    {
        return state switch
        {
            TileMapState.None => TileMapState.Center,
            TileMapState.Center => TileMapState.Dirty,
            _ => TileMapState.None
        };
    }

    private Color GetColorForState(TileMapState state)
    {
        return state switch
        {
            TileMapState.Center => Color.green,
            TileMapState.Dirty => Color.yellow,
            _ => Color.white
        };
    }
}
