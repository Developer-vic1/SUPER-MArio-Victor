using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Herramienta de autoría (editor): genera una escena de victoria y un castillo DEMOSTRATIVO
/// sin alterar el nivel 1-1 ni sobrescribir escenas que ya existan.
/// Menú: Super Mario Victor > Generar final original.
/// </summary>
public static class CrearFinalMarioVictor
{
    private const string VictoryPath = "Assets/Scenes/VictoriaFinal.unity";
    private const string DemoPath = "Assets/Scenes/CastilloFinal_Prototipo.unity";

    [MenuItem("Super Mario Victor/Generar final original y castillo de prueba")]
    private static void Generate()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
        {
            EditorUtility.DisplayDialog("Mario Victor", "Sal de Play antes de crear escenas.", "Entendido");
            return;
        }
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;

        if (!AssetDatabase.IsValidFolder("Assets/Scenes"))
            AssetDatabase.CreateFolder("Assets", "Scenes");

        if (AssetDatabase.LoadAssetAtPath<SceneAsset>(VictoryPath) == null)
            CreateVictoryScene();
        else
            Debug.Log("VictoriaFinal ya existe. No se sobrescribio.");

        if (AssetDatabase.LoadAssetAtPath<SceneAsset>(DemoPath) == null)
            CreateDemoScene();
        else
            Debug.Log("CastilloFinal_Prototipo ya existe. No se sobrescribio.");

        RegisterInBuild(VictoryPath);
        RegisterInBuild(DemoPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("Mario Victor",
            "Final original generado. Abre Assets/Scenes/CastilloFinal_Prototipo.unity y presiona Play.\n" +
            "Control: flechas / A-D + espacio. Busca los 3 sellos.\n" +
            "La escena de prueba NO reemplaza el castillo 8-4 definitivo.", "Perfecto");
    }

    private static void CreateVictoryScene()
    {
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        GameObject cameraObject = new GameObject("Main Camera", typeof(Camera), typeof(AudioListener));
        cameraObject.tag = "MainCamera";
        cameraObject.transform.position = new Vector3(0, 0, -10);
        Camera camera = cameraObject.GetComponent<Camera>();
        camera.orthographic = true;
        camera.backgroundColor = new Color(0.05f, 0.07f, 0.15f);
        camera.clearFlags = CameraClearFlags.SolidColor;

        GameObject controller = new GameObject("Final original - El Reino Liberado");
        controller.AddComponent<FinalVictoryScene>();
        EditorSceneManager.SaveScene(scene, VictoryPath);
    }

    private static void CreateDemoScene()
    {
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        GameObject cam = new GameObject("Main Camera", typeof(Camera), typeof(AudioListener), typeof(FinalDemoCamera));
        cam.tag = "MainCamera";
        cam.transform.position = new Vector3(0, 2.5f, -10f);
        Camera camera = cam.GetComponent<Camera>();
        camera.orthographic = true;
        camera.orthographicSize = 5.4f;
        camera.backgroundColor = new Color(0.19f, 0.1f, 0.24f);
        camera.clearFlags = CameraClearFlags.SolidColor;

        Sprite groundSprite = LoadSprite("GroundBlock.png");
        Sprite brickSprite = LoadSprite("UndergroundBrick.png");
        Sprite mystery = LoadSprite("MysteryBlock.png");
        Sprite marioSprite = LoadSprite("Mario_Small_Idle.png");
        Sprite bossSprite = LoadSprite("Koopa_Walk1.png");
        if (groundSprite == null || marioSprite == null)
            Debug.LogWarning("No se encontraron todos los sprites del proyecto base. Revisa Assets/Sprites.");

        // Terreno: las zonas con lava pueden superarse saltando.
        for (int x = -5; x <= 49; x++)
        {
            CreateSprite("Bloque de suelo " + x, groundSprite, new Vector3(x, -1.5f, 0), Vector3.one,
                new Color(0.53f, 0.35f, 0.44f));
        }
        // Doble capa de piedras subterráneas para simular un castillo propio.
        for (int x = -5; x <= 49; x += 3)
        {
            CreateSprite("Almenas " + x, brickSprite, new Vector3(x, 4.4f, 0f), Vector3.one,
                new Color(0.42f, 0.3f, 0.59f));
        }
        GameObject terrain = new GameObject("Suelo continuo", typeof(BoxCollider2D));
        terrain.transform.position = new Vector3(22f, -1.5f, 0);
        terrain.GetComponent<BoxCollider2D>().size = new Vector2(56f, 1f);

        GameObject mario = CreateSprite("Mario - Demo", marioSprite, Vector3.zero,
            new Vector3(1.5f, 1.5f, 1), Color.white);
        mario.tag = "Player";
        mario.AddComponent<BoxCollider2D>().size = new Vector2(0.65f, 0.65f);
        var rigid = mario.AddComponent<Rigidbody2D>();
        rigid.gravityScale = 3f;
        rigid.freezeRotation = true;
        rigid.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rigid.interpolation = RigidbodyInterpolation2D.Interpolate;
        mario.AddComponent<FinalDemoPlayer>().spawnPoint = Vector3.zero;
        cam.GetComponent<FinalDemoCamera>().target = mario.transform;

        GameObject managerObject = new GameObject("Sistema Final - 3 sellos");
        FinalCastleManager manager = managerObject.AddComponent<FinalCastleManager>();
        manager.requiredSeals = 3;
        manager.victoryScene = "VictoriaFinal";
        manager.audioSource = managerObject.AddComponent<AudioSource>();

        float[] sealPositions = { 8f, 21f, 35f };
        Color[] sealColors = { new Color(0.96f, 0.64f, 0.25f), new Color(0.28f, 0.86f, 0.95f), new Color(0.96f, 0.41f, 0.72f) };
        for (int i = 0; i < sealPositions.Length; i++)
        {
            GameObject sealObject = CreateSprite("Sello " + (i + 1), mystery,
                new Vector3(sealPositions[i], 0.25f, 0), Vector3.one * 1.7f, sealColors[i]);
            var trigger = sealObject.AddComponent<BoxCollider2D>();
            trigger.isTrigger = true;
            trigger.size = new Vector2(0.85f, 0.85f);
            var seal = sealObject.AddComponent<FinalCastleSeal>();
            seal.castleManager = manager;
        }

        GameObject bowser = CreateSprite("BOWSER - MARCADOR TEMPORAL (sustituir sprite)", bossSprite,
            new Vector3(44f, -0.1f, 0f), Vector3.one * 3.5f,
            new Color(0.59f, 0.25f, 0.41f, 1f));
        manager.bowserVisual = bowser.transform;

        CreateLava(brickSprite, 14f);
        CreateLava(brickSprite, 27f);
        CreateLava(brickSprite, 40f);

        EditorSceneManager.SaveScene(scene, DemoPath);
    }

    private static void CreateLava(Sprite sprite, float x)
    {
        GameObject hazard = CreateSprite("Lava " + x, sprite,
            new Vector3(x, -0.9f, 0), new Vector3(2.2f, 0.7f, 1f),
            new Color(1f, 0.28f, 0.04f, 1f));
        var collider = hazard.AddComponent<BoxCollider2D>();
        collider.isTrigger = true;
        collider.size = Vector2.one;
        hazard.AddComponent<FinalDemoLava>();
    }

    private static Sprite LoadSprite(string fileName)
    {
        return AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/" + fileName);
    }

    private static GameObject CreateSprite(string name, Sprite sprite, Vector3 position, Vector3 scale, Color color)
    {
        GameObject go = new GameObject(name, typeof(SpriteRenderer));
        go.transform.position = position;
        go.transform.localScale = scale;
        SpriteRenderer renderer = go.GetComponent<SpriteRenderer>();
        renderer.sprite = sprite;
        renderer.color = color;
        return go;
    }

    private static void RegisterInBuild(string scenePath)
    {
        // En Unity 6 la escena se agrega a la lista del perfil de compilación activo.
        var scenes = EditorBuildSettings.scenes.ToList();
        if (scenes.Any(s => s.path == scenePath)) return;
        scenes.Add(new EditorBuildSettingsScene(scenePath, true));
        EditorBuildSettings.scenes = scenes.ToArray();
    }
}
