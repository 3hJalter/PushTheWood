using UnityEditor;
using UnityEditor.SceneManagement;

namespace VinhLB
{
    public static class EditorTools
    {
        [MenuItem("Tools/VinhLB/Scenes/GameDemo")]
        private static void OpenGameDemoScene()
        {
            OpenScene("GameDemo");
        }

        [MenuItem("Tools/VinhLB/Scenes/ChooseLevel")]
        private static void OpenChooseLevelScene()
        {
            OpenScene("ChooseLevel");
        }

        [MenuItem("Tools/VinhLB/Scenes/MapEditor")]
        private static void OpenMapEditorScene()
        {
            OpenScene("MapEditor");
        }

        [MenuItem("Tools/VinhLB/Scenes/BuildingTest")]
        private static void OpenBuildingTestScene()
        {
            OpenScene("BuildingTest");
        }

        [MenuItem("Tools/VinhLB/Scenes/BendingTest")]
        private static void OpenBendingTestScene()
        {
            OpenScene("BendingTest");
        }

        private static void OpenScene(string sceneName)
        {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                EditorSceneManager.OpenScene($"Assets/_Game/_Scenes/{sceneName}.unity", OpenSceneMode.Single);
        }
    }
}