using UnityEngine;
using System;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    public Sound[] sounds;
    public Sound[] music;
    public MatchSong[] songs;

    // Start is called before the first frame update
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        SetAllSounds(sounds);
        SetAllSounds(music);
        PlayMusic("EDM");
    }

    void SetAllSounds(Sound[] array)
    {
        foreach (Sound s in array)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }

    //Play specified sound effect
    public void PlaySoundEffect(string name)
    {
        PlaySound(name, sounds);
    }

    //Play specified music
    public void PlayMusic(string name)
    {
        PlaySound(name, music);
    }

    //Play matching music
    public void PlaySong(string name)
    {
        PlayMatchSong(name, songs);
    }


    //Search for a sound to play
    void PlaySound(string name, Sound[] sounds)
    {
        //Find sound which has name
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogError("No Sound found called: " + name);
            return;
        }
        s.source.Play();
    }

    //Search for a sound to play
    void PlayMatchSong(string name, MatchSong[] songs)
    {
        //Find sound which has name
        MatchSong s = Array.Find(songs, matchSong => matchSong.song.name == name);
        if (s == null)
        {
            Debug.LogError("No Sound found called: " + name);
            return;
        }
        s.song.source.Play();
    }
}
