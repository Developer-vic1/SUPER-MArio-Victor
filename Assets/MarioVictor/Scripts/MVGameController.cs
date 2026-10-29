using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Super Mario Víctor - campaña autocontenida.
/// Stage 0 menú, 1 tierra, 2 subterráneo, 3 agua, 4 castillo, 5 Game Over, 6 victoria.
/// Los niveles se construyen a partir de escenas guardadas que existen en el repositorio.
/// </summary>
public sealed class MVGameController : MonoBehaviour
{
    public int stageId;
    public bool authoredLevel;
    public Player legacyHero { get; private set; }
    public MVHero hero { get; private set; }
    public bool paused { get; private set; }
    public int seals { get; private set; }
    public FinalCastleManager castle { get; private set; }
    public static int Lives => lives;
    public static int CoinsCollected => coins;
    public static int Score => score;
    private bool bossDead;
    private bool exiting;
    private bool showHelp;
    private float hintUntil;
    private string hint = "";
    private Camera cam;
    private float worldWidth = 68f;
    private float timeLeft;
    private Texture2D solidTexture;
    private Sprite[] runAnim;
    private float ambientTimer;
    private float constructionOffset;

    private static int lives = 3;
    private static int coins;
    private static int score;
    private static bool initialized;
    private static readonly string[] stageScene = { "MV_Menu", "1-1", "MV_1-2", "MV_2-2", "MV_8-4", "MV_GameOver", "MV_Victoria" };
    private static readonly string[] stageName = { "MENU", "MUNDO 1-1", "MUNDO 1-2", "MUNDO 2-2", "MUNDO 8-4", "GAME OVER", "VICTORIA" };

    public static Sprite Sprite(string spriteName) { return Resources.Load<Sprite>("MarioVictor/Sprites/" + spriteName); }

    private void Start()
    {
        Time.timeScale = 1f;
        if (!initialized) { lives = 3; coins = 0; score = 0; initialized = true; }
        if (authoredLevel) cam = Camera.main;
        else SetupCamera();
        if (stageId == 0) { MVAudio.Music("super-mario-bros"); SceneBackdrop(new Color(.035f,.075f,.15f)); gameObject.AddComponent<MVMenuScreen>().game = this; }
        else if (stageId == 5) { MVAudio.StopMusic(); MVAudio.OneShot("mario-bros-game-over-1"); SceneBackdrop(new Color(.12f,.07f,.13f)); }
        else if (stageId == 6) { SceneBackdrop(new Color(.16f,.29f,.41f)); VictoryDecor(); gameObject.AddComponent<FinalVictoryScene>().campaignMode = true; }
        else
        {
            timeLeft = stageId == 4 ? 720f : 600f;
            if (authoredLevel)
            {
                legacyHero = FindFirstObjectByType<Player>();
                MVAudio.Music("super-mario-bros");
            }
            else BuildStage();
        }
    }

    private void SetupCamera()
    {
        var go = new GameObject("Main Camera - Mario Victor");
        go.tag = "MainCamera";
        go.transform.position = new Vector3(8f, 0, -10f);
        cam = go.AddComponent<Camera>();
        cam.orthographic = true;
        cam.orthographicSize = 5.25f;
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = Color.black;
        go.AddComponent<AudioListener>();
    }

    private void SceneBackdrop(Color color) { if (cam != null) cam.backgroundColor = color; }
    private void Update()
    {
        if (stageId < 1 || stageId > 4) return;
        if (MVKeys.Pressed(KeyCode.Escape))
        {
            paused = !paused;
            Time.timeScale = paused ? 0f : 1f;
        }
        if (paused || exiting) return;
        timeLeft -= Time.deltaTime;
        if (timeLeft <= 0f && hero != null) hero.Hit();
        if (timeLeft <= 0f && legacyHero != null && !legacyHero.dead) legacyHero.Death();
        if (hero == null) return;
        float halfWidth = cam.orthographicSize * cam.aspect;
        float desiredX = Mathf.Clamp(hero.transform.position.x + 2.4f, Mathf.Min(halfWidth, worldWidth / 2f), Mathf.Max(halfWidth, worldWidth - halfWidth));
        float desiredY = stageId == 3 ? Mathf.Clamp(hero.transform.position.y * .22f, -.4f, .65f) : 0;
        cam.transform.position = Vector3.Lerp(cam.transform.position, new Vector3(desiredX, desiredY, -10f), 4.5f * Time.deltaTime);
        ambientTimer += Time.deltaTime;
        if (stageId == 3 && ambientTimer > .3f) { ambientTimer = 0; Bubble(hero.transform.position + Vector3.right * Random.Range(-2f, 7f) + Vector3.down * Random.Range(0f, 2f)); }
    }

    public void StartNewGame()
    {
        lives = 3; coins = 0; score = 0; initialized = true;
        Load(1);
    }
    public void ReturnToMenu() { Time.timeScale = 1f; Load(0); }
    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
    private static void Load(int sceneId)
    {
        SceneManager.LoadScene(stageScene[Mathf.Clamp(sceneId, 0, stageScene.Length-1)]);
    }

    public void AddScore(int points) { score += points; }
    public void Coin()
    {
        coins++;
        score += 50;
        MVAudio.OneShot("mario-moneda", .5f);
        if (coins >= 100) { coins -= 100; ExtraLife(); }
    }
    public void ExtraLife() { lives++; MVAudio.OneShot("mario-bros-vida", .75f); ShowHint("¡VIDA EXTRA!"); }
    public void Seal()
    {
        if (stageId != 4 || seals >= 3) return;
        seals++;
        score += 250;
        MVAudio.OneShot("mario-monedas-varias-veces", .7f);
        ShowHint("SELLO " + seals + "/3: " + (seals == 3 ? "¡ESCUDO DESACTIVADO!" : "El castillo pierde poder"));
        if (seals == 3) ExtraLife();
    }
    public void BossDefeated()
    {
        bossDead = true;
        score += 3000;
        MVAudio.OneShot("mario-bros-vida-1", .7f);
        ShowHint("¡EL GUARDIÁN HA CAÍDO! Busca el portal de la estrella.");
    }
    public void ShowHint(string message) { hint = message; hintUntil = Time.time + 3.7f; }

    public void LoseLife()
    {
        if (exiting) return;
        exiting = true;
        lives--;
        MVAudio.OneShot("mario-bros-game-over-1", .62f);
        StartCoroutine(RestartAfterDeath());
    }
    private IEnumerator RestartAfterDeath()
    {
        if (hero != null) { hero.controlEnabled = false; var image = hero.GetComponent<SpriteRenderer>(); if (image != null) image.sprite = Sprite("Mario_Small_Death"); }
        yield return new WaitForSeconds(1.5f);
        Load(lives > 0 ? stageId : 5);
    }
    public void FinishLevel()
    {
        if (exiting) return;
        if (stageId == 4 && (!bossDead || castle == null || !castle.defeatComplete))
        {
            ShowHint(seals < 3 ? "Primero activa los tres cristales." : "Derrota al guardián saltando sobre él tres veces.");
            if (hero != null) { hero.transform.position += Vector3.left * 2f; hero.Stomp(); }
            return;
        }
        exiting = true;
        if (hero != null) hero.controlEnabled = false;
        MVAudio.OneShot("mario-bros-vida-1", .52f);
        score += Mathf.Max(0, (int)timeLeft * 10);
        StartCoroutine(NextStageRoutine());
    }
    private IEnumerator NextStageRoutine() { yield return new WaitForSeconds(1.25f); Load(stageId == 4 ? 6 : stageId + 1); }

    // =========================== ESCENARIOS ===========================
    private GameObject Visual(string name, string spriteName, float x, float y, float scale = 1f, Color? tint = null, int order = 0)
    {
        GameObject o = new GameObject(name);
        o.transform.position = new Vector3(x + constructionOffset, y, 0);
        o.transform.localScale = new Vector3(scale, scale, 1);
        SpriteRenderer r = o.AddComponent<SpriteRenderer>();
        r.sprite = Sprite(spriteName);
        r.sortingOrder = order;
        r.color = tint ?? Color.white;
        if (r.sprite == null) Debug.LogWarning("MarioVictor sprite faltante: " + spriteName);
        return o;
    }
    private void Solid(string sprite, float x, float y, int count = 1)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject o = Visual("Solido " + sprite + " " + (x+i), sprite, x+i, y);
            o.AddComponent<BoxCollider2D>().size = new Vector2(.99f, .99f);
        }
    }
    private void Platform(int start, int end, float y, string sprite)
    {
        for (int x = start; x <= end; x++) Solid(sprite, x, y);
    }
    private bool IsHole(int x)
    {
        if (stageId == 2) x %= 65;
        if (stageId == 4) x %= 56;
        if (stageId == 1) return (x >= 18 && x <= 20) || (x >= 39 && x <= 40);
        if (stageId == 2) return (x >= 18 && x <= 20) || (x >= 40 && x <= 42);
        if (stageId == 4) return (x >= 16 && x <= 18) || (x >= 32 && x <= 35) || (x >= 47 && x <= 50);
        return false;
    }
    private void BuildStage()
    {
        bool water = stageId == 3;
        worldWidth = stageId == 4 ? 190f : (water ? 207f : stageId == 2 ? 195f : 65f);
        Color bg = stageId == 1 ? new Color(.34f,.66f,.95f) : stageId == 2 ? new Color(.045f,.065f,.19f) : water ? new Color(.045f,.23f,.48f) : new Color(.12f,.07f,.19f);
        SceneBackdrop(bg);
        MVAudio.Music(stageId == 1 ? "super-mario-bros" : stageId == 2 ? "mario-bros-1" : water ? "super-mario-bros" : "super-mario-bros-castillo");
        string floor = stageId == 2 ? "UndergroundBlock" : stageId == 4 ? "RocaOscura" : "GroundBlock";
        for (int x = -1; x < worldWidth + 2; x++)
        {
            if (!IsHole(x))
            {
                Solid(floor, x, -3f);
                Visual("Relleno inferior", floor, x, -4, 1, new Color(.73f,.73f,.78f), -5);
            }
            else if (stageId == 4)
            {
                GameObject lava = Visual("Lava mortal", "Lava", x, -3.15f, 1f, null, 2);
                BoxCollider2D c = lava.AddComponent<BoxCollider2D>(); c.isTrigger = true; lava.AddComponent<MVLava>();
            }
        }
        if (stageId == 1) BuildOverworld();
        else if (stageId == 2 || stageId == 3)
        {
            for (int section = 0; section < 3; section++)
            {
                constructionOffset = section * (water ? 69 : 65);
                if (water) BuildUnderwater(); else BuildUnderground();
            }
            constructionOffset = 0;
        }
        else BuildCastle();
        hero = CreateHero(water);
        if (stageId == 4)
        {
            var helper = Visual("Tiko - compañero dinosaurio original", "Tiko", .7f, -1.5f, 1.1f, null, 6);
            helper.AddComponent<MVCompanion>().hero = hero;
            ShowHint("TIKO: ¡Activemos los tres sellos estelares!");
        }
        Exit(worldWidth - 3f, stageId == 4 ? "Portal" : "Flag", -1.6f);
    }

    private MVHero CreateHero(bool water)
    {
        var player = Visual("Mario - jugador", "Mario_Small_Idle", 2f, -1.45f, 1f, null, 12);
        player.tag = "Player";
        BoxCollider2D c = player.AddComponent<BoxCollider2D>(); c.size = new Vector2(.72f,.88f);
        Rigidbody2D rb = player.AddComponent<Rigidbody2D>(); rb.gravityScale = water ? .58f : 2.35f;
        rb.freezeRotation = true; rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        var p = player.AddComponent<MVHero>(); p.game = this; p.inWater = water;
        return p;
    }
    private void Exit(float x, string sprite, float y)
    {
        GameObject end = Visual("SALIDA -> siguiente nivel", sprite, x, y, sprite == "Portal" ? 1.8f : 1.25f, null, 5);
        BoxCollider2D collider = end.AddComponent<BoxCollider2D>(); collider.isTrigger = true; collider.size = sprite == "Portal" ? new Vector2(1.3f,2.5f) : new Vector2(1.3f,2.9f);
        end.AddComponent<MVExit>().game = this;
    }
    private void Pick(string sprite, float x, float y, int kind = 0)
    {
        var obj = Visual(kind == 2 ? "Sello de luz" : "Recompensa", sprite, x, y, kind == 2 ? 1.1f : .85f, null, 6);
        if (kind == 2)
        {
            obj.AddComponent<BoxCollider2D>().isTrigger = true;
            obj.AddComponent<FinalCastleSeal>().castleManager = castle;
        }
        else
        {
            var c = obj.AddComponent<CircleCollider2D>(); c.radius = .5f; c.isTrigger = true;
            var pick = obj.AddComponent<MVPickup>(); pick.game = this; pick.kind = kind;
        }
    }
    private void Coins(int start, int count, float y)
    {
        for (int x = start; x < start + count; x += 2) Pick(stageId == 2 ? "Coin_Underground" : "Coin", x, y);
    }
    private void Enemy(string sprite, float x, float y, bool aquatic = false, float span = 1.9f)
    {
        var o = Visual("Enemigo " + sprite, sprite, x, y, aquatic ? 1.05f : 1.1f, null, 7);
        var c = o.AddComponent<BoxCollider2D>(); c.size = new Vector2(.8f,.75f); c.isTrigger = true;
        var enemy = o.AddComponent<MVEnemy>(); enemy.game = this; enemy.aquatic = aquatic; enemy.originX = x + constructionOffset; enemy.distance = span; enemy.velocity = .8f + (x % 3) * .2f;
    }
    private void Pipe(float x, int height, bool purple = false)
    {
        // Geometría de tubería verde construida con los sprites que entregó el usuario.
        for (int y = 0; y < height; y++)
            for (int column = 0; column < 2; column++)
            {
                var go = Visual("Tuberia", y == height-1 ? "PipeTop" : "PipeBottom", x + column, -2f + y, 1, purple ? new Color(.84f,.7f,1) : Color.white, 2);
                go.AddComponent<BoxCollider2D>().size = Vector2.one;
            }
    }
    private void BackdropDecor(int start, int end, int step, string sprite, float y, float scale = 1f, Color? color = null)
    {
        for (int x = start; x < end; x += step) Visual("Decoracion " + sprite, sprite, x, y, scale, color, -7);
    }
    private void BuildOverworld()
    {
        BackdropDecor(4, 62, 12, "Cloud2", 2.8f, 2f);
        BackdropDecor(9, 62, 14, "Hill1", -1.95f, 1.9f);
        BackdropDecor(1, 60, 10, "Bush2", -2.0f, 1.4f);
        Platform(8,12,.4f,"Brick"); Platform(24,29,.4f,"MysteryBlock"); Platform(44,48,.7f,"Brick");
        Solid("GroundBlock",22,-2); Solid("GroundBlock",23,-1);
        Pipe(14,2); Pipe(34,3);
        Coins(6,7,1.7f); Coins(23,8,1.7f); Coins(42,9,1.6f);
        Enemy("Goomba_Walk1",8,-1.75f); Enemy("Koopa_Walk1",28,-1.7f); Enemy("Goomba_Walk2",48,-1.7f);
        Pick("1upMushroom",32,1.35f,1);
    }
    private void BuildUnderground()
    {
        BackdropDecor(2,64,3,"UndergroundBrick",4.5f,1f,new Color(.65f,.67f,.86f));
        BackdropDecor(4,64,9,"UndergroundBlock",2.9f,2f,new Color(.47f,.5f,.68f));
        Platform(6,13,0,"UndergroundBrick"); Platform(23,29,0,"UndergroundBrick"); Platform(37,43,.5f,"UndergroundBrick");
        Platform(48,54,0,"UndergroundBrick");
        Pipe(14,2,true); Pipe(32,2,true); Pipe(55,3,true);
        for (int i=0; i<4; i++) Solid("UndergroundBlock",46+i,-2+i);
        Coins(7,6,1.25f); Coins(24,7,1.5f); Coins(45,12,2.4f);
        Enemy("Goomba_Walk1",11,-1.7f); Enemy("Koopa_Walk1",28,-1.7f); Enemy("Goomba_Walk2",49,-1.7f);
        Pick("1upMushroom",53,1.8f,1);
    }
    private void BuildUnderwater()
    {
        BackdropDecor(2,69,6,"Coral",-1.8f,1.2f);
        BackdropDecor(5,69,10,"PipeBottom",-1.5f,2f,new Color(.35f,.75f,.84f));
        BackdropDecor(7,69,12,"Cloud1",2.8f,1.25f,new Color(.55f,.9f,1f,.55f));
        Platform(15,20,-.9f,"HardBlock"); Platform(30,35,.25f,"HardBlock"); Platform(49,54,-.5f,"HardBlock");
        Pipe(12,2); Pipe(42,3);
        Coins(5,10,1.5f); Coins(23,10,2.5f); Coins(43,12,1.85f); Coins(58,6,1.3f);
        Enemy("Pez",14,1.1f,true,2.5f); Enemy("Medusa",26,-.1f,true,1.6f);
        Enemy("Pez",38,1.8f,true,2.8f); Enemy("Medusa",53,.4f,true,1.6f);
        Enemy("Pez",61,1.5f,true,1.4f);
        Pick("1upMushroom",50,3f,1);
    }
    private void BuildCastle()
    {
        castle = new GameObject("Final original - tres sellos").AddComponent<FinalCastleManager>();
        castle.campaignMode = true;
        castle.SealActivated += count => Seal();
        BackdropDecor(2,79,4,"RocaOscura",4.25f,1.7f,new Color(.6f,.55f,.85f));
        BackdropDecor(4,79,8,"Lava",-4.2f,2f,new Color(.8f,.5f,.45f));
        Platform(8,14,0,"RocaOscura"); Platform(20,25,.5f,"RocaOscura");
        Platform(29,31,1.35f,"RocaOscura"); Platform(36,42,.2f,"RocaOscura");
        Platform(52,58,.7f,"RocaOscura");
        for (int n=0;n<5;n++) { Solid("RocaOscura",66+n,-2 + n); }
        Pick("SelloEstelar",11,1.7f,2);
        for (int section = 1; section < 3; section++)
        {
            constructionOffset = section * 56;
            BackdropDecor(2,56,4,"RocaOscura",4.25f,1.7f,new Color(.6f,.55f,.85f));
            Platform(8,14,0,"RocaOscura"); Platform(20,25,.5f,"RocaOscura");
            Platform(29,31,1.35f,"RocaOscura"); Platform(36,42,.2f,"RocaOscura");
            Pick("SelloEstelar",24,2.1f,2);
            Coins(6,7,2.4f); Coins(26,9,2.9f);
            Enemy("Goomba_Walk1",22,-1.7f); Enemy("Koopa_Walk1",41,-1.7f);
        }
        constructionOffset = 0;
        Coins(6,7,2.4f); Coins(26,9,2.9f); Coins(48,7,2.8f);
        Enemy("Goomba_Walk1",22,-1.7f); Enemy("Koopa_Walk1",41,-1.7f);
        Pick("1upMushroom",52,2.5f,1);
        var bossObj = Visual("GUARDIAN DE LAS CENIZAS - jefe final", "BowserSombra",174f,-1.30f,2.3f,null,10);
        BoxCollider2D bossCol = bossObj.AddComponent<BoxCollider2D>(); bossCol.isTrigger = true; bossCol.size = new Vector2(.8f,.75f);
        MVBoss boss = bossObj.AddComponent<MVBoss>(); boss.game = this;
    }
    public void SpawnFire(Vector3 at)
    {
        var fire = Visual("Bola de fuego", "Fuego", at.x, at.y,1.15f,null,9);
        var c = fire.AddComponent<CircleCollider2D>(); c.isTrigger = true; c.radius = .43f;
        fire.AddComponent<MVProjectile>().game = this;
    }
    private void Bubble(Vector3 at)
    {
        GameObject b = Visual("Burbuja", "Coin", at.x, at.y, .18f, new Color(.5f,.93f,1f,.24f), -2);
        Destroy(b, .65f);
    }
    private void MenuDecor()
    {
        BackdropDecor(1,25,4,"Cloud2",2.35f,2f);
        BackdropDecor(1,25,5,"Hill1",-2.8f,2f);
        Visual("Mario en portada", "Mario_Big_Idle", 5,-1.1f,2.1f,null,5);
        Visual("Tiko - compañero", "Tiko", 8,-1.1f,1.8f,null,5);
        Visual("Estrella central", "Estrella", 12,1.35f,2.5f,null,5);
    }
    private void VictoryDecor()
    {
        BackdropDecor(1,27,4,"Cloud2",2.5f,1.75f);
        BackdropDecor(2,27,5,"Hill1",-2.65f,2f);
        for (int i=0;i<15;i++)
            Visual("Estrellas liberadas", "Estrella", 1+(i*3)%23, -1.5f + (i*7)%6, .55f + (i%3)*.2f, null, 6);
        Visual("Mario vencedor", "Mario_Big_Idle",5,-1.5f,2.5f,null,9);
        Visual("Tiko vencedor", "Tiko",8,-1.3f,2.3f,null,9);
        Visual("Estrella rescatada", "Estrella",12, .15f, 4.3f,null,9);
    }

    // ============================= INTERFAZ =============================
    private GUIStyle White(int size, bool bold = false, TextAnchor align = TextAnchor.MiddleCenter)
    {
        var style = new GUIStyle(GUI.skin.label);
        style.fontSize = size; style.fontStyle = bold ? FontStyle.Bold : FontStyle.Normal;
        style.alignment = align; style.normal.textColor = Color.white;
        style.wordWrap = true;
        return style;
    }
    private GUIStyle ButtonStyle()
    {
        var style = new GUIStyle(GUI.skin.button);
        style.fontSize = 24; style.fontStyle = FontStyle.Bold;
        style.normal.textColor = new Color(.13f,.17f,.25f);
        style.padding = new RectOffset(12,12,10,10);
        return style;
    }
    private void Print(Rect box, string message, int size, bool bold=false) { GUI.Label(box,message,White(size,bold)); }
    private bool Click(Rect rect,string label) { return GUI.Button(rect,label,ButtonStyle()); }
    private void Panel(Rect rect) { GUI.Box(rect,""); }

    private void OnGUI()
    {
        if (stageId == 0 || stageId == 6) return; // Pantallas con Canvas y navegación de teclado.
        float sx = Screen.width / 1280f, sy = Screen.height / 720f;
        float factor = Mathf.Max(.01f, Mathf.Min(sx,sy));
        Vector3 offset = new Vector3((Screen.width-1280*factor)/2f,(Screen.height-720*factor)/2f,0);
        GUI.matrix = Matrix4x4.TRS(offset,Quaternion.identity,new Vector3(factor,factor,1));
        if (stageId == 0) DrawMenu();
        else if (stageId == 5) DrawGameOver();
        else if (stageId == 6) DrawVictory();
        else DrawHUD();
    }
    private void DrawMenu()
    {
        Print(new Rect(120,48,1040,80),"SUPER MARIO VÍCTOR",54,true);
        Print(new Rect(180,126,920,60),"LA ÚLTIMA ESTRELLA",32,true);
        Print(new Rect(180,184,920,65),"4 mundos · 3 vidas · 1 aventura · 1 final original",21);
        Panel(new Rect(425,295,430,285));
        if (Click(new Rect(490,310,300,65),"JUGAR")) StartNewGame();
        if (Click(new Rect(490,392,300,60),showHelp ? "OCULTAR CONTROLES" : "CONTROLES")) showHelp = !showHelp;
        if (Click(new Rect(490,472,300,60),"SALIR")) Quit();
        if (showHelp) Print(new Rect(350,578,580,80),"A / D o ← / → mover     ESPACIO / W / ↑ saltar o nadar     ESC pausar",19,true);
        Print(new Rect(120,670,1040,30),"Proyecto académico Unity 6 · Acompañante original: Tiko",18);
    }
    private void DrawHUD()
    {
        GUI.Box(new Rect(14,12,1252,71),"");
        Print(new Rect(35,16,240,60),stageName[stageId],23,true);
        Print(new Rect(290,16,175,60),"VIDAS: " + lives,22,true);
        Print(new Rect(470,16,180,60),"MONEDAS: " + coins,20,true);
        Print(new Rect(665,16,175,60),"PUNTOS: " + score,19,true);
        Print(new Rect(865,16,170,60),"TIEMPO: " + Mathf.CeilToInt(Mathf.Max(0,timeLeft)),19,true);
        Print(new Rect(1045,16,180,60),stageId == 4 ? "SELLOS: " + seals + "/3" : "ESC: PAUSA",18,true);
        if (stageId == 3) Print(new Rect(310,610,650,70),"AGUA: mantén ESPACIO / W / ↑ para nadar",19,true);
        if (hintUntil > Time.time) { GUI.Box(new Rect(248,95,780,65),""); Print(new Rect(265,100,750,55),hint,20,true); }
        if (paused)
        {
            Panel(new Rect(385,165,510,395));
            Print(new Rect(405,190,470,68),"PAUSA",39,true);
            if (Click(new Rect(445,275,390,65),"CONTINUAR")) { paused = false; Time.timeScale = 1f; }
            if (Click(new Rect(445,365,390,65),"REINICIAR PARTIDA")) StartNewGame();
            if (Click(new Rect(445,457,390,65),"VOLVER AL MENÚ")) ReturnToMenu();
        }
    }
    private void DrawGameOver()
    {
        Print(new Rect(150,100,980,100),"GAME OVER",72,true);
        Print(new Rect(210,220,860,85),"Mario perdió todas sus vidas. ¡La estrella todavía puede salvarse!",23);
        Print(new Rect(230,325,820,55),"PUNTUACIÓN: " + score + "     MONEDAS: " + coins,23,true);
        if (Click(new Rect(450,424,380,70),"INTENTAR OTRA VEZ")) StartNewGame();
        if (Click(new Rect(450,510,380,70),"MENÚ PRINCIPAL")) ReturnToMenu();
    }
    private void DrawVictory()
    {
        Print(new Rect(125,45,1030,90),"¡LA ÚLTIMA ESTRELLA BRILLA!",44,true);
        Print(new Rect(155,138,970,88),"Mario y Tiko han liberado la luz del reino.",26,true);
        Print(new Rect(155,228,970,50),"El castillo deja atrás las cenizas y comienza un nuevo amanecer.",21);
        Print(new Rect(200,425,880,70),"VIDAS: " + lives + "     MONEDAS: " + coins + "     PUNTOS: " + score,25,true);
        if (Click(new Rect(270,542,330,68),"JUGAR DE NUEVO")) StartNewGame();
        if (Click(new Rect(675,542,330,68),"IR AL MENÚ")) ReturnToMenu();
        Print(new Rect(160,644,960,40),"FINAL ORIGINAL · Super Mario Víctor — La última estrella",19,true);
    }
}
