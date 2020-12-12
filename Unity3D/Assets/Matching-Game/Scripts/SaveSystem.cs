using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem 
{
    //Save song scores in game
    public static void SaveSongInfo(MatchSong[] songs)
    {
        //Create Binary formatter
        BinaryFormatter formatter = new BinaryFormatter();

        //Set path to save information to a data directory
        string path = Application.persistentDataPath + "/songInfo.fun"; //create save of type nat (author)

        //Create a file on the system and write to it
        FileStream stream = new FileStream(path, FileMode.Create);

        //Format data
        GameData data = new GameData(songs);

        //Insert data to file
        formatter.Serialize(stream, data);
        stream.Close();
    }

    //load saved highscores for game
    public static GameData LoadSongData()
    {
        string path = Application.persistentDataPath + "/songInfo.fun";

        if (File.Exists(path))
        {
            //Create Binary formatter
            BinaryFormatter formatter = new BinaryFormatter();

            //Open existing save file
            FileStream stream = new FileStream(path, FileMode.Open);

            //retrieve stored information and store it as GameData
            GameData data = formatter.Deserialize(stream) as GameData;
            stream.Close();

            return data;
        }
        else
        {
            Debug.LogError("Save file not found in "+ path);
            return null;
        }
    }
}
