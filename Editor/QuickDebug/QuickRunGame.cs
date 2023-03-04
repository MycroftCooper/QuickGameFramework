using UnityEditor;

public class QuickRunGame : Editor {
    private const string START_SCENE_PATH = "Assets/GameMain/Scenes/EntryScene.unity";

    [MenuItem("QuickDebug/Run Game _%g")]
    public static void RunGame() {
        UnityEditor.SceneManagement.EditorSceneManager.OpenScene(START_SCENE_PATH);
        EditorApplication.ExecuteMenuItem("Edit/Play");
    }
}