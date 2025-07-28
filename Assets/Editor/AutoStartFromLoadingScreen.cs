#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
public class AutoStartFromLoadingScreen
{
    private const string PreviousSceneKey = "AutoStartFromLoadingScreen.PreviousScene";

    static AutoStartFromLoadingScreen()
    {
        EditorApplication.playModeStateChanged += HandlePlayModeStateChanged;
    }

    private static void HandlePlayModeStateChanged(PlayModeStateChange state)
    {
        switch (state)
        {
            case PlayModeStateChange.ExitingEditMode:

                string currentScene = SceneManager.GetActiveScene().path;
                EditorPrefs.SetString(PreviousSceneKey, currentScene);

                string loginScenePath = SceneUtility.GetScenePathByBuildIndex(0);
                if (currentScene != loginScenePath)
                {
                    bool userWantsToSave = EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
                    if (userWantsToSave)
                    {
                        EditorSceneManager.OpenScene(loginScenePath);
                    }
                    else
                    {
                        EditorApplication.isPlaying = false;
                    }
                }

                break;

            case PlayModeStateChange.EnteredEditMode:

                if (EditorPrefs.HasKey(PreviousSceneKey))
                {
                    string previousScene = EditorPrefs.GetString(PreviousSceneKey);
                    string actualScene = SceneManager.GetActiveScene().path;

                    if (actualScene != previousScene)
                    {
                        EditorSceneManager.OpenScene(previousScene);
                    }

                    EditorPrefs.DeleteKey(PreviousSceneKey);
                }

                break;
        }
    }
}
#endif