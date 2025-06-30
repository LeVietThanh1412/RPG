using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;

public class TilemapLayerSetup : EditorWindow
{
    [MenuItem("RPG Tools/Environment/Setup Tilemap Layers")]
    public static void ShowWindow()
    {
        GetWindow<TilemapLayerSetup>("Tilemap Layer Setup");
    }

    private void OnGUI()
    {
        GUILayout.Label("Tilemap Layer Setup Tool", EditorStyles.boldLabel);
        GUILayout.Space(10);

        if (GUILayout.Button("Create Standard Layer Setup"))
        {
            CreateStandardLayers();
        }

        GUILayout.Space(10);
        GUILayout.Label("Standard layers will be created with proper sorting:", EditorStyles.helpBox);
        GUILayout.Label("• Background (-10): Sky, distant mountains");
        GUILayout.Label("• Ground (-5): Grass, dirt, water");
        GUILayout.Label("• Objects (0): Trees, rocks, buildings");
        GUILayout.Label("• Decoration (5): Flowers, small details");
        GUILayout.Label("• Collision (10): Invisible collision tiles");

        GUILayout.Space(10);
        if (GUILayout.Button("Setup Layer Physics"))
        {
            SetupLayerPhysics();
        }

        if (GUILayout.Button("Create Grid Parent"))
        {
            CreateGridParent();
        }
    }

    private void CreateStandardLayers()
    {
        // Find or create Grid parent
        GameObject grid = GameObject.Find("Grid");
        if (grid == null)
        {
            grid = CreateGridParent();
        }

        // Layer configurations: Name, Sorting Order, Has Collision
        var layerConfigs = new[]
        {
            ("Background", -10, false),
            ("Ground", -5, false),
            ("Objects", 0, false),
            ("Decoration", 5, false),
            ("Collision", 10, true)
        };

        foreach (var (name, sortingOrder, hasCollision) in layerConfigs)
        {
            CreateTilemapLayer(grid, name, sortingOrder, hasCollision);
        }

        Debug.Log("Standard tilemap layers created successfully!");
    }

    private GameObject CreateGridParent()
    {
        GameObject grid = new GameObject("Grid");
        grid.AddComponent<Grid>();

        // Position at origin
        grid.transform.position = Vector3.zero;

        Debug.Log("Grid parent object created");
        return grid;
    }

    private void CreateTilemapLayer(GameObject parent, string layerName, int sortingOrder, bool hasCollision)
    {
        // Check if layer already exists
        Transform existingLayer = parent.transform.Find(layerName);
        if (existingLayer != null)
        {
            Debug.Log($"Layer '{layerName}' already exists, skipping...");
            return;
        }

        // Create tilemap object
        GameObject tilemapObj = new GameObject(layerName);
        tilemapObj.transform.SetParent(parent.transform);
        tilemapObj.transform.position = Vector3.zero;

        // Add Tilemap component
        Tilemap tilemap = tilemapObj.AddComponent<Tilemap>();

        // Add TilemapRenderer component
        TilemapRenderer renderer = tilemapObj.AddComponent<TilemapRenderer>();
        renderer.sortingOrder = sortingOrder;

        // Add collision if needed
        if (hasCollision)
        {
            TilemapCollider2D collider = tilemapObj.AddComponent<TilemapCollider2D>();
            // Optional: Add CompositeCollider2D for better performance
            CompositeCollider2D compositeCollider = tilemapObj.AddComponent<CompositeCollider2D>();
            collider.compositeOperation = Collider2D.CompositeOperation.Merge;

            // Set rigidbody for composite collider
            Rigidbody2D rb = tilemapObj.GetComponent<Rigidbody2D>();
            if (rb == null) rb = tilemapObj.AddComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Static;
        }

        Debug.Log($"Created layer: {layerName} (Sorting Order: {sortingOrder})");
    }

    private void SetupLayerPhysics()
    {
        // Set up physics layers if they don't exist
        string[] layerNames = { "Ground", "Player", "Enemy", "Items", "Environment" };

        for (int i = 0; i < layerNames.Length; i++)
        {
            int layerIndex = 8 + i; // Start from layer 8 (avoid built-in layers)
            if (layerIndex < 32 && string.IsNullOrEmpty(LayerMask.LayerToName(layerIndex)))
            {
                SetLayerName(layerIndex, layerNames[i]);
                Debug.Log($"Created physics layer: {layerNames[i]} on layer {layerIndex}");
            }
        }

        Debug.Log("Physics layers setup complete!");
    }

    private void SetLayerName(int layerIndex, string layerName)
    {
        // This requires reflection as Unity doesn't expose layer setting directly
        var tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        var layers = tagManager.FindProperty("layers");

        if (layers != null && layerIndex < layers.arraySize)
        {
            var layerProperty = layers.GetArrayElementAtIndex(layerIndex);
            layerProperty.stringValue = layerName;
            tagManager.ApplyModifiedProperties();
        }
    }
}
