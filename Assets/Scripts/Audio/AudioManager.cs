using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [System.Serializable]
    public class Sound
    {
        public SoundEffect id;
        public AudioClip clip;
    }

    [System.Serializable]
    public class Music
    {
        public MusicTrack id;
        public AudioClip clip;
    }

    [Header("Sound Effects")]
    public Sound[] sounds;
    private Dictionary<SoundEffect, AudioClip> soundDict;

    [Header("Music Tracks")]
    public Music[] musics;
    private Dictionary<MusicTrack, AudioClip> musicDict;

    private AudioSource musicSource;
    private AudioSource sfxSource;

    [Range(0f, 1f)] public float masterVolume = 1f;
    [Range(0f, 1f)] public float musicVolume = 1f;
    [Range(0f, 1f)] public float sfxVolume = 1f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = true;

        sfxSource = gameObject.AddComponent<AudioSource>();

        soundDict = new Dictionary<SoundEffect, AudioClip>();
        foreach (Sound s in sounds)
        {
            if (!soundDict.ContainsKey(s.id))
                soundDict.Add(s.id, s.clip);
        }

        musicDict = new Dictionary<MusicTrack, AudioClip>();
        foreach (Music m in musics)
        {
            if (!musicDict.ContainsKey(m.id))
                musicDict.Add(m.id, m.clip);
        }

        ApplyVolumes();
    }

    public void PlaySound(SoundEffect sound)
    {
        if (soundDict.ContainsKey(sound))
            sfxSource.PlayOneShot(soundDict[sound], masterVolume * sfxVolume);
        else
            Debug.LogWarning("Sonido no encontrado: " + sound);
    }

    public void PlayMusic(MusicTrack track)
    {
        if (musicDict.ContainsKey(track))
        {
            musicSource.clip = musicDict[track];
            musicSource.Play();
            ApplyVolumes();
        }
        else
        {
            Debug.LogWarning("Música no encontrada: " + track);
        }
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    public void SetMasterVolume(float value)
    {
        masterVolume = Mathf.Clamp01(value);
        ApplyVolumes();
    }

    public void SetMusicVolume(float value)
    {
        musicVolume = Mathf.Clamp01(value);
        ApplyVolumes();
    }

    public void SetSFXVolume(float value)
    {
        sfxVolume = Mathf.Clamp01(value);
        ApplyVolumes();
    }

    private void ApplyVolumes()
    {
        musicSource.volume = masterVolume * musicVolume;
        sfxSource.volume = masterVolume * sfxVolume;
    }

    private void Start()
    {
        PlayMusic(MusicTrack.MainMenu);
    }
}

