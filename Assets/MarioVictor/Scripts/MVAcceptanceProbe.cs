#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Solo existe en el Editor; nunca se incluye en el ejecutable de entrega.
public sealed class MVAcceptanceProbe : MonoBehaviour
{
    [Serializable] private class Report
    {
        public string unity;
        public string utc;
        public bool passed;
        public List<string> checks = new List<string>();
        public List<string> errors = new List<string>();
    }
    private Report report = new Report();
    private MVGameController Game => FindFirstObjectByType<MVGameController>();
    private Transform Character => Game.hero != null ? Game.hero.transform : Game.legacyHero.transform;
    private void Awake() { DontDestroyOnLoad(gameObject); }
    private void OnEnable() { Application.logMessageReceived += CaptureError; }
    private void OnDisable() { Application.logMessageReceived -= CaptureError; }
    private void CaptureError(string message, string trace, LogType type)
    {
        if (type == LogType.Error || type == LogType.Exception || type == LogType.Assert)
            report.errors.Add(message);
    }
    private void Check(bool condition, string label)
    {
        if (!condition) throw new Exception("QA: " + label);
        report.checks.Add(label);
    }
    private void Place(Vector2 at, Vector2 velocity = default)
    {
        var body = Character.GetComponent<Rigidbody2D>();
        body.position = at;
        body.linearVelocity = velocity;
        Physics2D.SyncTransforms();
    }
    private IEnumerator Press(KeyCode key)
    {
        MVKeys.TestPressed = k => k == key;
        yield return null;
        MVKeys.TestPressed = null;
        yield return null;
    }
    private IEnumerator ExitStage(int expected)
    {
        if (Game.authoredLevel)
        {
            var flag = FindFirstObjectByType<FlagPole>();
            var at = flag.GetComponent<Collider2D>().bounds.center;
            Camera.main.transform.position = new Vector3(at.x, Camera.main.transform.position.y, -10);
            Place(at);
            yield return new WaitForSeconds(7f);
        }
        else
        {
            Place(FindFirstObjectByType<MVExit>().transform.position);
            yield return new WaitForSeconds(1.7f);
        }
        Check(Game != null && Game.stageId == expected, "Transición física a etapa " + expected);
    }
    private IEnumerator Run()
    {
        Check(Game != null && Game.stageId == 0, "Arranque en menú");
        ScreenCapture.CaptureScreenshot("Logs/MarioQA/menu-ui.png");
        yield return new WaitForSeconds(.2f);
        Game.StartNewGame();
        yield return new WaitForSeconds(.8f);
        Check(Game.stageId == 1 && MVGameController.Lives == 3, "Nueva partida con tres vidas");
        Check(Camera.main.orthographic && FindObjectsByType<Rigidbody>(FindObjectsSortMode.None).Length == 0 &&
              FindObjectsByType<Collider>(FindObjectsSortMode.None).Length == 0, "Cámara ortográfica y física exclusivamente 2D");
        Check(Game.authoredLevel && Game.legacyHero != null && FindObjectsByType<BlockHit>(FindObjectsSortMode.None).Length > 10 && FindObjectsByType<Pipe>(FindObjectsSortMode.None).Length > 0, "Mapa original completo con bloques y tuberías conservados");
        Check(Game.legacyHero.GetComponent<PlayerMovement>().grounded, "Mario permanece sobre el suelo original");
        float startX = Character.position.x;
        MVKeys.TestHeld = k => k == KeyCode.D;
        yield return new WaitForSeconds(.3f);
        MVKeys.TestHeld = null;
        Check(Character.position.x > startX + .1f, "Entrada D mueve a Mario original");
        yield return new WaitForSeconds(.4f);
        float groundY = Character.position.y;
        yield return Press(KeyCode.Space);
        yield return new WaitForSeconds(.15f);
        Check(Character.position.y > groundY + .6f, "Entrada Espacio produce salto original");
        yield return Press(KeyCode.Escape);
        Check(Game.paused && Time.timeScale == 0, "Escape pausa la partida");
        yield return Press(KeyCode.Escape);
        Check(!Game.paused && Time.timeScale == 1, "Escape reanuda la partida");
        var coin = new GameObject("Moneda QA del sistema original").AddComponent<BlockCoin>();
        yield return new WaitForSeconds(.1f);
        Check(MVGameController.CoinsCollected == 1, "Moneda original conectada al contador de campaña");
        for (int i = 0; i < 99; i++) GameManager.Instance.AddCoin();
        Check(MVGameController.CoinsCollected == 0 && MVGameController.Lives == 4, "Cien monedas conceden una vida");
        GameManager.Instance.AddLife();
        Check(MVGameController.Lives == 5, "1-UP original conectado a la campaña");
        yield return ExitStage(2);
        yield return ExitStage(3);
        Check(Game.hero.inWater && Game.hero.body.gravityScale < 1, "Nivel acuático con gravedad de natación");
        Place(new Vector2(3, 0));
        MVKeys.TestHeld = k => k == KeyCode.Space;
        yield return new WaitForSeconds(.4f);
        MVKeys.TestHeld = null;
        Check(Game.hero.transform.position.y > 1, "Mantener Espacio permite nadar");
        yield return ExitStage(4);
        Game.hero.Invincibility(120);
        Place(FindFirstObjectByType<MVExit>().transform.position);
        yield return new WaitForSeconds(.2f);
        Check(Game.stageId == 4 && Game.seals == 0, "Portal bloqueado antes de resolver el castillo");
        var boss = FindFirstObjectByType<MVBoss>();
        Place((Vector2)boss.transform.position + Vector2.up * 1.8f, Vector2.down * 2);
        yield return new WaitForSeconds(.35f);
        Check(boss.health == 3, "Escudo del jefe impide daño sin sellos");
        var seals = FindObjectsByType<FinalCastleSeal>(FindObjectsSortMode.None);
        Check(seals.Length == 3, "Castillo contiene los tres sellos originales");
        for (int i = 0; i < seals.Length; i++)
        {
            Place(seals[i].transform.position);
            yield return new WaitForSeconds(.12f);
            Check(Game.seals == i + 1, "Sello físico " + (i + 1) + " registrado sin duplicados");
        }
            Place(new Vector2(166, 3));
        yield return new WaitForSeconds(1.5f);
        for (int hit = 1; hit <= 3; hit++)
        {
            Place((Vector2)boss.transform.position + Vector2.up * 1.8f, Vector2.down * 2);
            yield return new WaitForSeconds(.35f);
            Check(boss.health == 3 - hit, "Salto físico sobre jefe: impacto " + hit);
            if (hit < 3)
            {
                Place(new Vector2(166, 3));
                yield return new WaitForSeconds(1.4f);
            }
        }
        yield return new WaitForSeconds(2.5f);
        Check(Game.castle.defeatComplete, "Animación original de derrota completada");
        yield return ExitStage(6);
        Check(FindFirstObjectByType<FinalVictoryScene>()?.campaignMode == true, "Final original integrado en victoria de campaña");
        var restart = FindObjectsByType<Button>(FindObjectsSortMode.None).First(b => b.GetComponentInChildren<Text>().text == "JUGAR DE NUEVO");
        restart.onClick.Invoke();
        yield return new WaitForSeconds(.3f);
        Check(Game.stageId == 1 && MVGameController.Lives == 3 && MVGameController.CoinsCollected == 0, "Botón de victoria reinicia la campaña");
        for (int death = 1; death <= 3; death++)
        {
            Game.legacyHero.Death();
            Game.legacyHero.Death();
            yield return new WaitForSeconds(5.2f);
            Check(MVGameController.Lives == 3 - death, "Muerte " + death + " descuenta exactamente una vida");
        }
        Check(Game.stageId == 5, "Tres muertes muestran Game Over");
        Game.StartNewGame();
        yield return new WaitForSeconds(.3f);
        Check(Game.stageId == 1 && MVGameController.Lives == 3, "Reintento desde Game Over");
        Game.ReturnToMenu();
        yield return new WaitForSeconds(.3f);
        Check(FindObjectsByType<AudioListener>(FindObjectsSortMode.None).Length == 1 &&
              FindObjectsByType<MVAudio>(FindObjectsSortMode.None).Length == 1, "Sin duplicados de cámara de audio ni mezclador");
    }
    private IEnumerator Start()
    {
        report.unity = Application.unityVersion;
        report.utc = DateTime.UtcNow.ToString("o");
        var stack = new Stack<IEnumerator>();
        stack.Push(Run());
        while (stack.Count > 0)
        {
            object current = null;
            bool moved = false;
            try { moved = stack.Peek().MoveNext(); if (moved) current = stack.Peek().Current; }
            catch (Exception error) { report.errors.Add(error.ToString()); break; }
            if (!moved) { stack.Pop(); continue; }
            if (current is IEnumerator nested) stack.Push(nested);
            else yield return current;
        }
        MVKeys.TestHeld = null;
        MVKeys.TestPressed = null;
        Time.timeScale = 1;
        report.passed = report.errors.Count == 0;
        Directory.CreateDirectory("Logs/MarioQA");
        File.WriteAllText("Logs/MarioQA/acceptance.json", JsonUtility.ToJson(report, true));
        Debug.Log("Mario QA: " + (report.passed ? "PASS" : "FAIL") + " — " + report.checks.Count + " comprobaciones");
        Destroy(gameObject);
    }
}
#endif
