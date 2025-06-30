using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;
using System.Linq;

public class TilemapValidator : EditorWindow
{
    [MenuItem("RPG Tools/Environment/Validate Tilemap Setup")]
    public static void ShowWindow()
    {
        GetWindow<TilemapValidator>("Tilemap Validator");
    }

    private Vector2 scrollPos;

    private void OnGUI()
    {
        GUILayout.Label("Tilemap Setup Validator", EditorStyles.boldLabel);
        GUILayout.Space(10);

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        if (GUILayout.Button("🔍 Validate Current Scene Setup"))
        {
            ValidateScene();
        }

        GUILayout.Space(10);

        if (GUILayout.Button("🧹 Clean Empty Tilemaps"))
        {
            CleanEmptyTilemaps();
        }

        if (GUILayout.Button("📊 Show Layer Information"))
        {
            ShowLayerInfo();
        }

        GUILayout.Space(10);

        if (GUILayout.Button("⚙️ Fix Common Issues"))
        {
            FixCommonIssues();
        }

        EditorGUILayout.EndScrollView();
    }

    private void ValidateScene()
    {
        Debug.Log("=== TILEMAP VALIDATION REPORT ===");

        // Check for Grid parent
        GameObject grid = GameObject.Find("Grid");
        if (grid == null)
        {
            Debug.LogWarning("❌ No Grid parent found! Create one with 'Setup Tilemap Layers'");
            return;
        }
        else
        {
            Debug.Log("✅ Grid parent found");
        }

        // Check standard layers
        string[] standardLayers = { "Background", "Ground", "Objects", "Decoration", "Collision" };
        int[] expectedSortingOrders = { -10, -5, 0, 5, 10 };

        for (int i = 0; i < standardLayers.Length; i++)
        {
            Transform layer = grid.transform.Find(standardLayers[i]);
            if (layer == null)
            {
                Debug.LogWarning($"❌ Missing layer: {standardLayers[i]}");
                continue;
            }

            TilemapRenderer renderer = layer.GetComponent<TilemapRenderer>();
            if (renderer == null)
            {
                Debug.LogWarning($"❌ {standardLayers[i]}: Missing TilemapRenderer");
                continue;
            }

            if (renderer.sortingOrder != expectedSortingOrders[i])
            {
                Debug.LogWarning($"⚠️ {standardLayers[i]}: Wrong sorting order (got {renderer.sortingOrder}, expected {expectedSortingOrders[i]})");
            }
            else
            {
                Debug.Log($"✅ {standardLayers[i]}: Correct sorting order ({renderer.sortingOrder})");
            }

            // Special check for collision layer
            if (standardLayers[i] == "Collision")
            {
                TilemapCollider2D collider = layer.GetComponent<TilemapCollider2D>();
                CompositeCollider2D composite = layer.GetComponent<CompositeCollider2D>();
                Rigidbody2D rb = layer.GetComponent<Rigidbody2D>();

                if (collider == null)
                    Debug.LogWarning($"❌ {standardLayers[i]}: Missing TilemapCollider2D");
                else if (collider.compositeOperation != Collider2D.CompositeOperation.Merge)
                    Debug.LogWarning($"⚠️ {standardLayers[i]}: TilemapCollider2D should use composite");
                else
                    Debug.Log($"✅ {standardLayers[i]}: TilemapCollider2D configured correctly");

                if (composite == null)
                    Debug.LogWarning($"❌ {standardLayers[i]}: Missing CompositeCollider2D");
                else
                    Debug.Log($"✅ {standardLayers[i]}: CompositeCollider2D found");

                if (rb == null)
                    Debug.LogWarning($"❌ {standardLayers[i]}: Missing Rigidbody2D");
                else if (rb.bodyType != RigidbodyType2D.Static)
                    Debug.LogWarning($"⚠️ {standardLayers[i]}: Rigidbody2D should be Static");
                else
                    Debug.Log($"✅ {standardLayers[i]}: Rigidbody2D configured correctly");
            }
        }

        // Check player setup
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogWarning("❌ No Player found! Make sure Player has 'Player' tag");
        }
        else
        {
            SpriteRenderer playerRenderer = player.GetComponent<SpriteRenderer>();
            if (playerRenderer != null)
            {
                if (playerRenderer.sortingOrder == 1)
                    Debug.Log("✅ Player: Correct sorting order (1)");
                else
                    Debug.LogWarning($"⚠️ Player: Should have sorting order 1 (got {playerRenderer.sortingOrder})");
            }
        }

        Debug.Log("=== VALIDATION COMPLETE ===");
    }

    private void ShowLayerInfo()
    {
        Debug.Log("=== LAYER INFORMATION ===");

        GameObject grid = GameObject.Find("Grid");
        if (grid == null)
        {
            Debug.LogWarning("No Grid found in scene");
            return;
        }

        TilemapRenderer[] renderers = grid.GetComponentsInChildren<TilemapRenderer>();
        if (renderers.Length == 0)
        {
            Debug.LogWarning("No tilemap layers found");
            return;
        }

        // Sort by sorting order
        var sortedRenderers = renderers.OrderBy(r => r.sortingOrder).ToArray();

        foreach (TilemapRenderer renderer in sortedRenderers)
        {
            Tilemap tilemap = renderer.GetComponent<Tilemap>();
            string layerName = renderer.gameObject.name;
            int sortingOrder = renderer.sortingOrder;
            int tileCount = tilemap.GetUsedTilesCount();
            bool hasCollision = renderer.GetComponent<TilemapCollider2D>() != null;

            Debug.Log($"🗺️ {layerName}: Sorting {sortingOrder}, {tileCount} tiles, Collision: {(hasCollision ? "Yes" : "No")}");
        }

        Debug.Log("=== END LAYER INFO ===");
    }

    private void CleanEmptyTilemaps()
    {
        GameObject grid = GameObject.Find("Grid");
        if (grid == null) return;

        TilemapRenderer[] renderers = grid.GetComponentsInChildren<TilemapRenderer>();
        int cleanedCount = 0;

        foreach (TilemapRenderer renderer in renderers)
        {
            Tilemap tilemap = renderer.GetComponent<Tilemap>();
            if (tilemap.GetUsedTilesCount() == 0)
            {
                Debug.Log($"🧹 Cleaning empty tilemap: {renderer.gameObject.name}");
                DestroyImmediate(renderer.gameObject);
                cleanedCount++;
            }
        }

        if (cleanedCount > 0)
        {
            Debug.Log($"✅ Cleaned {cleanedCount} empty tilemaps");
        }
        else
        {
            Debug.Log("✅ No empty tilemaps found");
        }
    }

    private void FixCommonIssues()
    {
        Debug.Log("=== FIXING COMMON ISSUES ===");

        GameObject grid = GameObject.Find("Grid");
        if (grid == null)
        {
            Debug.LogWarning("No Grid found - run 'Setup Tilemap Layers' first");
            return;
        }

        // Fix collision layer issues
        Transform collisionLayer = grid.transform.Find("Collision");
        if (collisionLayer != null)
        {
            TilemapCollider2D collider = collisionLayer.GetComponent<TilemapCollider2D>();
            CompositeCollider2D composite = collisionLayer.GetComponent<CompositeCollider2D>();
            Rigidbody2D rb = collisionLayer.GetComponent<Rigidbody2D>();

            if (collider != null && collider.compositeOperation != Collider2D.CompositeOperation.Merge)
            {
                collider.compositeOperation = Collider2D.CompositeOperation.Merge;
                Debug.Log("✅ Fixed: TilemapCollider2D now uses composite");
            }

            if (rb != null && rb.bodyType != RigidbodyType2D.Static)
            {
                rb.bodyType = RigidbodyType2D.Static;
                Debug.Log("✅ Fixed: Rigidbody2D set to Static");
            }
        }

        // Fix player sorting order
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            SpriteRenderer playerRenderer = player.GetComponent<SpriteRenderer>();
            if (playerRenderer != null && playerRenderer.sortingOrder != 1)
            {
                playerRenderer.sortingOrder = 1;
                Debug.Log("✅ Fixed: Player sorting order set to 1");
            }
        }

        Debug.Log("=== FIXES COMPLETE ===");
    }
}
