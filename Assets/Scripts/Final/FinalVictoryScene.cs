using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

/// <summary>
/// "El Reino Liberado": cierre original de la aventura.
/// Construye en tiempo de ejecución una interfaz escalable, sin prefabs UI externos.
/// </summary>
public sealed class FinalVictoryScene : MonoBehaviour
{
    public bool campaignMode;
    private readonly List<Image> stars = new List<Image>();
    private AudioSource music;
    private Font font;
    private float elapsed;

    private void Start()
    {
        Time.timeScale = 1f;
        MVAudio.StopMusic();
        font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        BuildUI();
        StartCoroutine(PlayVictoryMusic());
    }

    private IEnumerator PlayVictoryMusic()
    {
        music = gameObject.AddComponent<AudioSource>();
        music.playOnAwake = false;
        music.loop = false;
        music.spatialBlend = 0f;
        music.volume = 0.45f;
        music.mute = !MVAudio.MusicEnabled;
        AudioClip chime = Resources.Load<AudioClip>("MarioVictor/Audio/mario-bros-vida");
        if (chime != null) music.PlayOneShot(chime, 0.85f);
        yield return new WaitForSecondsRealtime(chime == null ? 0.3f : Mathf.Max(0.5f, chime.length));
        AudioClip musicClip = Resources.Load<AudioClip>("MarioVictor/Audio/super-mario");
        if (musicClip != null)
        {
            music.clip = musicClip;
            music.loop = true;
            music.Play();
        }
    }

    private void Update()
    {
        elapsed += Time.unscaledDeltaTime;
        for (int i = 0; i < stars.Count; i++)
        {
            if (stars[i] == null) continue;
            Color c = stars[i].color;
            c.a = 0.3f + 0.65f * (0.5f + 0.5f * Mathf.Sin(elapsed * (1.3f + i % 5 * 0.27f) + i));
            stars[i].color = c;
        }
    }

    private void BuildUI()
    {
        GameObject canvasObject = new GameObject("Canvas - Final Original", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        Canvas canvas = canvasObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;
        CanvasScaler scale = canvasObject.GetComponent<CanvasScaler>();
        scale.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scale.referenceResolution = new Vector2(1280f, 720f);
        scale.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scale.matchWidthOrHeight = 0.5f;

        RectTransform root = canvasObject.GetComponent<RectTransform>();
        Image bg = MakeImage(root, "Fondo nocturno", new Color(0.055f, 0.075f, 0.155f, 1f));
        Stretch(bg.rectTransform);
        Image horizon = MakeImage(root, "Horizonte amanecer", new Color(0.25f, 0.14f, 0.32f, 0.95f));
        Place(horizon.rectTransform, new Vector2(0.5f, 0f), new Vector2(1280f, 210f), new Vector2(0, 102));
        Image ground = MakeImage(root, "Suelo del reino", new Color(0.14f, 0.3f, 0.27f, 1f));
        Place(ground.rectTransform, new Vector2(0.5f, 0f), new Vector2(1280f, 80f), new Vector2(0, 14));

        // Pequeñas luces animadas, reproducibles sin sprites externos.
        for (int i = 0; i < 36; i++)
        {
            Image light = MakeImage(root, "Luz " + (i + 1), new Color(1f, 0.92f, 0.62f, 0.8f));
            float x = -590f + (i * 163f % 1180f);
            float y = -65f + (i * 97f % 390f);
            float size = (i % 4 == 0) ? 8f : 4f;
            Place(light.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(size, size), new Vector2(x, y));
            stars.Add(light);
        }

        Text subtitle = MakeText(root, "Pre-título", "EL ÚLTIMO CASTILLO HA CAÍDO", 24, new Color(1f, 0.82f, 0.42f));
        Place(subtitle.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(970f, 65f), new Vector2(0, 208));
        Text title = MakeText(root, "Título", "¡EL REINO LIBERADO!", 60, Color.white);
        Place(title.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(1160f, 95f), new Vector2(0, 131));
        title.fontStyle = FontStyle.Bold;

        Text story = MakeText(root, "Historia", "Tres sellos rompieron el hechizo de Bowser.\nLa luz ha regresado al castillo y sus habitantes vuelven a sonreír.", 24, new Color(0.91f, 0.94f, 1f));
        Place(story.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(1050f, 120f), new Vector2(0, 10));
        Text credits = MakeText(root, "Créditos", "UN FINAL ORIGINAL · PROYECTO SUPER MARIO VÍCTOR", 18, new Color(0.85f, 0.86f, 1f));
        Place(credits.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(1000f, 42f), new Vector2(0, -95));

        CreateButton(root, "JUGAR DE NUEVO", new Vector2(-169f, -201f), RestartGame);
        CreateButton(root, "SALIR", new Vector2(169f, -201f), ExitGame);

        if (FindFirstObjectByType<EventSystem>() == null)
        {
            GameObject events = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
            // EventSystem pertenece solo a esta escena; no crear duplicados persistentes.
        }
    }

    private Image MakeImage(RectTransform parent, string name, Color color)
    {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        go.transform.SetParent(parent, false);
        Image image = go.GetComponent<Image>();
        image.color = color;
        image.raycastTarget = false;
        return image;
    }

    private Text MakeText(RectTransform parent, string name, string content, int fontSize, Color color)
    {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
        go.transform.SetParent(parent, false);
        Text t = go.GetComponent<Text>();
        t.font = font;
        t.text = content;
        t.fontSize = fontSize;
        t.alignment = TextAnchor.MiddleCenter;
        t.horizontalOverflow = HorizontalWrapMode.Wrap;
        t.verticalOverflow = VerticalWrapMode.Overflow;
        t.color = color;
        t.raycastTarget = false;
        return t;
    }

    private void CreateButton(RectTransform parent, string text, Vector2 position, UnityEngine.Events.UnityAction onClick)
    {
        GameObject go = new GameObject("Boton - " + text, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
        go.transform.SetParent(parent, false);
        RectTransform rect = go.GetComponent<RectTransform>();
        Place(rect, new Vector2(0.5f, 0.5f), new Vector2(284f, 73f), position);
        Image image = go.GetComponent<Image>();
        image.color = new Color(0.75f, 0.34f, 0.15f, 1f);
        Button button = go.GetComponent<Button>();
        button.targetGraphic = image;
        button.onClick.AddListener(onClick);
        Text caption = MakeText(rect, "Texto", text, 22, Color.white);
        caption.fontStyle = FontStyle.Bold;
        Stretch(caption.rectTransform);
    }

    private static void Place(RectTransform rt, Vector2 anchor, Vector2 size, Vector2 position)
    {
        rt.anchorMin = anchor;
        rt.anchorMax = anchor;
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = size;
        rt.anchoredPosition = position;
    }

    private static void Stretch(RectTransform rt)
    {
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        if (campaignMode)
        {
            FindFirstObjectByType<MVGameController>().StartNewGame();
            return;
        }
        if (GameManager.Instance != null) GameManager.Instance.NewGame();
        else SceneManager.LoadScene("1-1");
    }

    public void ExitGame()
    {
        Time.timeScale = 1f;
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
