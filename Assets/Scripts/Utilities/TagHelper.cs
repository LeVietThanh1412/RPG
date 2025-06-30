using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public static class TagHelper
{
    public const string PLAYER_SPAWN_TAG = "PlayerSpawn";

    /// <summary>
    /// Kiểm tra và tạo tag nếu chưa tồn tại (chỉ hoạt động trong Editor)
    /// </summary>
    /// <param name="tagName">Tên tag cần tạo</param>
    /// <returns>True nếu tag tồn tại hoặc đã được tạo thành công</returns>
    public static bool EnsureTagExists(string tagName)
    {
#if UNITY_EDITOR
        // Kiểm tra tag đã tồn tại chưa
        if (IsTagDefined(tagName))
        {
            Debug.Log($"✅ Tag '{tagName}' already exists");
            return true;
        }

        // Tạo tag mới
        try
        {
            SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            SerializedProperty tagsProp = tagManager.FindProperty("tags");

            // Thêm tag mới
            tagsProp.InsertArrayElementAtIndex(0);
            SerializedProperty newTagProp = tagsProp.GetArrayElementAtIndex(0);
            newTagProp.stringValue = tagName;

            // Lưu thay đổi
            tagManager.ApplyModifiedProperties();

            Debug.Log($"✅ Successfully created tag: '{tagName}'");
            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ Failed to create tag '{tagName}': {e.Message}");
            Debug.LogError("💡 Please manually add the tag: Edit → Project Settings → Tags and Layers → Tags");
            return false;
        }
#else
        // Runtime - chỉ kiểm tra
        return IsTagDefined(tagName);
#endif
    }

    /// <summary>
    /// Kiểm tra tag có tồn tại không
    /// </summary>
    /// <param name="tagName">Tên tag</param>
    /// <returns>True nếu tag tồn tại</returns>
    public static bool IsTagDefined(string tagName)
    {
        try
        {
            GameObject.FindGameObjectWithTag(tagName);
            return true;
        }
        catch (UnityException)
        {
            return false;
        }
    }

    /// <summary>
    /// Tạo GameObject với PlayerSpawn tag tại vị trí hiện tại
    /// </summary>
    /// <param name="position">Vị trí spawn point</param>
    /// <returns>GameObject được tạo</returns>
    public static GameObject CreatePlayerSpawnPoint(Vector3 position = default)
    {
        // Đảm bảo tag tồn tại
        EnsureTagExists(PLAYER_SPAWN_TAG);

        // Tạo GameObject
        GameObject spawnPoint = new GameObject("PlayerSpawn");
        spawnPoint.transform.position = position;

        // Gán tag nếu có thể
        if (IsTagDefined(PLAYER_SPAWN_TAG))
        {
            spawnPoint.tag = PLAYER_SPAWN_TAG;
            Debug.Log($"✅ Created PlayerSpawn GameObject at {position}");
        }
        else
        {
            Debug.LogWarning($"⚠️ Created GameObject but couldn't assign tag '{PLAYER_SPAWN_TAG}'");
        }

        return spawnPoint;
    }

#if UNITY_EDITOR
    /// <summary>
    /// Menu item để tạo PlayerSpawn tag
    /// </summary>
    [MenuItem("RPG Tools/Create PlayerSpawn Tag")]
    public static void CreatePlayerSpawnTag()
    {
        if (EnsureTagExists(PLAYER_SPAWN_TAG))
        {
            EditorUtility.DisplayDialog("Success", $"Tag '{PLAYER_SPAWN_TAG}' is now available!", "OK");
        }
    }

    /// <summary>
    /// Menu item để tạo PlayerSpawn GameObject
    /// </summary>
    [MenuItem("RPG Tools/Create PlayerSpawn GameObject")]
    public static void CreatePlayerSpawnGameObject()
    {
        // Tạo tag trước
        EnsureTagExists(PLAYER_SPAWN_TAG);

        // Tạo GameObject tại center của Scene view
        Vector3 spawnPosition = Vector3.zero;
        if (SceneView.lastActiveSceneView != null)
        {
            spawnPosition = SceneView.lastActiveSceneView.camera.transform.position;
            spawnPosition.z = 0; // Top-down game
        }

        GameObject spawnPoint = CreatePlayerSpawnPoint(spawnPosition);

        // Focus vào object vừa tạo
        Selection.activeGameObject = spawnPoint;
        EditorGUIUtility.PingObject(spawnPoint);
    }
#endif
}
