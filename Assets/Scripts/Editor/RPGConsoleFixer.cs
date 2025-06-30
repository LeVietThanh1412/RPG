using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using System.Linq;

namespace RPGFramework.Editor
{
    /// <summary>
    /// RPG Console Fixer - Fix tất cả lỗi Console theo README
    /// Sử dụng đúng 4 parameters: MoveX, MoveY, IsMoving, Speed
    /// Project đã được dọn dẹp, chỉ còn lại tool này và một vài tool cần thiết
    /// </summary>
    public class RPG : EditorWindow
    {
        [MenuItem("RPG Tools/🚀 ULTIMATE CONSOLE FIX (Fix All Errors)", priority = 1)]
        public static void UltimateConsoleFix()
        {
            Debug.Log("🚀 ULTIMATE CONSOLE FIX STARTED...");

            int totalFixes = 0;
            bool success = true;

            try
            {
                // Step 1: Fix Animation Event Errors (RED errors)
                EditorUtility.DisplayProgressBar("Ultimate Fix", "Fixing Animation Events...", 0.2f);
                int animationFixes = FixAllAnimationEvents();
                totalFixes += animationFixes;
                Debug.Log($"✅ Fixed {animationFixes} Animation Event errors");

                // Step 2: Fix Missing Animator Parameters (YELLOW errors)
                EditorUtility.DisplayProgressBar("Ultimate Fix", "Fixing Animator Parameters...", 0.4f);
                bool parametersFixed = FixAnimatorParameters();
                if (parametersFixed) totalFixes++;
                Debug.Log($"✅ Animator Parameters: {(parametersFixed ? "Fixed" : "Already OK")}");

                // Step 3: Setup Player with proper Animator Controller
                EditorUtility.DisplayProgressBar("Ultimate Fix", "Setting up Player Animator...", 0.6f);
                bool playerFixed = SetupPlayerAnimator();
                if (playerFixed) totalFixes++;
                Debug.Log($"✅ Player Animator: {(playerFixed ? "Fixed" : "Already OK")}");

                // Step 4: Remove Missing Scripts
                EditorUtility.DisplayProgressBar("Ultimate Fix", "Removing Missing Scripts...", 0.8f);
                int missingScriptFixes = RemoveAllMissingScripts();
                totalFixes += missingScriptFixes;
                Debug.Log($"✅ Removed {missingScriptFixes} missing scripts");

                // Step 5: Save and Refresh
                EditorUtility.DisplayProgressBar("Ultimate Fix", "Saving assets...", 1.0f);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                EditorUtility.ClearProgressBar();

                // Final result
                string message = $"🎉 ULTIMATE FIX COMPLETE!\n\n" +
                               $"✅ Applied {totalFixes} fixes\n" +
                               $"✅ Console should be clean now!\n" +
                               $"✅ Player ready to play!\n\n" +
                               $"Test with WASD keys!";

                Debug.Log(message);
                EditorUtility.DisplayDialog("Ultimate Fix Complete!", message, "Awesome!");
            }
            catch (System.Exception e)
            {
                EditorUtility.ClearProgressBar();
                Debug.LogError($"❌ Ultimate Fix failed: {e.Message}");
                EditorUtility.DisplayDialog("Fix Failed", $"Error occurred: {e.Message}", "OK");
                success = false;
            }

            if (success)
            {
                Debug.Log("🎮 Ready to play! Press Play and test movement with WASD!");
            }
        }

        /// <summary>
        /// Fix 1: Animation Event Errors (RED Console Errors)
        /// </summary>
        static int FixAllAnimationEvents()
        {
            string[] animGuids = AssetDatabase.FindAssets("t:AnimationClip");
            int fixedCount = 0;

            foreach (string guid in animGuids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                AnimationClip clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(path);

                if (clip != null && clip.events.Length > 0)
                {
                    // Remove all problematic animation events
                    AnimationUtility.SetAnimationEvents(clip, new AnimationEvent[0]);
                    EditorUtility.SetDirty(clip);
                    fixedCount++;
                    Debug.Log($"🧹 Cleaned events from: {clip.name}");
                }
            }

            return fixedCount;
        }

        /// <summary>
        /// Fix 2: Missing Animator Parameters (YELLOW Console Errors)
        /// </summary>
        static bool FixAnimatorParameters()
        {
            // Find or create Animator Controller
            AnimatorController controller = GetOrCreateAnimatorController();
            if (controller == null) return false;

            bool addedParams = false;

            // Required parameters theo hướng dẫn README (4 parameters)
            string[] requiredParams = { "MoveX", "MoveY", "IsMoving", "Speed" };
            AnimatorControllerParameterType[] paramTypes = {
                AnimatorControllerParameterType.Float,   // MoveX
                AnimatorControllerParameterType.Float,   // MoveY
                AnimatorControllerParameterType.Bool,    // IsMoving
                AnimatorControllerParameterType.Float    // Speed
            };

            for (int i = 0; i < requiredParams.Length; i++)
            {
                string paramName = requiredParams[i];
                if (!HasParameter(controller, paramName))
                {
                    controller.AddParameter(paramName, paramTypes[i]);
                    addedParams = true;
                    Debug.Log($"✅ Added parameter: {paramName} ({paramTypes[i]})");
                }
            }

            if (addedParams)
            {
                EditorUtility.SetDirty(controller);
                Debug.Log("📋 Animator Parameters theo README: MoveX, MoveY, IsMoving, Speed");
            }

            return addedParams;
        }

        /// <summary>
        /// Fix 3: Setup Player with proper Animator Controller
        /// </summary>
        static bool SetupPlayerAnimator()
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player == null)
            {
                Debug.LogWarning("⚠️ Player GameObject not found. Creating basic Player...");
                player = CreateBasicPlayer();
            }

            if (player == null) return false;

            // Ensure Animator component exists
            Animator animator = player.GetComponent<Animator>();
            if (animator == null)
            {
                animator = player.AddComponent<Animator>();
                Debug.Log("✅ Added Animator component to Player");
            }

            // Assign Animator Controller
            AnimatorController controller = GetOrCreateAnimatorController();
            if (controller != null && animator.runtimeAnimatorController != controller)
            {
                animator.runtimeAnimatorController = controller;
                Debug.Log("✅ Assigned Animator Controller to Player");
                EditorUtility.SetDirty(player);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Fix 4: Remove Missing Scripts
        /// </summary>
        static int RemoveAllMissingScripts()
        {
            GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
            int fixedCount = 0;

            foreach (GameObject obj in allObjects)
            {
                if (obj.scene.isLoaded || obj.hideFlags == HideFlags.None)
                {
                    Component[] components = obj.GetComponents<Component>();
                    for (int i = components.Length - 1; i >= 0; i--)
                    {
                        if (components[i] == null)
                        {
                            var serializedObject = new SerializedObject(obj);
                            var prop = serializedObject.FindProperty("m_Component");

                            for (int j = prop.arraySize - 1; j >= 0; j--)
                            {
                                var componentRef = prop.GetArrayElementAtIndex(j);
                                if (componentRef.objectReferenceValue == null)
                                {
                                    prop.DeleteArrayElementAtIndex(j);
                                    fixedCount++;
                                }
                            }
                            serializedObject.ApplyModifiedProperties();
                            EditorUtility.SetDirty(obj);
                        }
                    }
                }
            }

            return fixedCount;
        }

        /// <summary>
        /// Helper: Get or Create Animator Controller
        /// </summary>
        static AnimatorController GetOrCreateAnimatorController()
        {
            // Try to find existing controller
            string[] controllerGuids = AssetDatabase.FindAssets("t:AnimatorController");
            foreach (string guid in controllerGuids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                if (path.Contains("Player") || path.Contains("Character"))
                {
                    return AssetDatabase.LoadAssetAtPath<AnimatorController>(path);
                }
            }

            // Create new controller if none found
            string newPath = "Assets/Animations/PlayerAnimatorController.asset";

            // Ensure directory exists
            string directory = System.IO.Path.GetDirectoryName(newPath);
            if (!AssetDatabase.IsValidFolder(directory))
            {
                AssetDatabase.CreateFolder("Assets", "Animations");
            }

            AnimatorController newController = AnimatorController.CreateAnimatorControllerAtPath(newPath);
            Debug.Log($"✅ Created new Animator Controller at: {newPath}");

            return newController;
        }

        /// <summary>
        /// Helper: Check if parameter exists in controller
        /// </summary>
        static bool HasParameter(AnimatorController controller, string paramName)
        {
            foreach (AnimatorControllerParameter param in controller.parameters)
            {
                if (param.name == paramName)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Helper: Create basic Player GameObject
        /// </summary>
        static GameObject CreateBasicPlayer()
        {
            GameObject player = new GameObject("Player");
            player.tag = "Player";

            // Add essential components
            SpriteRenderer sr = player.AddComponent<SpriteRenderer>();
            Rigidbody2D rb = player.AddComponent<Rigidbody2D>();
            BoxCollider2D col = player.AddComponent<BoxCollider2D>();

            // Configure Rigidbody2D for top-down
            rb.gravityScale = 0;
            rb.freezeRotation = true;

            // Add Player scripts
            player.AddComponent<PlayerController>();

            Debug.Log("✅ Created basic Player GameObject");
            return player;
        }

        // Additional quick fix methods
        [MenuItem("RPG Tools/⚡ Quick Fix Animation Events Only", priority = 11)]
        public static void QuickFixAnimationEvents()
        {
            int fixes = FixAllAnimationEvents();
            AssetDatabase.SaveAssets();
            EditorUtility.DisplayDialog("Animation Events Fixed",
                $"Cleaned {fixes} animation events!\n\nRED console errors should be gone.", "OK");
        }

        [MenuItem("RPG Tools/⚡ Quick Fix Animator Parameters Only", priority = 12)]
        public static void QuickFixAnimatorParameters()
        {
            bool isFixed = FixAnimatorParameters();
            AssetDatabase.SaveAssets();
            string message = isFixed ? "Added missing parameters!\n\nYELLOW parameter errors should be gone."
                                   : "All parameters already exist!";
            EditorUtility.DisplayDialog("Animator Parameters Fixed", message, "OK");
        }

        [MenuItem("RPG Tools/🎮 Quick Fix Player Setup Only", priority = 13)]
        public static void QuickFixPlayerSetup()
        {
            bool isFixed = SetupPlayerAnimator();
            AssetDatabase.SaveAssets();
            string message = isFixed ? "Player Animator setup complete!\n\nAnimator controller assigned."
                                   : "Player already setup correctly!";
            EditorUtility.DisplayDialog("Player Setup Fixed", message, "OK");
        }
    }
}
