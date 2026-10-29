using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>Portada 2D: sprites a escala controlada, texto separado del arte y navegación nativa.</summary>
public sealed class MVMenuScreen : MonoBehaviour
{
    public MVGameController game;
    private Font font;
    private RectTransform content;
    private GameObject help;
    private CanvasGroup main;
    private Button play, close;
    private Text musicLabel, motionLabel;
    private RectTransform star;
    private bool reducedMotion;
    private static readonly Color Cream = new Color32(255, 244, 213, 255);
    private static readonly Color Muted = new Color32(183, 206, 225, 255);
    private static readonly Color Gold = new Color32(255, 211, 91, 255);
    private static readonly Color Navy = new Color32(12, 29, 52, 255);

    private void Start()
    {
        font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        reducedMotion = PlayerPrefs.GetInt("MV.ReducedMotion", 0) == 1;
        var canvasObject = new GameObject("Menu 2D", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        var canvas = canvasObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;
        var scaler = canvasObject.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1280, 720);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
        var background = Image(canvasObject.transform, "Fondo", Navy);
        background.rectTransform.anchorMin = Vector2.zero;
        background.rectTransform.anchorMax = Vector2.one;
        background.rectTransform.sizeDelta = Vector2.zero;
        content = new GameObject("Contenido seguro 1280x720", typeof(RectTransform), typeof(CanvasGroup)).GetComponent<RectTransform>();
        content.SetParent(canvasObject.transform, false);
        content.sizeDelta = new Vector2(1280, 720);
        main = content.GetComponent<CanvasGroup>();

        Label(content, "SUPER MARIO VÍCTOR", 17, Muted, 72, 36, 500, 28, true);
        Label(content, "UNA AVENTURA EN 2D", 14, Gold, 904, 36, 304, 28, true, TextAnchor.MiddleRight);
        Box(content, "Separador superior", new Color32(47, 70, 96, 255), 72, 78, 1136, 2);
        Label(content, "LA ÚLTIMA", 68, Cream, 72, 111, 665, 85, true);
        var title = Label(content, "ESTRELLA", 80, Gold, 69, 184, 650, 101, true);
        var shadow = title.gameObject.AddComponent<Shadow>();
        shadow.effectColor = new Color32(137, 64, 36, 255);
        shadow.effectDistance = new Vector2(3, -4);
        Label(content, "Cuatro mundos. Un reino por liberar.", 24, Cream, 76, 294, 590, 40);
        Label(content, "Acompaña a Mario y Tiko, reúne los tres sellos\ny desafía al guardián del último castillo.", 19, Muted, 76, 337, 590, 62);
        play = Button(content, "JUGAR AHORA     >", 76, 426, 366, 64, new Color32(239, 179, 61, 255), Navy, () => game.StartNewGame());
        Button(content, "CONTROLES", 76, 505, 176, 48, new Color32(39, 64, 91, 255), Cream, () => ShowHelp(true));
        var music = Button(content, "", 266, 505, 176, 48, new Color32(39, 64, 91, 255), Cream, ToggleMusic);
        musicLabel = music.GetComponentInChildren<Text>();
        UpdateMusicLabel();
        Button(content, "SALIR", 462, 505, 130, 48, new Color32(39, 64, 91, 255), Cream, () => game.Quit());
        BuildIllustration();

        WorldCard(76, "01", "TIERRA", "GroundBlock", new Color32(124, 217, 139, 255));
        WorldCard(362, "02", "SUBTERRÁNEO", "UndergroundBrick", new Color32(166, 173, 255, 255));
        WorldCard(648, "03", "OCÉANO", "Pez", new Color32(102, 215, 237, 255));
        WorldCard(934, "04", "CASTILLO", "SelloEstelar", new Color32(255, 163, 122, 255));
        Label(content, "Autor: Víctor · VictorDev", 14, Cream, 76, 679, 470, 22, true);
        Label(content, "Base tutorial: Zigurous  ·  Adaptación académica", 12, Muted, 620, 679, 584, 22, false, TextAnchor.MiddleRight);
        BuildHelp(canvasObject.transform);
        if (FindFirstObjectByType<EventSystem>() == null)
            new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
        EventSystem.current.SetSelectedGameObject(play.gameObject);
    }

    private void BuildIllustration()
    {
        Box(content, "Viñeta del reino", new Color32(23, 49, 80, 255), 730, 112, 478, 442);
        Box(content, "Línea estelar", Gold, 730, 112, 478, 4);
        for (int i = 0; i < 19; i++)
            Box(content, "Punto de luz", new Color32(126, 162, 191, 255), 754 + (i * 67) % 420, 138 + (i * 43) % 232, i % 3 == 0 ? 4 : 2, i % 3 == 0 ? 4 : 2);
        Pixel(content, "Cloud1", 755, 165, 112, 46, new Color(1, 1, 1, .65f));
        Pixel(content, "Cloud2", 1053, 240, 127, 52, new Color(1, 1, 1, .5f));
        star = Pixel(content, "Estrella", 946, 155, 89, 89).rectTransform;
        Pixel(content, "Hill1", 752, 370, 174, 108, new Color32(91, 157, 136, 255));
        Pixel(content, "Hill2", 995, 347, 205, 132, new Color32(80, 132, 140, 255));
        for (int i = 0; i < 11; i++) Pixel(content, "GroundBlock", 730 + i * 44, 478, 44, 44);
        Pixel(content, "Mario_Big_Idle", 845, 318, 76, 160);
        Pixel(content, "Tiko", 969, 389, 90, 89);
        Pixel(content, "MysteryBlock", 1080, 326, 38, 38);
        Label(content, "MARIO + TIKO", 16, Cream, 748, 526, 440, 25, true, TextAnchor.MiddleCenter);
    }
    private void WorldCard(float x, string number, string title, string sprite, Color accent)
    {
        Box(content, "Mundo " + number, new Color32(24, 44, 68, 255), x, 594, 270, 68);
        Box(content, "Acento", accent, x, 594, 3, 68);
        Pixel(content, sprite, x + 17, 614, 29, 29);
        Label(content, number, 12, Muted, x + 60, 601, 190, 20, true);
        Label(content, title, 16, Cream, x + 60, 622, 200, 30, true);
    }
    private void BuildHelp(Transform canvas)
    {
        var overlay = Image(canvas, "Controles y preferencias", new Color32(5, 14, 29, 248));
        help = overlay.gameObject;
        overlay.raycastTarget = true;
        overlay.rectTransform.anchorMin = Vector2.zero;
        overlay.rectTransform.anchorMax = Vector2.one;
        overlay.rectTransform.sizeDelta = Vector2.zero;
        var panel = new GameObject("Panel controles", typeof(RectTransform)).GetComponent<RectTransform>();
        panel.SetParent(help.transform, false);
        panel.sizeDelta = new Vector2(1280, 720);
        Label(panel, "PREPARA TU AVENTURA", 38, Gold, 220, 102, 840, 66, true);
        Label(panel, "MOVERSE          Flechas izquierda / derecha o A / D\nSALTAR               Espacio, W o flecha arriba\nNADAR                Mantén la tecla de salto bajo el agua\nPAUSAR               Escape durante la partida\nMENÚ                   Flechas para elegir · Enter para confirmar", 22, Cream, 220, 202, 840, 235);
        var motion = Button(panel, "", 220, 469, 410, 55, new Color32(39, 64, 91, 255), Cream, () => {
            reducedMotion = !reducedMotion;
            PlayerPrefs.SetInt("MV.ReducedMotion", reducedMotion ? 1 : 0);
            UpdateMotionLabel();
        });
        motionLabel = motion.GetComponentInChildren<Text>();
        UpdateMotionLabel();
        close = Button(panel, "VOLVER AL MENÚ", 650, 469, 410, 55, Gold, Navy, () => ShowHelp(false));
        help.SetActive(false);
    }
    public void ShowHelp(bool show)
    {
        help.SetActive(show);
        main.interactable = !show;
        main.blocksRaycasts = !show;
        EventSystem.current.SetSelectedGameObject(show ? close.gameObject : play.gameObject);
    }
    private void ToggleMusic() { MVAudio.SetMusicEnabled(!MVAudio.MusicEnabled); UpdateMusicLabel(); }
    private void UpdateMusicLabel() { musicLabel.text = "MÚSICA: " + (MVAudio.MusicEnabled ? "SÍ" : "NO"); }
    private void UpdateMotionLabel() { motionLabel.text = reducedMotion ? "ANIMACIÓN: REDUCIDA" : "ANIMACIÓN: ACTIVADA"; }
    private void Update()
    {
        if (star != null) star.anchoredPosition = new Vector2(946 + 44.5f, -199.5f + (reducedMotion ? 0 : Mathf.Sin(Time.unscaledTime * 1.5f) * 6));
        if (help != null && help.activeSelf && MVKeys.Pressed(KeyCode.Escape)) ShowHelp(false);
    }
    private Image Image(Transform parent, string name, Color color)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        go.transform.SetParent(parent, false);
        var image = go.GetComponent<Image>(); image.color = color; image.raycastTarget = false;
        return image;
    }
    private void Position(RectTransform rect, float x, float y, float width, float height)
    {
        rect.anchorMin = rect.anchorMax = new Vector2(0, 1);
        rect.pivot = new Vector2(.5f, .5f);
        rect.sizeDelta = new Vector2(width, height);
        rect.anchoredPosition = new Vector2(x + width / 2, -y - height / 2);
    }
    private Image Box(Transform parent, string name, Color color, float x, float y, float width, float height)
    {
        var image = Image(parent, name, color); Position(image.rectTransform, x, y, width, height); return image;
    }
    private Image Pixel(Transform parent, string sprite, float x, float y, float width, float height, Color? tint = null)
    {
        var image = Box(parent, sprite, tint ?? Color.white, x, y, width, height);
        image.sprite = MVGameController.Sprite(sprite); image.preserveAspect = true; return image;
    }
    private Text Label(Transform parent, string value, int size, Color color, float x, float y, float width, float height, bool bold = false, TextAnchor align = TextAnchor.MiddleLeft)
    {
        var go = new GameObject(value, typeof(RectTransform), typeof(CanvasRenderer), typeof(Text)); go.transform.SetParent(parent, false);
        var text = go.GetComponent<Text>(); text.font = font; text.text = value; text.fontSize = size; text.color = color;
        text.fontStyle = bold ? FontStyle.Bold : FontStyle.Normal; text.alignment = align; text.raycastTarget = false;
        text.horizontalOverflow = HorizontalWrapMode.Wrap; text.verticalOverflow = VerticalWrapMode.Truncate;
        Position(text.rectTransform, x, y, width, height); return text;
    }
    private Button Button(Transform parent, string caption, float x, float y, float width, float height, Color color, Color ink, UnityEngine.Events.UnityAction action)
    {
        var image = Box(parent, "Botón " + caption, color, x, y, width, height); image.raycastTarget = true;
        var button = image.gameObject.AddComponent<Button>(); button.targetGraphic = image;
        var colors = button.colors; colors.highlightedColor = new Color(1.15f, 1.15f, 1.15f);
        colors.selectedColor = new Color(1.25f, 1.25f, 1.25f); colors.pressedColor = new Color(.78f, .78f, .78f); colors.fadeDuration = .12f; button.colors = colors;
        var outline = image.gameObject.AddComponent<Outline>(); outline.effectColor = new Color32(159, 199, 222, 100); outline.effectDistance = new Vector2(1, -1);
        image.gameObject.AddComponent<MVMenuFocus>();
        button.onClick.AddListener(action);
        Label(image.transform, caption, height > 55 ? 23 : 16, ink, 8, 0, width - 16, height, true, TextAnchor.MiddleCenter);
        return button;
    }
}
