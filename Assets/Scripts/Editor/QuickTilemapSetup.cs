using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;

public class QuickTilemapSetup : EditorWindow
{
    [MenuItem("RPG Tools/🚀 Quick Tilemap Setup")]
    public static void SetupTilemapsNow()
    {
        Debug.Log("=== QUICK TILEMAP SETUP STARTING ===");

        // Check if Grid already exists
        GameObject existingGrid = GameObject.Find("Grid");
        if (existingGrid != null)
        {
            Debug.Log("⚠️ Grid already exists! Removing old setup...");
            DestroyImmediate(existingGrid);
        }

        // Create Grid parent
        GameObject grid = new GameObject("Grid");
        grid.AddComponent<Grid>();
        grid.transform.position = Vector3.zero;
        Debug.Log("✅ Created Grid parent object");

        // Layer configurations: Name, Sorting Order, Has Collision
        var layerConfigs = new[]
        {
            ("Background", -10, false),
            ("Ground", -5, false),
            ("Objects", 0, false),
            ("Decoration", 5, false),
            ("Collision", 10, true)
        };

        Debug.Log("🎨 Creating tilemap layers...");

        foreach (var (name, sortingOrder, hasCollision) in layerConfigs)
        {
            CreateTilemapLayer(grid, name, sortingOrder, hasCollision);
        }

        // Set player sorting order if exists
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            SpriteRenderer playerRenderer = player.GetComponent<SpriteRenderer>();
            if (playerRenderer != null)
            {
                playerRenderer.sortingOrder = 1;
                Debug.Log("✅ Set Player sorting order to 1 (between Objects and Decoration)");
            }
        }

        Debug.Log("🎉 TILEMAP SETUP COMPLETE!");
        Debug.Log("📋 Hierarchy now has: Grid → Background → Ground → Objects → Decoration → Collision");
        Debug.Log("🎯 Next steps: Import sprites → Create tile palettes → Paint your map!");

        // Focus on Grid in hierarchy
        Selection.activeGameObject = grid;
        EditorGUIUtility.PingObject(grid);
    }

    private static void CreateTilemapLayer(GameObject parent, string layerName, int sortingOrder, bool hasCollision)
    {
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
            CompositeCollider2D compositeCollider = tilemapObj.AddComponent<CompositeCollider2D>();
            collider.compositeOperation = Collider2D.CompositeOperation.Merge;

            // Set rigidbody for composite collider
            Rigidbody2D rb = tilemapObj.GetComponent<Rigidbody2D>();
            if (rb == null) rb = tilemapObj.AddComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Static;

            Debug.Log($"🧱 {layerName}: Created with collision (Sorting: {sortingOrder})");
        }
        else
        {
            Debug.Log($"🎨 {layerName}: Created visual layer (Sorting: {sortingOrder})");
        }
    }

    [MenuItem("RPG Tools/🔍 Show Current Tilemap Status")]
    public static void ShowTilemapStatus()
    {
        Debug.Log("=== CURRENT TILEMAP STATUS ===");

        GameObject grid = GameObject.Find("Grid");
        if (grid == null)
        {
            Debug.LogWarning("❌ No Grid found! Run 'Quick Tilemap Setup' first.");
            return;
        }

        Debug.Log("✅ Grid found!");

        TilemapRenderer[] renderers = grid.GetComponentsInChildren<TilemapRenderer>();
        if (renderers.Length == 0)
        {
            Debug.LogWarning("❌ No tilemap layers found in Grid");
            return;
        }

        Debug.Log($"📊 Found {renderers.Length} tilemap layers:");

        foreach (TilemapRenderer renderer in renderers)
        {
            string layerName = renderer.gameObject.name;
            int sortingOrder = renderer.sortingOrder;
            bool hasCollision = renderer.GetComponent<TilemapCollider2D>() != null;

            string collisionStatus = hasCollision ? "🧱 Collision" : "🎨 Visual";
            Debug.Log($"   • {layerName}: Sorting {sortingOrder} ({collisionStatus})");
        }

        // Check Environment folders
        bool hasTerrainFolder = AssetDatabase.IsValidFolder("Assets/Sprites/Environment/Terrain");
        bool hasPalettesFolder = AssetDatabase.IsValidFolder("Assets/Palettes");

        Debug.Log("📁 Environment Folders:");
        Debug.Log($"   • Terrain: {(hasTerrainFolder ? "✅ Exists" : "❌ Missing")}");
        Debug.Log($"   • Palettes: {(hasPalettesFolder ? "✅ Exists" : "❌ Missing")}");

        Debug.Log("=== STATUS CHECK COMPLETE ===");
    }
}
