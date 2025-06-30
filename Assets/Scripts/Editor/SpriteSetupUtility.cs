using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
public class SpriteSetupUtility : EditorWindow
{
    [MenuItem("RPG Framework/Setup Sprites")]
    public static void ShowWindow()
    {
        GetWindow<SpriteSetupUtility>("Sprite Setup");
    }

    private void OnGUI()
    {
        GUILayout.Label("Sprite Setup Utility", EditorStyles.boldLabel);
        GUILayout.Space(10);

        GUILayout.Label("Automatically configure sprite import settings for RPG sprites", EditorStyles.helpBox);
        GUILayout.Space(10);

        if (GUILayout.Button("Setup Character Sprites"))
        {
            SetupCharacterSprites();
        }

        if (GUILayout.Button("Setup Environment Sprites"))
        {
            SetupEnvironmentSprites();
        }

        if (GUILayout.Button("Setup Item Sprites"))
        {
            SetupItemSprites();
        }

        if (GUILayout.Button("Setup All Sprites"))
        {
            SetupAllSprites();
        }

        GUILayout.Space(20);
        GUILayout.Label("This will set optimal import settings for pixel art:", EditorStyles.helpBox);
        GUILayout.Label("• Sprite Mode: Multiple");
        GUILayout.Label("• Pixels Per Unit: 16");
        GUILayout.Label("• Filter Mode: Point");
        GUILayout.Label("• Compression: None");
    }

    private void SetupCharacterSprites()
    {
        string[] characterPaths = {
            "Assets/Sprites/Characters/Idle.png",
            "Assets/Sprites/Characters/Walk.png"
        };

        foreach (string path in characterPaths)
        {
            ConfigureSprite(path, 16, true, 16, 16);
        }

        Debug.Log("Character sprites configured!");
    }

    private void SetupEnvironmentSprites()
    {
        string[] environmentPaths = {
            "Assets/Sprites/Environment/Tilesets/Tileset Spring.png",
            "Assets/Sprites/Environment/Tilesets/Walls and Floors copiar.png",
            "Assets/Sprites/Environment/Buildings/House.png",
            "Assets/Sprites/Environment/Buildings/Fence's copiar.png",
            "Assets/Sprites/Environment/Roads/Road copiar.png",
            "Assets/Sprites/Environment/Vegetation/Maple Tree.png",
            "Assets/Sprites/Environment/Vegetation/Spring Crops.png"
        };

        foreach (string path in environmentPaths)
        {
            ConfigureSprite(path, 16, true, 16, 16);
        }

        Debug.Log("Environment sprites configured!");
    }

    private void SetupItemSprites()
    {
        string[] itemPaths = {
            "Assets/Sprites/Items/Food/Plates.png",
            "Assets/Sprites/Items/Furniture/Interior.png"
        };

        foreach (string path in itemPaths)
        {
            ConfigureSprite(path, 16, true, 16, 16);
        }

        Debug.Log("Item sprites configured!");
    }

    private void SetupAllSprites()
    {
        SetupCharacterSprites();
        SetupEnvironmentSprites();
        SetupItemSprites();

        // Setup animals
        string[] animalPaths = {
            "Assets/Sprites/Animals/Baby Chicken Yellow.png",
            "Assets/Sprites/Animals/Chicken Blonde  Green.png",
            "Assets/Sprites/Animals/Chicken Red.png",
            "Assets/Sprites/Animals/Female Cow Brown.png",
            "Assets/Sprites/Animals/Male Cow Brown.png"
        };

        foreach (string path in animalPaths)
        {
            ConfigureSprite(path, 16, false, 0, 0); // Animals might be single sprites
        }

        Debug.Log("All sprites configured!");
    }

    private void ConfigureSprite(string assetPath, int pixelsPerUnit, bool isMultiple, int cellWidth, int cellHeight)
    {
        TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
        if (importer != null)
        {
            // Basic settings
            importer.textureType = TextureImporterType.Sprite;
            importer.spritePixelsPerUnit = pixelsPerUnit;
            importer.filterMode = FilterMode.Point;
            importer.textureCompression = TextureImporterCompression.Uncompressed;

            // Sprite mode
            if (isMultiple)
            {
                importer.spriteImportMode = SpriteImportMode.Multiple;

                // Auto slice if cell size specified
                if (cellWidth > 0 && cellHeight > 0)
                {
                    // Note: Automatic sprite slicing now handled through Sprite Editor
                    // The spritesheet property is deprecated
                    Debug.Log("💡 Use Sprite Editor for manual slicing: Select sprite → Sprite Editor → Slice → Grid By Cell Size");
                }
            }
            else
            {
                importer.spriteImportMode = SpriteImportMode.Single;
            }

            // Apply settings
            AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
        }
        else
        {
            Debug.LogWarning($"Could not find sprite at path: {assetPath}");
        }
    }
}
#endif
