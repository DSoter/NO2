using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private bool isPaused;
    [SerializeField] private AudioSource musicSource, sfxSource;
    public void PlayMusic(AudioClip clip)
    {
        if (musicSource.isPlaying && musicSource.clip == clip)
            return; // do not restart the song if it is already playing
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
        sfxSource.volume = volume;
        sfxSource.pitch = 1 + Random.Range(-pitchVariation, pitchVariation);
        sfxSource.PlayOneShot(clip);
    }
}