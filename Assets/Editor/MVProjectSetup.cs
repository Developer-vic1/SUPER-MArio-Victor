using UnityEditor;
using UnityEditor.SceneManagement;

[InitializeOnLoad]
public static class MVProjectSetup
{
    static MVProjectSetup()
    {
        EditorApplication.playModeStateChanged += state => {
            if (state == PlayModeStateChange.EnteredPlayMode && SessionState.GetBool("MV.RunAcceptance", false))
            {
                SessionState.SetBool("MV.RunAcceptance", false);
                EditorApplication.delayCall += () => new UnityEngine.GameObject("Mario Acceptance Tests").AddComponent<MVAcceptanceProbe>();
            }
        };
        EditorApplication.delayCall += () => {
            // Play siempre entra por el menú; abrir un nivel para editar no altera el arranque.
            var menu = AssetDatabase.LoadAssetAtPath<SceneAsset>("Assets/MarioVictor/Scenes/MV_Menu.unity");
            if (menu != null) EditorSceneManager.playModeStartScene = menu;
            EditorSettings.defaultBehaviorMode = EditorBehaviorMode.Mode2D;
        };
    }

    [MenuItem("Mario Victor/3. Ejecutar pruebas de aceptación 2D")]
    public static void RunAcceptance()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
            throw new System.InvalidOperationException("Detén Play antes de ejecutar la suite.");
        EditorSceneManager.playModeStartScene = AssetDatabase.LoadAssetAtPath<SceneAsset>("Assets/MarioVictor/Scenes/MV_Menu.unity");
        SessionState.SetBool("MV.RunAcceptance", true);
        EditorApplication.isPlaying = true;
    }

    [MenuItem("Mario Victor/4. Integrar campaña en el nivel original completo")]
    public static void IntegrateOriginalLevel()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode) throw new System.InvalidOperationException("Detén Play primero.");
        var scene = EditorSceneManager.OpenScene("Assets/Scenes/1-1.unity");
        var controller = UnityEngine.Object.FindFirstObjectByType<MVGameController>();
        if (controller == null) controller = new UnityEngine.GameObject("Campaña Mario Victor - nivel original completo").AddComponent<MVGameController>();
        controller.stageId = 1;
        controller.authoredLevel = true;
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        var list = new System.Collections.Generic.List<EditorBuildSettingsScene>();
        foreach (var item in EditorBuildSettings.scenes)
        {
            if (item.path == "Assets/Scenes/1-1.unity") continue;
            list.Add(item.path == "Assets/MarioVictor/Scenes/MV_1-1.unity" ? new EditorBuildSettingsScene("Assets/Scenes/1-1.unity", true) : item);
        }
        EditorBuildSettings.scenes = list.ToArray();
        AssetDatabase.SaveAssets();
        EditorSceneManager.OpenScene("Assets/MarioVictor/Scenes/MV_Menu.unity");
    }
}
