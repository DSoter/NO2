using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class AudioManager : MonoBehaviour
{
    [Header("Music")]
    [SerializeField] private AudioSource musicSource;

    [Space(5)]
    [Header("SFX Pool")]
    [SerializeField] private ObjectPool<AudioSource> _sfxPool;
    [SerializeField] private AudioSource _sfxPrefab;
    [SerializeField] private int _defaultCapacity = 10;
    [SerializeField] private int _maxSize = 20;

    private bool isPaused;

    private void Awake()
    {
        _sfxPool = new ObjectPool<AudioSource>(
            createFunc: CreateFunc,
            actionOnGet: (source) => source.gameObject.SetActive(true),
            actionOnRelease: (source) => source.gameObject.SetActive(false),
            actionOnDestroy: (source) => Destroy(source.gameObject),
            defaultCapacity: _defaultCapacity,
            maxSize: _maxSize
            );
    }

    // ----------------------------------------------------
    // MUSIC
    // ----------------------------------------------------

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

    // ----------------------------------------------------
    // SFX
    // ----------------------------------------------------

    public void PlaySound(AudioClip clip, float volume = 1, float pitchVariation = 0, Vector3? position = null)
    {
        AudioSource source = _sfxPool.Get();

        if (position is not null)
        {
            source.spatialBlend = 1; // sonido 3D
            source.transform.position = position.Value;
        }
        else
        {
            source.spatialBlend = 0; // sonido 2D
            source.transform.position = Vector3.zero;
        }

        source.clip = clip;
        source.volume = volume;
        source.pitch = 1 + Random.Range(-pitchVariation, pitchVariation);
        source.Play();

        StartCoroutine(ReleaseWhenFinished(source, clip.length));
    }

    private AudioSource CreateFunc()
    {
        var sfxAudioSource = Instantiate(_sfxPrefab);
        sfxAudioSource.transform.parent = transform;
        return sfxAudioSource;
    }

    private IEnumerator ReleaseWhenFinished(AudioSource source, float duration)
    {
        yield return new WaitForSeconds(duration);
        _sfxPool.Release(source);
    }
}
