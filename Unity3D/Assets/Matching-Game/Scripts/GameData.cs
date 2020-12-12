using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public int[] songHighScore;

    public GameData(MatchSong[] songs)
    {
        if (songs != null)
        {
            songHighScore = new int[3]; //only three songs to save
            if (songs[0] != null)
                songHighScore[0] = songs[0].highScore;
            if (songs[1] != null)
                songHighScore[1] = songs[1].highScore;
            if (songs[2] != null)
                songHighScore[2] = songs[2].highScore;
        }
        else
        {
            Debug.LogError("GameData is Null: " + this);
        }
    }
}
