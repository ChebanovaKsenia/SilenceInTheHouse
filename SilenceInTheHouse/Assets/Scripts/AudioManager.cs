using UnityEngine;
using UnityEngine.Audio;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Mixer")]
    public AudioMixer audioMixer;

    [Header("Music")]
    public AudioClip menuMusic;
    private AudioSource musicSource;
    private float musicVolume = 0.5f;

    [Header("SFX")]
    private float sfxVolume = 0.5f;

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

        //создаём AudioSource для музыки
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = true;
        musicSource.playOnAwake = false;

        //привязываем к группе Music в микшере
        if (audioMixer != null)
        {
            musicSource.outputAudioMixerGroup = audioMixer.FindMatchingGroups("Music")[0];
        }

        //загружаем музыку
        if (menuMusic != null)
        {
            musicSource.clip = menuMusic;
        }

        //загружаем сохранённые значения громкости
        LoadVolumes();
    }

    void Start()
    {
        //начинаем играть музыку в главном меню
        PlayMenuMusic();
    }

    public void PlayMenuMusic()
    {
        if (musicSource.clip != menuMusic)
        {
            musicSource.clip = menuMusic;
            musicSource.Play();
        }
        else if (!musicSource.isPlaying)
        {
            musicSource.Play();
        }

        //плавное появление музыки (2 сек)
        StartCoroutine(FadeMusicIn(2f));
    }

    public void StopMenuMusic()
    {
        StopAllCoroutines();
        StartCoroutine(FadeMusicOut());
    }

    IEnumerator FadeMusicIn(float duration = 2f)
    {
        musicSource.volume = 0f;
        if (!musicSource.isPlaying)
        {
            musicSource.Play();
        }

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(0f, musicVolume, elapsed / duration);
            yield return null;
        }

        musicSource.volume = musicVolume;
    }

    IEnumerator FadeMusicOut(float duration = 1.5f)
    {
        float startVolume = musicSource.volume;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / duration);
            yield return null;
        }

        musicSource.volume = 0f;
        musicSource.Stop();
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = volume;
        musicSource.volume = volume;

        //конвертируем в децибелы для AudioMixer
        float db = volume > 0 ? Mathf.Log10(volume) * 20 : -80f;
        audioMixer.SetFloat("MusicVolume", db);

        PlayerPrefs.SetFloat("MusicVolume", volume);
        PlayerPrefs.Save();
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = volume;
        AudioListener.volume = volume;

        float db = volume > 0 ? Mathf.Log10(volume) * 20 : -80f;
        audioMixer.SetFloat("SFXVolume", db);

        PlayerPrefs.SetFloat("SFXVolume", volume);
        PlayerPrefs.Save();
    }

    void LoadVolumes()
    {
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 0.5f);

        musicSource.volume = musicVolume;
        AudioListener.volume = sfxVolume;

        float musicDB = musicVolume > 0 ? Mathf.Log10(musicVolume) * 20 : -80f;
        float sfxDB = sfxVolume > 0 ? Mathf.Log10(sfxVolume) * 20 : -80f;

        audioMixer.SetFloat("MusicVolume", musicDB);
        audioMixer.SetFloat("SFXVolume", sfxDB);
    }

    public void PlayLevelCompleteMusic()
    {
        // Останавливаем любую текущую корутину затухания
        StopAllCoroutines();

        if (musicSource.clip != menuMusic)
        {
            musicSource.clip = menuMusic;
        }

        // Сбрасываем громкость и запускаем музыку
        musicSource.volume = 0f;

        if (!musicSource.isPlaying)
        {
            musicSource.Play();
        }

        // Медленное появление музыки за 3 секунды
        StartCoroutine(FadeMusicIn(3f));

        Debug.Log("PlayLevelCompleteMusic: музыка запущена с плавным появлением");
    }
}