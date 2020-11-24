using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//A song in the game to make matches to the beat of
[System.Serializable]
public class MatchSong
{
    //Song to play to
    public Sound song;
    //highest score for this song
    public int highScore;
    //for giving the exact same sequence of actions each time this song is played
    public int seed;
    //for syncing song with match objects
    public float beat, objectSpeed, songDelay;
}
