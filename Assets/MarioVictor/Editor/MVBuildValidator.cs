using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class MVBuildValidator
{
    private static readonly string Prefix = "Assets/MarioVictor/Scenes/";
    private static readonly string[] Names = { "MV_Menu", "MV_1-1", "MV_1-2", "MV_2-2", "MV_8-4", "MV_GameOver", "MV_Victoria" };
    private static readonly string[] KeySprites = { "Mario_Small_Idle", "GroundBlock", "UndergroundBrick", "Pez", "Medusa", "BowserSombra", "SelloEstelar", "Tiko", "Portal" };
    private static readonly string[] KeyAudio = { "super-mario-bros", "mario-bros-1", "super-mario-bros-castillo", "mario-bros-game-over-1", "mario-moneda" };

    [MenuItem("Mario Victor/0. Reparar escenas generadas (solo si fallan)")]
    public static void RepairScenes()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
            throw new Exception("Sal de modo Play antes de reconstruir escenas.");
        if (!EditorApplication.isBatchMode && !EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
        List<EditorBuildSettingsScene> targetScenes = new List<EditorBuildSettingsScene>();
        for (int i=0;i<Names.Length;i++)
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            var root = new GameObject("MV_SceneRoot_" + Names[i]);
            root.AddComponent<MVSceneEntry>().stageId = i;
            string file = Prefix + Names[i] + ".unity";
            if (!EditorSceneManager.SaveScene(scene,file)) throw new Exception("No se pudo guardar escena: " + file);
            targetScenes.Add(new EditorBuildSettingsScene(file,true));
        }
        EditorBuildSettings.scenes = targetScenes.ToArray();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("MarioVictor: siete escenas reconstruidas. Ejecuta la validacion.");
    }

    [MenuItem("Mario Victor/1. Verificar proyecto completo")]
    public static void Validate()
    {
        List<string> faults = Inspect();
        string path = Path.Combine(Directory.GetCurrentDirectory(), "MarioVictor_VALIDACION.txt");
        var msg = faults.Count == 0 ? "OK: escenas, sprites, audio y build list presentes." : "FALLOS:\n" + string.Join("\n", faults.ToArray());
        File.WriteAllText(path, "Mario Victor - Verificacion estatica de Editor\nUnity: " + Application.unityVersion + "\n" + DateTime.Now + "\n" + msg);
        if (faults.Count != 0) throw new Exception("MarioVictor: verificacion FALLIDA. Revisa MarioVictor_VALIDACION.txt\n" + msg);
        Debug.Log("MarioVictor: comprobacion OK. Ver MarioVictor_VALIDACION.txt. No reemplaza pruebas manuales de jugabilidad.");
    }

    private static List<string> Inspect()
    {
        List<string> faults = new List<string>();
        var enabled = new List<string>();
        foreach (var item in EditorBuildSettings.scenes) if (item.enabled) enabled.Add(item.path);
        for (int i=0;i<Names.Length;i++)
        {
            string p = Prefix + Names[i] + ".unity";
            if (AssetDatabase.LoadAssetAtPath<SceneAsset>(p) == null) faults.Add("Escena ausente: " + p);
            if (!enabled.Contains(p)) faults.Add("Escena deshabilitada o ausente de Build Profiles: " + p);
        }
        if (enabled.Count < Names.Length || enabled[0] != Prefix + Names[0] + ".unity")
            faults.Add("La primera escena de arranque debe ser MV_Menu.unity");
        foreach (string s in KeySprites) if (Resources.Load<Sprite>("MarioVictor/Sprites/"+s) == null) faults.Add("Sprite ausente o no importado como Sprite: " + s);
        foreach (string s in KeyAudio) if (Resources.Load<AudioClip>("MarioVictor/Audio/"+s) == null) faults.Add("Audio ausente o no importado: " + s);
        if (!Application.unityVersion.StartsWith("6000.3.")) Debug.LogWarning("MarioVictor: version validada para Unity 6000.3.x. Esta abierta: " + Application.unityVersion);
        return faults;
    }

    [MenuItem("Mario Victor/2. Verificar y compilar Windows 64-bit")]
    public static void VerifyAndBuild()
    {
        Validate();
        string outputFolder = Path.Combine(Directory.GetCurrentDirectory(), "Builds");
        Directory.CreateDirectory(outputFolder);
        List<string> scenes = new List<string>();
        foreach (var item in EditorBuildSettings.scenes) if (item.enabled && item.path.StartsWith(Prefix)) scenes.Add(item.path);
        var opts = new BuildPlayerOptions();
        opts.scenes = scenes.ToArray();
        opts.locationPathName = Path.Combine(outputFolder, "SuperMarioVictor.exe");
        opts.target = BuildTarget.StandaloneWindows64;
        opts.options = BuildOptions.None;
        BuildReport report = BuildPipeline.BuildPlayer(opts);
        string receipt = Path.Combine(Directory.GetCurrentDirectory(), "MarioVictor_BUILD_RESULTADO.txt");
        File.WriteAllText(receipt, "Compilacion Windows64 Unity " + Application.unityVersion + "\n" + report.summary.result + "\nErrores: " + report.summary.totalErrors + "\nAdvertencias: " + report.summary.totalWarnings + "\nRuta: " + opts.locationPathName);
        if (report.summary.result != BuildResult.Succeeded) throw new Exception("Compilacion FALLIDA: ver MarioVictor_BUILD_RESULTADO.txt");
        Debug.Log("Mario Victor: build de Windows correcto en Builds/SuperMarioVictor.exe");
    }
}
