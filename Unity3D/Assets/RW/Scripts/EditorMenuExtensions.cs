#if UNITY_EDITOR //Ensures it only functions in editor
using UnityEngine;
using UnityEditor;
using System.Diagnostics;
using System;

public static class EditorMenuExtensions
{
    [MenuItem("Tools/Run Open CV")]
    public static void RunOpenCV()
    {

        string pathToExe = Application.dataPath.Replace(@"/", @"\"); //Replace forward slashes with backslashes for formatting in CMD
        string command = pathToExe + "cd Assets/Python/";
        command.Replace(@"/", @"\");
        //command = "-NoExit -Command " + command;
        Process process = Process.Start("cmd.exe", command);
        //process.CloseMainWindow();
        UnityEngine.Debug.Log(Application.dataPath);
        process.WaitForExit();
        process.Close();
    }

    [MenuItem("Tools/Run Open CV2")]
    public static void RunOpenCV2()
    {
        var processInfo = new ProcessStartInfo("cmd.exe", @"ffmpeg -i " + "Assets/Python/python Recognition.py" + @" -acodec libvorbis -vcodec libtheora -f ogg " + "Assets/Python/python Recognition.py".Split('.')[0] + @".ogg");
        processInfo.CreateNoWindow = false;
        processInfo.UseShellExecute = false;

        var process = Process.Start(processInfo);

        process.WaitForExit();
        process.Close();
    }

}
#endif