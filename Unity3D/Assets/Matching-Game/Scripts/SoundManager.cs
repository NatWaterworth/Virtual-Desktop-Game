using UnityEngine;
using System;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    public Sound[] sounds;
    public Sound[] music;
    public MatchSong[] songs;

    AudioVisualizer audioVisualiser;
    //objects that change visually due to audio
    AudioActivatedObject[] audioObjects;

    //song currently been played
    MatchSong currentSong;

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
        SetAllSongs(songs);
        //PlayMusic("EDM");
    }

    //Stops Current song being played
    public void StopSong()
    {
        if(currentSong!=null)
            currentSong.song.source.Stop();
    }

    //If a song is playing, pause it
    public void PauseSong()
    {
        if (currentSong != null)
            currentSong.song.source.Pause();
    }

    //If a song is paused, play it
    public void UnPauseSong()
    {
        if (currentSong != null)
            currentSong.song.source.UnPause();
    }

    private void Update()
    {
        UpdateVisualObjects();
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

    void SetAllSongs(MatchSong[] array)
    {
        foreach (MatchSong s in array)
        {
            s.song.source = gameObject.AddComponent<AudioSource>();
            s.song.source.clip = s.song.clip;
            s.song.source.volume = s.song.volume;
            s.song.source.pitch = s.song.pitch;
            s.song.source.loop = s.song.loop;
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

    //Stop a sound effect playing
    public void StopSoundEffect(string name)
    {
        //Find sound which has name
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogError("No Sound found called: " + name);
            return;
        }
        s.source.Stop();
    }

    //Stop a song playing
    public void StopMusic(string name)
    {
        //Find sound which has name
        Sound s = Array.Find(music, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogError("No Music found called: " + name);
            return;
        }
        s.source.Stop();
    }

    //Play matching music
    public void PlaySong(string name)
    {
        Debug.Log(name);
        PlayMatchSong(name, songs);
    }

    //Get song length
    public float GetSongLength(string songName)
    {
        MatchSong s = Array.Find(songs, matchSong => matchSong.song.name == songName);

        if (s == null)
        {
            Debug.LogError("No Sound found called: " + name);
            return 0;
        }
        return s.song.clip.length;
    }

    //Return a match song if there is one with name of songName
    public MatchSong GetMatchSong(string songName)
    {
        MatchSong s = Array.Find(songs, matchSong => matchSong.song.name == songName);

        if (s == null)
        {
            Debug.LogError("No Sound found called: " + name);
            return null;
        }
        return s;
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
        if(!s.source.isPlaying)
            s.source.Play();
    }

    //Search for a Song to play
    void PlayMatchSong(string name, MatchSong[] songs)
    {
        //Find sound which has name
        MatchSong s = Array.Find(songs, matchSong => matchSong.song.name == name);
        //Debug.Log(s.song.name);
        if (s == null)
        {
            Debug.LogError("No Sound found called: " + name);
            return;
        }
        s.song.source.Play();
        currentSong = s;
        SetAudioVisualizers(s.song);
    }

    //Saves score of a song
    public void StoreSongScore(string name, int score)
    {
        MatchSong s = Array.Find(songs, matchSong => matchSong.song.name == name);
        if (s == null)
        {
            Debug.LogError("No Sound found called: " + name);
            return;
        }
        s.highScore = score;
    }

    //Returns the score of song with name if found
    public int GetSongScore(string name, int score)
    {
        MatchSong s = Array.Find(songs, matchSong => matchSong.song.name == name);
        if (s == null)
        {
            Debug.LogError("No Sound found called: " + name);
            return 0;
        }
        return s.highScore;
    }

    //Setup audio visualisation
    public void SetAudioVisualizers(Sound song)
    {
        if (GetComponent<AudioVisualizer>() != null)
            audioVisualiser = GetComponent<AudioVisualizer>();
        else
        {
            gameObject.AddComponent<AudioVisualizer>();
            audioVisualiser = GetComponent<AudioVisualizer>();
            //Debug.Log("Adding audioVisualizer setup for: " + this);
        }

        //set up audio visualizer
        audioVisualiser.SetAudioSource(song.source);
        //
        audioObjects = FindObjectsOfType<AudioActivatedObject>();
    }

    void UpdateVisualObjects()
    {
        if (audioVisualiser != null)
        {
            float[] amps = audioVisualiser.GetBandAmplitudes();

            foreach (AudioActivatedObject obj in audioObjects)
            {
                for (int i = 0; i < 8; i++)
                {
                    if (obj.GetBand() == i+1)
                        obj.AlterMaterialEmission(amps[i]);
                }     
                
                if(obj.GetBand() == 0)
                    obj.AlterMaterialEmission(audioVisualiser.GetMaxBandAmpltiude());
            }
        }
    }
}
