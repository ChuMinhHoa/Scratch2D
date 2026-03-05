using System.Collections.Generic;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;
using Sirenix.Utilities;

[CreateAssetMenu(fileName = "CardDesignGlobalConfig", menuName = "GlobalConfigs/CardDesignGlobalConfig")]
[GlobalConfig("Assets/Resources/GlobalConfig/")]
public class CardDesignGlobalConfig : GlobalConfig<CardDesignGlobalConfig>
{
    [field: SerializeField, CardDataEditor]
    public CardData CardData { get; set; }
}

[System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
public sealed class CardDataEditor : System.Attribute
{
}

#if UNITY_EDITOR
public sealed class CardDataEditorAttributeDrawer : OdinAttributeDrawer<CardDataEditor, CardData>
{
    private const float Padding = 10f;
    private const float LayerHeaderHeight = 25f;
    private const float StickerSize = 80f;
    private const float StickerSpacing = 2f;
    private const float CardPadding = 4f;
    private const int StickersPerRow = 3;
    private const float PositionScale = 50f;
    private const float MinLayerHeight = 200f;
    private const float ExtraPadding = 50f;

    private static List<Card> _cardPrefabs = new();
    private static Transform _cardContainer;
    private static Dictionary<(int layer, int card), GameObject> _spawnedCards = new();
    private static bool _prefabsFoldout = true;

    private const int VerticalCardTypeThreshold = 99;

    // Drag state
    private static bool _isDragging = false;
    private static int _draggingLayerIndex = -1;
    private static int _draggingCardIndex = -1;
    private static Vector2 _dragStartMousePos;
    private static Vector3 _dragStartCardPos;

    private Vector2 GetCardSize(CardData card)
    {
        int stickerCount = card.stickers?.Length ?? 0;
        if (stickerCount == 0)
        {
            return new Vector2(StickerSize + CardPadding * 2, StickerSize + CardPadding * 2);
        }

        bool isVertical = (int)card.cardType > VerticalCardTypeThreshold;

        if (isVertical)
        {
            float width = StickerSize + CardPadding * 2;
            float height = stickerCount * (StickerSize + StickerSpacing) - StickerSpacing + CardPadding * 2;
            return new Vector2(width, height);
        }
        else
        {
            int cols = Mathf.Min(stickerCount, StickersPerRow);
            int rows = Mathf.CeilToInt((float)stickerCount / StickersPerRow);

            float width = cols * (StickerSize + StickerSpacing) - StickerSpacing + CardPadding * 2;
            float height = rows * (StickerSize + StickerSpacing) - StickerSpacing + CardPadding * 2;
            return new Vector2(width, height);
        }
    }

    protected override void DrawPropertyLayout(GUIContent label)
    {
        var levelConfig = LevelDesignGlobalConfig.Instance;
        if (levelConfig == null || levelConfig.CurrentLevel == null)
        {
            EditorGUILayout.HelpBox("LevelDesignGlobalConfig hoặc CurrentLevel chưa được thiết lập!",
                MessageType.Warning);
            return;
        }

        var levelData = levelConfig.CurrentLevel.LevelData;
        if (levelData == null || levelData.layerCards == null || levelData.layerCards.Length == 0)
        {
            EditorGUILayout.HelpBox("Không có dữ liệu layer!", MessageType.Info);
            return;
        }

        // Handle global drag events
        HandleGlobalDragEvents(levelData);

        DrawPrefabSettings();
        DrawSpawnControls(levelData);

        float totalHeight = CalculateTotalHeight(levelData);
        Rect mainRect = EditorGUILayout.GetControlRect(false, totalHeight);

        DrawLayerGrid(mainRect, levelData);
    }

    private void HandleGlobalDragEvents(LevelData levelData)
    {
        Event e = Event.current;

        if (_isDragging)
        {
            if (e.type == EventType.MouseDrag)
            {
                Vector2 delta = e.mousePosition - _dragStartMousePos;

                // Convert screen delta to world position delta
                float deltaX = delta.x / PositionScale;
                float deltaY = -delta.y / PositionScale; // Invert Y

                Vector3 newPos = _dragStartCardPos + new Vector3(deltaX, deltaY, 0);

                // Snap to grid (optional - 0.1 unit grid)
                newPos.x = Mathf.Round(newPos.x * 10f) / 10f;
                newPos.y = Mathf.Round(newPos.y * 10f) / 10f;

                levelData.layerCards[_draggingLayerIndex].cards[_draggingCardIndex].position = newPos;

                EditorUtility.SetDirty(levelConfig.CurrentLevel);
                e.Use();
            }
            else if (e.type == EventType.MouseUp && e.button == 0)
            {
                _isDragging = false;
                _draggingLayerIndex = -1;
                _draggingCardIndex = -1;
                e.Use();
            }
        }
    }

    private static LevelDesignGlobalConfig levelConfig => LevelDesignGlobalConfig.Instance;

    private void DrawPrefabSettings()
    {
        _prefabsFoldout = EditorGUILayout.Foldout(_prefabsFoldout, "Card Prefabs", true);

        if (_prefabsFoldout)
        {
            EditorGUI.indentLevel++;

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Container", GUILayout.Width(80));
            _cardContainer = EditorGUILayout.ObjectField(_cardContainer, typeof(Transform), true) as Transform;
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(3);

            for (int i = 0; i < _cardPrefabs.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                _cardPrefabs[i] = EditorGUILayout.ObjectField(_cardPrefabs[i], typeof(Card), false) as Card;

                if (GUILayout.Button("-", GUILayout.Width(20)))
                {
                    _cardPrefabs.RemoveAt(i);
                    i--;
                }

                EditorGUILayout.EndHorizontal();
            }

            if (GUILayout.Button("+ Add Card Prefab"))
            {
                _cardPrefabs.Add(null);
            }

            EditorGUI.indentLevel--;
        }

        EditorGUILayout.Space(5);
    }

    private void DrawSpawnControls(LevelData levelData)
    {
        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Spawn All Cards", GUILayout.Height(25)))
        {
            SpawnAllCards(levelData);
        }

        if (GUILayout.Button("Sync Positions", GUILayout.Height(25)))
        {
            SyncAllCardPositions(levelData);
        }

        if (GUILayout.Button("Clear All", GUILayout.Height(25)))
        {
            ClearAllSpawnedCards();
        }

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space(5);
    }

    private void SpawnAllCards(LevelData levelData)
    {
        if (_cardPrefabs.Count == 0 || _cardPrefabs.TrueForAll(p => p == null))
        {
            EditorUtility.DisplayDialog("Error", "Please assign at least one Card Prefab!", "OK");
            return;
        }

        if (_cardContainer == null)
        {
            GameObject container = new GameObject("CardContainer");
            _cardContainer = container.transform;
            Undo.RegisterCreatedObjectUndo(container, "Create Card Container");
        }

        ClearAllSpawnedCards();

        for (int layerIndex = 0; layerIndex < levelData.layerCards.Length; layerIndex++)
        {
            var layer = levelData.layerCards[layerIndex];
            if (layer.cards == null) continue;

            GameObject layerParent = new GameObject($"Layer_{layerIndex}");
            layerParent.transform.SetParent(_cardContainer);
            layerParent.transform.localPosition = new Vector3(0, 0, -layerIndex * 0.1f);
            Undo.RegisterCreatedObjectUndo(layerParent, "Create Layer Parent");

            for (int cardIndex = 0; cardIndex < layer.cards.Length; cardIndex++)
            {
                var cardData = layer.cards[cardIndex];
                SpawnCard(cardData, layerIndex, cardIndex, layerParent.transform);
            }
        }

        Debug.Log($"Spawned {_spawnedCards.Count} cards");
    }

    private void SpawnCard(CardData cardData, int layerIndex, int cardIndex, Transform parent)
    {
        GameObject prefab = GetPrefabForCardType(cardData.cardType);

        if (prefab == null)
        {
            Debug.LogWarning($"No prefab found for CardType: {cardData.cardType}");
            return;
        }

        GameObject cardObj = PrefabUtility.InstantiatePrefab(prefab, parent) as GameObject;
        if (cardObj == null) return;

        cardObj.name = $"Card_{layerIndex}_{cardIndex}_{cardData.cardType}";
        cardObj.transform.localPosition = cardData.position;
        cardObj.transform.localEulerAngles = cardData.rotation;

        _spawnedCards[(layerIndex, cardIndex)] = cardObj;
        Undo.RegisterCreatedObjectUndo(cardObj, "Spawn Card");
    }

    private GameObject GetPrefabForCardType(CardType cardType)
    {
        foreach (var card in _cardPrefabs)
        {
            if (card != null && card.cardType == cardType)
            {
                return card.gameObject;
            }
        }

        return null;
    }


    private void SyncAllCardPositions(LevelData levelData)
    {
        if (_spawnedCards.Count == 0)
        {
            EditorUtility.DisplayDialog("Info", "No spawned cards to sync. Spawn cards first.", "OK");
            return;
        }

        int syncedCount = 0;

        for (int layerIndex = 0; layerIndex < levelData.layerCards.Length; layerIndex++)
        {
            var layer = levelData.layerCards[layerIndex];
            if (layer.cards == null) continue;

            for (int cardIndex = 0; cardIndex < layer.cards.Length; cardIndex++)
            {
                var cardData = layer.cards[cardIndex];
                var key = (layerIndex, cardIndex);

                if (_spawnedCards.TryGetValue(key, out GameObject cardObj) && cardObj != null)
                {
                    Undo.RecordObject(cardObj.transform, "Sync Card Position");
                    var pos = cardData.position;
                    pos.z = layerIndex; // Ensure Z position matches layer
                    cardObj.transform.localPosition = pos;
                    cardObj.transform.localEulerAngles = cardData.rotation;
                    syncedCount++;
                }
            }
        }

        Debug.Log($"Synced {syncedCount} card positions");
    }

    private void ClearAllSpawnedCards()
    {
        foreach (var kvp in _spawnedCards)
        {
            if (kvp.Value != null)
            {
                Undo.DestroyObjectImmediate(kvp.Value);
            }
        }

        _spawnedCards.Clear();

        if (_cardContainer != null)
        {
            while (_cardContainer.childCount > 0)
            {
                Undo.DestroyObjectImmediate(_cardContainer.GetChild(0).gameObject);
            }
        }
    }

    private float CalculateTotalHeight(LevelData levelData)
    {
        float totalHeight = Padding;
        for (int i = 0; i < levelData.layerCards.Length; i++)
        {
            totalHeight += CalculateLayerHeight(levelData.layerCards[i]) + Padding;
        }

        return totalHeight;
    }

    private float CalculateLayerHeight(LayerCardData layer)
    {
        if (layer.cards == null || layer.cards.Length == 0)
        {
            return MinLayerHeight;
        }

        float minY = float.MaxValue;
        float maxY = float.MinValue;

        foreach (var card in layer.cards)
        {
            Vector2 cardSize = GetCardSize(card);
            float cardTopY = -card.position.y * PositionScale - cardSize.y / 2;
            float cardBottomY = -card.position.y * PositionScale + cardSize.y / 2;

            minY = Mathf.Min(minY, cardTopY);
            maxY = Mathf.Max(maxY, cardBottomY);
        }

        float contentHeight = maxY - minY + ExtraPadding * 2;
        return Mathf.Max(MinLayerHeight, contentHeight + LayerHeaderHeight + Padding);
    }

    private void DrawLayerGrid(Rect rect, LevelData levelData)
    {
        float currentY = rect.y + Padding;

        for (int layerIndex = 0; layerIndex < levelData.layerCards.Length; layerIndex++)
        {
            var layer = levelData.layerCards[layerIndex];
            float layerHeight = CalculateLayerHeight(layer);
            Rect layerRect = new Rect(rect.x, currentY, rect.width, layerHeight);

            DrawLayer(layerRect, layer, layerIndex);

            currentY += layerHeight + Padding;
        }
    }

    private void DrawLayer(Rect rect, LayerCardData layer, int layerIndex)
    {
        Color layerColor = GetLayerColor(layerIndex);
        SirenixEditorGUI.DrawSolidRect(rect, layerColor);
        SirenixEditorGUI.DrawBorders(rect, 1);

        Rect headerRect = new Rect(rect.x + Padding, rect.y + 2, rect.width - Padding * 2, LayerHeaderHeight - 4);
        EditorGUI.LabelField(headerRect, $"Layer {layerIndex} ({layer.cards?.Length ?? 0} cards)",
            EditorStyles.boldLabel);

        Rect contentRect = new Rect(rect.x + Padding, rect.y + LayerHeaderHeight,
            rect.width - Padding * 2, rect.height - LayerHeaderHeight - Padding);

        SirenixEditorGUI.DrawSolidRect(contentRect, new Color(0.15f, 0.15f, 0.15f, 1f));

        if (layer.cards == null || layer.cards.Length == 0)
        {
            EditorGUI.LabelField(contentRect, "No cards", EditorStyles.centeredGreyMiniLabel);
            return;
        }

        Vector2 boundsCenter = CalculateBoundsCenter(layer);
        Vector2 centerPoint = new Vector2(
            contentRect.x + contentRect.width / 2 - boundsCenter.x * PositionScale,
            contentRect.y + contentRect.height / 2 + boundsCenter.y * PositionScale
        );

        for (int cardIndex = 0; cardIndex < layer.cards.Length; cardIndex++)
        {
            var card = layer.cards[cardIndex];
            Vector2 cardSize = GetCardSize(card);

            float editorX = centerPoint.x + card.position.x * PositionScale - cardSize.x / 2;
            float editorY = centerPoint.y - card.position.y * PositionScale - cardSize.y / 2;

            Rect cardRect = new Rect(editorX, editorY, cardSize.x, cardSize.y);

            DrawCardCell(cardRect, card, layerIndex, cardIndex);
        }

        DrawOriginCrosshair(centerPoint);
    }

    private Vector2 CalculateBoundsCenter(LayerCardData layer)
    {
        if (layer.cards == null || layer.cards.Length == 0)
        {
            return Vector2.zero;
        }

        float minX = float.MaxValue, maxX = float.MinValue;
        float minY = float.MaxValue, maxY = float.MinValue;

        foreach (var card in layer.cards)
        {
            minX = Mathf.Min(minX, card.position.x);
            maxX = Mathf.Max(maxX, card.position.x);
            minY = Mathf.Min(minY, card.position.y);
            maxY = Mathf.Max(maxY, card.position.y);
        }

        return new Vector2((minX + maxX) / 2, (minY + maxY) / 2);
    }

    private void DrawOriginCrosshair(Vector2 center)
    {
        Color crossColor = new Color(1f, 1f, 1f, 0.3f);
        Rect hLine = new Rect(center.x - 20, center.y - 1, 40, 2);
        Rect vLine = new Rect(center.x - 1, center.y - 20, 2, 40);
        SirenixEditorGUI.DrawSolidRect(hLine, crossColor);
        SirenixEditorGUI.DrawSolidRect(vLine, crossColor);
    }

    private void DrawCardCell(Rect rect, CardData card, int layerIndex, int cardIndex)
    {
        float rotation = card.rotation.z;
        Event e = Event.current;

        // Save current matrix
        Matrix4x4 matrixBackup = GUI.matrix;

        // Apply rotation around card center
        Vector2 pivot = new Vector2(rect.x + rect.width / 2, rect.y + rect.height / 2);
        GUIUtility.RotateAroundPivot(rotation, pivot);

        // Draw card with rotation
        Color cardColor = GetCardColor(card.cardType);

        // Highlight if dragging this card
        if (_isDragging && _draggingLayerIndex == layerIndex && _draggingCardIndex == cardIndex)
        {
            cardColor = new Color(cardColor.r + 0.2f, cardColor.g + 0.2f, cardColor.b + 0.2f, 1f);
        }

        SirenixEditorGUI.DrawSolidRect(rect, cardColor);
        SirenixEditorGUI.DrawBorders(rect, 1, new Color(0.2f, 0.2f, 0.2f));

        Rect stickersArea = new Rect(rect.x + CardPadding, rect.y + CardPadding,
            rect.width - CardPadding * 2, rect.height - CardPadding * 2);
        DrawStickersInCard(stickersArea, card);

        // Restore matrix before handling input
        GUI.matrix = matrixBackup;

        // Handle drag start
        if (e.type == EventType.MouseDown && e.button == 0 && rect.Contains(e.mousePosition))
        {
            _isDragging = true;
            _draggingLayerIndex = layerIndex;
            _draggingCardIndex = cardIndex;
            _dragStartMousePos = e.mousePosition;
            _dragStartCardPos = card.position;

            ValueEntry.SmartValue = card;
            e.Use();
        }

        // Selection highlight
        if (ValueEntry.SmartValue == card)
        {
            GUIUtility.RotateAroundPivot(rotation, pivot);
            SirenixEditorGUI.DrawBorders(rect, 2, Color.yellow);
            GUI.matrix = matrixBackup;
        }

        // Hover highlight
        if (rect.Contains(e.mousePosition) && !_isDragging)
        {
            GUIUtility.RotateAroundPivot(rotation, pivot);
            SirenixEditorGUI.DrawSolidRect(rect, new Color(1f, 1f, 1f, 0.1f));
            GUI.matrix = matrixBackup;

            // Change cursor to indicate draggable
            EditorGUIUtility.AddCursorRect(rect, MouseCursor.MoveArrow);
        }

        // Draw position label
        GUI.matrix = matrixBackup;
        Rect labelRect = new Rect(rect.x, rect.yMax + 2, rect.width, 16);
        EditorGUI.LabelField(labelRect, $"({card.position.x:F1}, {card.position.y:F1})", EditorStyles.miniLabel);
    }

    private void DrawStickersInCard(Rect rect, CardData card)
    {
        if (card.stickers == null || card.stickers.Length == 0)
        {
            EditorGUI.LabelField(rect, "Empty", EditorStyles.centeredGreyMiniLabel);
            return;
        }

        bool isVertical = (int)card.cardType > VerticalCardTypeThreshold;

        float currentX = rect.x;
        float currentY = rect.y;

        for (int i = 0; i < card.stickers.Length; i++)
        {
            if (isVertical)
            {
                // Vertical layout: new row for each sticker
                if (i > 0)
                {
                    currentY += StickerSize + StickerSpacing;
                }
            }
            else
            {
                // Horizontal layout: StickersPerRow per row
                if (i > 0 && i % StickersPerRow == 0)
                {
                    currentX = rect.x;
                    currentY += StickerSize + StickerSpacing;
                }
            }

            Rect stickerRect = new Rect(currentX, currentY, StickerSize, StickerSize);
            DrawStickerIcon(stickerRect, card.stickers[i]);

            if (!isVertical)
            {
                currentX += StickerSize + StickerSpacing;
            }
        }
    }

    private void DrawStickerIcon(Rect rect, StickerData sticker)
    {
        Color bgColor = sticker.stickerType == StickerType.Normal
            ? new Color(0.4f, 0.4f, 0.4f, 1f)
            : new Color(0.8f, 0.7f, 0.2f, 1f);

        SirenixEditorGUI.DrawSolidRect(rect, bgColor);
        SirenixEditorGUI.DrawBorders(rect, 1, Color.black);

        Sprite sprite = SpriteGlobalConfig.Instance.GetStickerIcon(sticker.stickerID);
        if (sprite != null && sprite.texture != null)
        {
            Rect texCoords = new Rect(
                sprite.textureRect.x / sprite.texture.width,
                sprite.textureRect.y / sprite.texture.height,
                sprite.textureRect.width / sprite.texture.width,
                sprite.textureRect.height / sprite.texture.height
            );
            GUI.DrawTextureWithTexCoords(rect, sprite.texture, texCoords);
        }
        else
        {
            EditorGUI.LabelField(rect, sticker.stickerID >= 0 ? sticker.stickerID.ToString() : "?",
                EditorStyles.centeredGreyMiniLabel);
        }
    }

    private Color GetLayerColor(int layerIndex)
    {
        Color[] colors =
        {
            new Color(0.3f, 0.4f, 0.5f, 1f),
            new Color(0.4f, 0.3f, 0.5f, 1f),
            new Color(0.3f, 0.5f, 0.4f, 1f),
            new Color(0.5f, 0.4f, 0.3f, 1f),
            new Color(0.4f, 0.5f, 0.3f, 1f),
        };
        return colors[layerIndex % colors.Length];
    }

    private Color GetCardColor(CardType cardType)
    {
        return cardType switch
        {
            CardType.Card1 => new Color(0.5f, 0.6f, 0.7f, 1f),
            CardType.Card2 => new Color(0.6f, 0.5f, 0.7f, 1f),
            CardType.Card3 => new Color(0.7f, 0.6f, 0.5f, 1f),
            _ => new Color(0.5f, 0.5f, 0.5f, 1f)
        };
    }
}
#endif