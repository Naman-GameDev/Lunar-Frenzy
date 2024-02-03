using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;
    private AudioSource musicSource, effectsSource;
    [SerializeField] private AudioClip[] bgMusic;
    [SerializeField] private TimeManager timeManager;

    public enum Sounds
    {
        GuyAttack1, GuyAttack2, WWAttack1, WWAttack2, Jump, EnemyDie, GameOver
    }

    [System.Serializable]
    public struct SoundList
    {
        public Sounds sounds;
        public AudioClip audioClip;
    }

    public SoundList[] soundList;

    private bool fadeCalled;    

    private void OnEnable()
    {
        GameManager.OnGameStateChange += ChangeMusic;
    }

    private void OnDisable()
    {
        GameManager.OnGameStateChange -= ChangeMusic;
    }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
        {
            Destroy(gameObject); 
        }
        DontDestroyOnLoad(this);

        musicSource = gameObject.transform.GetChild(0).GetComponent<AudioSource>();
        effectsSource = gameObject.transform.GetChild(1).GetComponent<AudioSource>();
    }

    private void Start()
    {
        musicSource.Play();
    }

    private void Update()
    {
        MusicFade();
    }

    private AudioClip GetAudioClip(Sounds sound)
    {
        foreach (var soundAudioClip in soundList)
        {
            if(soundAudioClip.sounds == sound)
            {
                return soundAudioClip.audioClip;
            }
        }
        return null;
    }

    public void PlaySound(Sounds sound, float volume)
    {
        effectsSource.PlayOneShot(GetAudioClip(sound), volume);
    }

    public void PlaySound(Sounds sound)
    {
        effectsSource.PlayOneShot(GetAudioClip(sound));
    }

    private void ChangeMusic(GameManager.GameState gameState)
    {
        if(gameState == GameManager.GameState.Day)
        {
            musicSource.clip = bgMusic[0];
            musicSource.Play();
        }
        else if(gameState == GameManager.GameState.Night)
        {
            musicSource.clip = bgMusic[1];
            musicSource.Play();
        }
        else if(gameState == GameManager.GameState.GameOver)
        {
            musicSource.Stop();
        }
    }

    private void MusicFade()
    {
        //Fade out day BGM
        if(timeManager.currentTime >= 0.96f && timeManager.currentTime <= 0.97f && !fadeCalled)
        {
            StartCoroutine(StartFade(musicSource, 0f, 3f, () => {fadeCalled = false;}));
            fadeCalled = true;
        }
        //Fade in night BGM
        if(timeManager.currentTime >= 0f && timeManager.currentTime <= 0.1f  && !fadeCalled)
        {
            StartCoroutine(StartFade(musicSource, 0.4f, 3f, () => {fadeCalled = false;}));
            fadeCalled = true;
        }
        //Fade out night BGM
        if(timeManager.currentTime >= 0.34f && timeManager.currentTime <= 0.27f && !fadeCalled)
        {
            StartCoroutine(StartFade(musicSource, 0f, 2f, () => {fadeCalled = false;}));
            fadeCalled = true;
        }
        //Fade in day BGM
        if(timeManager.currentTime >= 0.375f && timeManager.currentTime <= 0.3f && !fadeCalled)
        {
            StartCoroutine(StartFade(musicSource, 0.4f, 3f, () => {fadeCalled = false;}));
            fadeCalled = true;
        }

    }

    private static IEnumerator StartFade(AudioSource audioSource, float targetVolume, float duration, System.Action onComplete = null)
    {
        float currentTime = 0;
        float start = audioSource.volume;
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(start, targetVolume, currentTime / duration);
            yield return null;
        }
        onComplete?.Invoke();
        yield break;
    }

}

