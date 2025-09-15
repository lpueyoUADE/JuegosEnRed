using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

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

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioMixer audioMixer;

    [Range(0f, 1f)] public float masterVolume = 1f;
    [Range(0f, 1f)] public float musicVolume = 1f;
    [Range(0f, 1f)] public float sfxVolume = 1f;

    private const string MASTER_VOLUME = "MasterVolume"; 
    private const string MUSIC_VOLUME = "MusicVolume"; 
    private const string SFX_VOLUME = "SFXVolume";

    public static event Action InitCompleted;

    private System.Random rng = new System.Random();
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

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
    }

    public void PlaySoundChoice(params SoundEffect[] soundsToChoose)
    {
        // Elegir un índice aleatorio
        int index = rng.Next(soundsToChoose.Length);
        SoundEffect chosen = soundsToChoose[index];

        PlaySound(chosen);
    }

    public void PlaySound(SoundEffect sound)
    {
        if (soundDict.ContainsKey(sound))
            sfxSource.PlayOneShot(soundDict[sound]);
        else
            Debug.LogWarning("Sonido no encontrado: " + sound);
    }

    public void PlayMusic(MusicTrack track)
    {
        if (musicDict.ContainsKey(track))
        {
            musicSource.clip = musicDict[track];
            musicSource.Play();
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
        audioMixer.SetFloat(MASTER_VOLUME, ToDecibels(masterVolume));
        PlayerPrefs.SetFloat(MASTER_VOLUME, masterVolume);
    }

    public void SetMusicVolume(float value)
    {
        musicVolume = Mathf.Clamp01(value);
        audioMixer.SetFloat(MUSIC_VOLUME, ToDecibels(musicVolume));
        PlayerPrefs.SetFloat(MUSIC_VOLUME, musicVolume);
    }

    public void SetSFXVolume(float value)
    {
        sfxVolume = Mathf.Clamp01(value);
        audioMixer.SetFloat(SFX_VOLUME, ToDecibels(sfxVolume));
        PlayerPrefs.SetFloat(SFX_VOLUME, sfxVolume);
    }

    private float ToDecibels(float value)
    {
        return value > 0 ? Mathf.Log10(value) * 20f : -80f; // -80 dB = silencio
    }

    private void Start()
    {
        SetMasterVolume(PlayerPrefs.GetFloat(MASTER_VOLUME));
        SetMusicVolume(PlayerPrefs.GetFloat(MUSIC_VOLUME));
        SetSFXVolume(PlayerPrefs.GetFloat(SFX_VOLUME));

        InitCompleted?.Invoke();
        PlayMusic(MusicTrack.MainMenu);
    }
}

