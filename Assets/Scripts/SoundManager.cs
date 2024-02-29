using UnityEngine;
using System.Collections;

public class SoundManager : Singleton<SoundManager>
{
    public AudioClip[] musicClips;
    public AudioClip[] winClips;
    public AudioClip[] loseClips;
    public AudioClip[] bonusClips;
    private AudioSource musicSource;

    [Range(0,1)]
    public float musicVolume = 0.5f;

    [Range(0,1)]
    public float fxVolume = 1.0f;

    public float lowPitch = 0.95f;
    public float highPitch = 1.05f;

	void Start () 
    {
        PlayRandomMusic();
	}
	
    public AudioSource PlayClipAtPoint(AudioClip clip, Vector3 position, float volume = 1f, bool isMusic = false, bool randomizePitch = true)
    {
        if (clip != null)
        {
            GameObject go = new GameObject("SoundFX" + clip.name);
            go.transform.position = position;

            AudioSource source = go.AddComponent<AudioSource>();
            source.clip = clip;

            if(randomizePitch){
                float randomPitch = Random.Range(lowPitch, highPitch);
                source.pitch = randomPitch;
            }

            source.volume = volume;

            source.Play();

            // If this is not for music, destroy the GameObject after clip finishes playing
            if (!isMusic)
            {
                Destroy(go, clip.length);
            }
            else
            {
                musicSource = source; // Assign the music source
                source.loop = true; // Ensure music loops
            }

            return source;
        }

        return null;
    }

    public AudioSource PlayRandom(AudioClip[] clips, Vector3 position, float volume = 1f)
    {
        if (clips != null)
        {
            if (clips.Length != 0)
            {
                int randomIndex = Random.Range(0, clips.Length);

                if (clips[randomIndex] != null)
                {
                    AudioSource source = PlayClipAtPoint(clips[randomIndex], position, volume);
                    source.loop = true;
                    return source;
                }
            }
        }
        return null;
    }

    public void PlayRandomMusic()
    {
        if (musicSource == null || !musicSource.isPlaying)
        {
            PlayClipAtPoint(musicClips[Random.Range(0, musicClips.Length)], Vector3.zero, musicVolume, isMusic: true);
        }
    }

    public void PlayWinSound()
    {
        PlayRandom(winClips, Vector3.zero, fxVolume);
    }

    public void PlayLoseSound()
    {
        PlayRandom(loseClips, Vector3.zero, fxVolume * 0.5f);
    }

    public void PlayBonusSound()
    {
        PlayRandom(bonusClips, Vector3.zero, fxVolume);
    }




}
