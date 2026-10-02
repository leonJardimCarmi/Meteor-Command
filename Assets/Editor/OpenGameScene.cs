using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

// A freshly cloned project opens in an empty, untitled scene. This opens the game scene instead, so whoever opens
// the project sees the game straight away and only has to press Play. It is an editor script, so it is not part
// of the built game.
[InitializeOnLoad]
public static class OpenGameScene
{
    private const string GameScenePath = "Assets/Scenes/Game.unity";

    static OpenGameScene()
    {
        EditorApplication.delayCall += OpenIfEmpty;
    }

    private static void OpenIfEmpty()
    {
        Scene active = SceneManager.GetActiveScene();
        bool isUntitled = string.IsNullOrEmpty(active.path);

        if (isUntitled && !active.isDirty && !EditorApplication.isPlayingOrWillChangePlaymode)
        {
            EditorSceneManager.OpenScene(GameScenePath);
        }
    }
}
