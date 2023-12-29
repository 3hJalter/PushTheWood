using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
public class SceneHotkey : MonoBehaviour
{
    public static void OpenScene(string sceneName)
    {
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            EditorSceneManager.OpenScene("Assets/_Game/_Scenes/" + sceneName + ".unity");
        }
    }

    [MenuItem("Open Scene/GameDemo")]
    public static void OpenSceneGameDemo()
    {
        OpenScene("GameDemo");
    }

    [MenuItem("Open Scene/MapEditor")]
    public static void OpenSceneMapEditor()
    {
        OpenScene("MapEditor");
    }
}

#endif

