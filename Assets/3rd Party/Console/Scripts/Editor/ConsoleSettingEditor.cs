// #if UNITY_EDITOR

// using UnityEditor;
// using UnityEngine;
// using TIM; // Add the missing using directive


// namespace TIM.Editor
// {
//     public class ConsoleSettingEditor : EditorWindow
//     {
//         [MenuItem("Tools/TIM.Console")]
//         private static void ShowWindow()
//         {
//             var window = GetWindow<ConsoleSettingEditor>();
//             window.titleContent = new GUIContent("TIM.Console", AssetDatabase.LoadAssetAtPath<Texture>("Assets/TIM/Console/Sprites/Console icon mini.png"));
//             window.Show();
//         }



//         private void OnGUI()
//         {
//             // Display the ConsoleSetting instance in the editor window
//             ConsoleSetting instance = ConsoleSetting.Instance;
//             if (instance != null)
//             {
//                 UnityEditor.Editor editor = UnityEditor.Editor.CreateEditor(instance);
//                 editor.OnInspectorGUI();
//             }
//         }
//     }
// }

// #endif