using UnityEngine;

public sealed class MVAudio : MonoBehaviour
{
    private static MVAudio instance;
    private AudioSource music;
    private AudioSource effects;
    private string currentSong = "";

    private static MVAudio Get()
    {
        if (instance != null) return instance;
        var go = new GameObject("Mario Victor - Audio Global");
        instance = go.AddComponent<MVAudio>();
        return instance;
    }

    private void Awake()
    {
        if (instance != null && instance != this) { Destroy(gameObject); return; }
        instance = this;
        DontDestroyOnLoad(gameObject);
        music = gameObject.AddComponent<AudioSource>();
        music.loop = true;
        music.volume = 0.42f;
        music.playOnAwake = false;
        effects = gameObject.AddComponent<AudioSource>();
        effects.volume = 0.7f;
        effects.playOnAwake = false;
        effects.loop = false;
    }

    public static void Music(string clipName)
    {
        MVAudio a = Get();
        if (a.currentSong == clipName && a.music.isPlaying) return;
        a.currentSong = clipName;
        a.music.Stop();
        a.music.clip = Resources.Load<AudioClip>("MarioVictor/Audio/" + clipName);
        if (a.music.clip != null) a.music.Play();
        else Debug.LogWarning("MarioVictor: audio no encontrado: " + clipName);
    }

    public static void OneShot(string clipName, float volume = 1f)
    {
        MVAudio a = Get();
        var clip = Resources.Load<AudioClip>("MarioVictor/Audio/" + clipName);
        if (clip != null) a.effects.PlayOneShot(clip, Mathf.Clamp01(volume));
    }

    public static void StopMusic()
    {
        MVAudio a = Get();
        a.music.Stop();
        a.currentSong = "";
    }
}
