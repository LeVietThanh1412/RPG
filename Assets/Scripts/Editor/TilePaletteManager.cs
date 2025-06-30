using UnityEngine;
using UnityEditor;
using System.IO;

public class TilePaletteManager : EditorWindow
{
    [MenuItem("RPG Tools/Environment/Tile Palette Manager")]
    public static void ShowWindow()
    {
        GetWindow<TilePaletteManager>("Tile Palette Manager");
    }

    private string paletteName = "Environment_Palette";
    private string tilesetPath = "Assets/Sprites/Environment/";

    private void OnGUI()
    {
        GUILayout.Label("Tile Palette Manager", EditorStyles.boldLabel);
        GUILayout.Space(10);

        paletteName = EditorGUILayout.TextField("Palette Name:", paletteName);
        tilesetPath = EditorGUILayout.TextField("Tileset Path:", tilesetPath);

        GUILayout.Space(10);

        if (GUILayout.Button("Create Palette"))
        {
            CreateTilePalette();
        }

        if (GUILayout.Button("Setup Environment Folders"))
        {
            SetupEnvironmentFolders();
        }

        GUILayout.Space(10);
        GUILayout.Label("Environment Organization:", EditorStyles.helpBox);
        GUILayout.Label("• Terrain: Grass, dirt, stone tiles");
        GUILayout.Label("• Nature: Trees, rocks, water");
        GUILayout.Label("• Structures: Buildings, fences, paths");
        GUILayout.Label("• Details: Flowers, small objects");
    }

    private void CreateTilePalette()
    {
        // Create Palettes folder if it doesn't exist
        string palettesPath = "Assets/Palettes";
        if (!AssetDatabase.IsValidFolder(palettesPath))
        {
            AssetDatabase.CreateFolder("Assets", "Palettes");
        }

        // Create specific palette folder
        string paletteFolder = $"{palettesPath}/{paletteName}";
        if (!AssetDatabase.IsValidFolder(paletteFolder))
        {
            AssetDatabase.CreateFolder(palettesPath, paletteName);
        }

        Debug.Log($"Tile palette folder created: {paletteFolder}");
        Debug.Log($"Now import your tileset sprites to {tilesetPath} and create a palette in {paletteFolder}");
    }

    private void SetupEnvironmentFolders()
    {
        string basePath = "Assets/Sprites/Environment";

        string[] subfolders = {
            "Terrain",
            "Nature",
            "Structures",
            "Details",
            "Water",
            "Sky"
        };

        foreach (string folder in subfolders)
        {
            string fullPath = $"{basePath}/{folder}";
            if (!AssetDatabase.IsValidFolder(fullPath))
            {
                string parentFolder = Path.GetDirectoryName(fullPath).Replace("\\", "/");
                string folderName = Path.GetFileName(fullPath);
                AssetDatabase.CreateFolder(parentFolder, folderName);
                Debug.Log($"Created folder: {fullPath}");
            }
        }

        // Create README for each folder
        CreateFolderReadme($"{basePath}/Terrain", "Ground tiles: grass, dirt, stone, sand");
        CreateFolderReadme($"{basePath}/Nature", "Natural objects: trees, rocks, bushes");
        CreateFolderReadme($"{basePath}/Structures", "Buildings, fences, paths, bridges");
        CreateFolderReadme($"{basePath}/Details", "Small decorative items: flowers, signs");
        CreateFolderReadme($"{basePath}/Water", "Water tiles and water-related objects");
        CreateFolderReadme($"{basePath}/Sky", "Sky, clouds, distant background elements");

        AssetDatabase.Refresh();
        Debug.Log("Environment folder structure created successfully!");
    }

    private void CreateFolderReadme(string folderPath, string description)
    {
        string readmePath = $"{folderPath}/README.md";
        if (!File.Exists(readmePath))
        {
            string content = $"# {Path.GetFileName(folderPath)} Assets\n\n{description}\n\n## Organization:\n- Keep similar assets together\n- Use descriptive filenames\n- Consider tile size consistency (16x16, 32x32, etc.)";
            File.WriteAllText(readmePath, content);
        }
    }
}
