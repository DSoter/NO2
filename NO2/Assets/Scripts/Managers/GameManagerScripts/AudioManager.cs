using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    private bool isPaused;
    [SerializeField] private AudioSource musicSource;

    [Header("SFX Pool")]
    [SerializeField] private int _sfxPoolSize = 10;
    [SerializeField] private AudioMixerGroup _sfxMixerGroup;

    private AudioSource[] _sfxSources;
    private int _nextSfxIndex = 0;

    private void Awake()
    {
        _sfxSources = new AudioSource[_sfxPoolSize];

        for (int i = 0; i < _sfxPoolSize; i++)
        {
            GameObject sfxObject = new GameObject($"SFX_Source_{i}");
            sfxObject.transform.SetParent(transform);

            AudioSource source = sfxObject.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.outputAudioMixerGroup = _sfxMixerGroup;

            _sfxSources[i] = source;
        }
    }

    public void PlayMusic(AudioClip clip)
    {
        if (musicSource.isPlaying && musicSource.clip == clip)
            return;
        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    public void PauseMusic()
    {
        if (musicSource.isPlaying)
        {
            musicSource.Pause();
            isPaused = true;
        }
    }

    public void ResumeMusic()
    {
        if (isPaused)
        {
            musicSource.UnPause();
            isPaused = false;
        }
    }

    public void PlaySound(AudioClip clip, float volume = 1, float pitchVariation = 0)
    {
        AudioSource source = _sfxSources[_nextSfxIndex];
        _nextSfxIndex = (_nextSfxIndex + 1) % _sfxSources.Length;

        source.clip = clip;
        source.volume = volume;
        source.pitch = 1 + Random.Range(-pitchVariation, pitchVariation);
        source.Play();
    }
}