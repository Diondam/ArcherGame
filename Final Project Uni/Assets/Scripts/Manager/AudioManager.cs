using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

public class AudioManager : SerializedMonoBehaviour
{
    #region Variables
    public static AudioManager Instance { get; private set; }
    [SerializeField] private AudioSource audioSource, musicSource, musicSource2;

    [SerializeField] private Dictionary<string, AudioClip> soundEffectsDict;
    [SerializeField] private Dictionary<string, AudioClip> backgroundMusicDict;
    [SerializeField] private AnimationCurve transitionCurve;
    [Range(0, 1)] public float volume;
    #endregion

    #region UnityMethod
    public void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        //SceneMusic(0); //test only
    }
    #endregion

    #region Methods

    public void PlaySoundEffect(string soundName)
    {
        if (soundEffectsDict.TryGetValue(soundName, out var clip))
        {
            audioSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning($"Sound effect '{soundName}' not found in the dictionary!");
        }
    }

    public void PlayBackgroundMusic(string sceneNumber)
    {
        if (backgroundMusicDict.TryGetValue(sceneNumber, out var clip))
        {
            musicSource.clip = clip;
            musicSource.Play();
        }
        else
        {
            Debug.LogWarning($"Background music for scene '{sceneNumber}' not found in the dictionary!");
        }
    }

    private async UniTask TransitionMusicAsync(string backgroundMusic)
    {
        float time = 0f;
        float duration = 1f;

        AudioClip newClip = await LoadAudioClipAsync(backgroundMusic);
        if (newClip != null)
        {
            musicSource2.clip = newClip;
            musicSource2.Play();
        }
        else
        {
            Debug.LogWarning($"Background music for scene '{backgroundMusic}' not found in the dictionary!");
        }

        while (time < duration)
        {
            time += Time.deltaTime;
            float volumeTransition = transitionCurve.Evaluate(time);
            musicSource.volume = (1 - volumeTransition) * volume;
            musicSource2.volume = volumeTransition * volume;
            await UniTask.Yield();
        }

        AudioSource temp = musicSource;
        musicSource = musicSource2;
        musicSource2 = temp;
        musicSource2.Stop();
    }

    private async UniTask<AudioClip> LoadAudioClipAsync(string sceneNumber)
    {
        if (backgroundMusicDict.TryGetValue(sceneNumber, out var clip))
        {
            // Simulate asynchronous loading
            return clip;
        }
        return null;
    }
    #endregion

    #region Ults
    [FoldoutGroup("Event Test")]
    [Button]
    public void PlaySound(string soundName)
    {
        PlaySoundEffect(soundName);
    }

    [FoldoutGroup("Event Test")]
    [Button]
    public void PlaySceneMusic(string sceneNumber)
    {
        PlayBackgroundMusic(sceneNumber);
    }

    [FoldoutGroup("Event Test")]
    [Button]
    public void ChangeMusic(string BGMId)
    {
        TransitionMusicAsync(BGMId).Forget();
    }

    #endregion
}