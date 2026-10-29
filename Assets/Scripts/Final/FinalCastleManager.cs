using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Final original: al activar tres sellos, Bowser pierde su escudo y se abre la escena de victoria.
/// Este controlador no reemplaza al GameManager del Mario original.
/// </summary>
[DisallowMultipleComponent]
public sealed class FinalCastleManager : MonoBehaviour
{
    public bool campaignMode;
    public bool defeatComplete { get; private set; }
    public event System.Action<int> SealActivated;
    [Header("Final original")]
    [Min(1)] public int requiredSeals = 3;
    public string victoryScene = "VictoriaFinal";
    public Transform bowserVisual;
    public float defeatAnimationSeconds = 2.25f;
    public float beforeVictorySeconds = 1.1f;

    [Header("Sonido (opcional)")]
    public AudioSource audioSource;
    public string castleAudioResource = "MarioVictor/Audio/super-mario-bros-castillo";
    public string sealAudioResource = "MarioVictor/Audio/mario-moneda";
    public string finalAudioResource = "MarioVictor/Audio/mario-bros-vida";

    private int activatedSeals;
    private bool ending;
    private string statusText = "Activa los tres sellos para romper el escudo de Bowser.";

    private void Start()
    {
        if (campaignMode) return; // La campaña utiliza su único mezclador MVAudio.
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f;
        var clip = Resources.Load<AudioClip>(castleAudioResource);
        if (clip != null)
        {
            audioSource.clip = clip;
            audioSource.loop = true;
            audioSource.volume = 0.35f;
            audioSource.Play();
        }
    }

    public void ActivateSeal(FinalCastleSeal seal)
    {
        if (ending || seal == null || !seal.TryActivate()) return;

        activatedSeals++;
        SealActivated?.Invoke(activatedSeals);
        if (!campaignMode) PlayOneShot(sealAudioResource);
        statusText = string.Format("Sellos activados: {0}/{1}", activatedSeals, requiredSeals);

        if (activatedSeals >= requiredSeals && !campaignMode)
        {
            ending = true;
            StartCoroutine(DefeatBowserAndFinish());
        }
    }

    public void DefeatCampaignBoss(Transform boss)
    {
        if (!campaignMode || ending || activatedSeals < requiredSeals) return;
        ending = true;
        bowserVisual = boss;
        StartCoroutine(DefeatBowserAndFinish());
    }

    private IEnumerator DefeatBowserAndFinish()
    {
        statusText = "¡El escudo ha desaparecido! ¡El reino está a salvo!";
        if (audioSource != null) audioSource.Stop();
        PlayOneShot(finalAudioResource);

        if (bowserVisual != null)
        {
            var sprite = bowserVisual.GetComponent<SpriteRenderer>();
            var initialPos = bowserVisual.position;
            var originalColor = sprite != null ? sprite.color : Color.white;
            float elapsed = 0f;
            float duration = Mathf.Max(0.1f, defeatAnimationSeconds);
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                bowserVisual.position = initialPos + Vector3.down * (3f * t);
                bowserVisual.Rotate(0f, 0f, 280f * Time.deltaTime);
                if (sprite != null)
                    sprite.color = new Color(originalColor.r, originalColor.g, originalColor.b, 1f - t);
                yield return null;
            }
        }
        defeatComplete = true;
        if (campaignMode) yield break; // El jugador aún debe alcanzar el portal.
        yield return new WaitForSeconds(Mathf.Max(0f, beforeVictorySeconds));
        Time.timeScale = 1f;
        if (Application.CanStreamedLevelBeLoaded(victoryScene))
            SceneManager.LoadScene(victoryScene);
        else
            Debug.LogError("Falta agregar '" + victoryScene + "' a la lista de escenas del Build Profile.");
    }

    private void PlayOneShot(string resourcePath)
    {
        var clip = Resources.Load<AudioClip>(resourcePath);
        if (clip != null && audioSource != null) audioSource.PlayOneShot(clip, 0.85f);
    }

    private void OnGUI()
    {
        if (campaignMode) return;
        GUIStyle box = new GUIStyle(GUI.skin.box);
        box.fontSize = Mathf.Clamp(Screen.height / 36, 14, 23);
        box.normal.textColor = Color.white;
        box.wordWrap = true;
        GUI.Box(new Rect(12, 12, Mathf.Min(660, Screen.width - 24), 68),
            "EL ULTIMO CASTILLO - " + statusText, box);
    }
}
