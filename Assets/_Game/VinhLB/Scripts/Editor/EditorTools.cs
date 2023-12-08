#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace VinhLB
{
    public static class EditorTools
    {
        [MenuItem("Tools/VinhLB/Scenes/BuildingTest")]
        private static void OpenBuildingTestScene()
        {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                EditorSceneManager.OpenScene("Assets/_Game/VinhLB/Scenes/BuildingTest.unity", OpenSceneMode.Single);
            }
        }

        [MenuItem("Tools/VinhLB/Scenes/GameDemo")]
        private static void OpenGameDemoScene()
        {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                EditorSceneManager.OpenScene("Assets/_Game/_Scenes/GameDemo.unity", OpenSceneMode.Single);
            }
        }
        
        [MenuItem("Tools/VinhLB/Scenes/MapEditor")]
        private static void OpenMapEditorScene()
        {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                EditorSceneManager.OpenScene("Assets/_Game/_Scenes/MapEditor.unity", OpenSceneMode.Single);
            }
        }
    }
}
#endif