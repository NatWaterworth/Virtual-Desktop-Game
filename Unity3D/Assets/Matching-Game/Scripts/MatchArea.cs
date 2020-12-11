using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using TMPro;

//Checks player inputs for matching with match objects
public class MatchArea : MonoBehaviour
{
    //For Testing Build...
    [SerializeField]
    TextMeshProUGUI error;
    //Port vairables - Unity <-> Python
    Thread receiveThread; 
    UdpClient client; 
    int port; 

    //Match variables
    [SerializeField]
    string colourName;
    [SerializeField]
    string sideOfScreen;
    [SerializeField]
    string emotion;

    string message;

    GameManager manager;
    bool connectedOpenCV = false;

    // Start is called before the first frame update
    void Start()
    {
        //Setup port for receiving information
        port = 5065;
        InitUDP();
        if (GameManager.Instance != null)
            manager = GameManager.Instance;
        else
            Debug.LogError("Game Manager has not been assigned to: " + this);
    }

    private void Update()
    {
        //error.text = message;
    }

    //Initialises Thread to run parallel to the game
    private void InitUDP()
    {
        print("UDP Initialized");
        receiveThread = new Thread(new ThreadStart(ReceiveData));
        receiveThread.IsBackground = true; //Runs parallel to the game
        receiveThread.Start();
        message = "Initializing UDP";
    }

    //Recieves information from port on a thread running parallel to the game.
    private void ReceiveData()
    {
        client = new UdpClient(port); //assign port
        while (true) //set to variable if you don't want data to be recieved.
        {
            try
            {               
                IPEndPoint anyIP = new IPEndPoint(IPAddress.Parse("0.0.0.0"), port); //where inputs are declared
                byte[] data = client.Receive(ref anyIP); //data recieved stored in binary form

                string text = Encoding.UTF8.GetString(data); //data encoded as utf-8 string format
                //print("OpenCV-Python Information: " + text);
                //Set to connected as input is being recieved
                connectedOpenCV = true;
                message = text;
                //Assign ported information to variables in-game usage
                if (manager != null)
                {
                    manager.SetCameraActivity(CheckCamera(text));

                    //only check when games in play
                    if(manager.CanPause() && GetPauseSignal(text))
                        manager.TriggerPause();
                }
                
                CheckInput(text);
            }
            catch (Exception e)
            {
                print(e.ToString()); //log exceptions to console
                connectedOpenCV = false;
                message = e.ToString();
            }
        }
    }

    //Checks if openCV inputs are being recieved in port
    public bool IsOpenCVActive()
    {
        //Debug.Log("connected: " + connectedOpenCV);
        return connectedOpenCV;
    }

    //Checks if camera is avaialble
    public bool CheckCamera(string message)
    {
        if(message == "No Webcam")
        {
            return false;
        }
        return true;
    }

    //Checks if game must be paused
    public bool GetPauseSignal(string message)
    {
        if (message == "Pause")
        {
            return true;
        }
        return false;
    }

    //Checks if game must be paused
    public bool GetUnPauseSignal(string message)
    {
        if (message == "UnPause")
        {
            return true;
        }
        return false;
    }


    //Checks if object matches up with user inputs. E.g. Red input, left side or sad face.
    public bool CheckMatch(MatchObject matchObject)
    {
        //if object is an facial match
        if (matchObject.GetType().Equals(typeof(EmotionMatch)))
        {
            if (matchObject.GetComponent<EmotionMatch>().GetEmotion().Equals(emotion))
                return true; // correct emotion
        }

        //if object is a colour match and on the correct side of the screen
        if (matchObject.GetType().Equals(typeof(ColourMatch)))
        {
            if (matchObject.GetComponent<ColourMatch>().GetColour().Equals(colourName) && matchObject.GetComponent<ColourMatch>().GetScreenSide().Equals(sideOfScreen))
                return true; // correct colour and side
        }
        return false;
    }

    void CheckInput(string text)
    {
        //Check for Colour Input
        if (text.Contains("Blue"))
        {
            colourName = "Blue";
        }
        else if (text.Contains("Green"))
        {
            colourName = "Green";
        }
        else if (text.Contains("Orange"))
        {
            colourName = "Orange";
        }

        //check for Colour position
        if (text.Contains("Right"))
        {
            sideOfScreen = "Right";
        }
        else if (text.Contains("Left"))
        {
            sideOfScreen = "Left";
        }

        //check for Emotion position
        if (text.Contains("Neutral"))
        {
            emotion = "Neutral";
        }
        else if (text.Contains("Angry"))
        {
            emotion = "Angry";
        }
        else if (text.Contains("Happy"))
        {
            emotion = "Happy";
        }
        else if (text.Contains("Surprise"))
        {
            emotion = "Surprise";
        }
    }

    // Update is called once per frame
   /* void Update()
    {
        //Testing Inputs to ensure matching works.
        //All inputs will be provided by Python port.

        //Reset Inputs each frame.
        //colourName = "";
        //sideOfScreen = "";
        //emotion = "";

        //Left side of screen inputs
        if (Input.GetKey(KeyCode.Q))
        {
            colourName = "Blue";
            sideOfScreen = "Left";
        }
        if (Input.GetKey(KeyCode.W))
        {
            colourName = "Green";
            sideOfScreen = "Left";
        }
        if (Input.GetKey(KeyCode.E))
        {
            colourName = "Purple";
            sideOfScreen = "Left";
        }

        //Right side of screen inputs
        if (Input.GetKey(KeyCode.I))
        {
            colourName = "Blue";
            sideOfScreen = "Right";
        }
        if (Input.GetKey(KeyCode.O))
        {
            colourName = "Green";
            sideOfScreen = "Right";
        }
        if (Input.GetKey(KeyCode.P))
        {
            colourName = "Purple";
            sideOfScreen = "Right";
        }

        //Facial Recognition inputs
        if (Input.GetKey(KeyCode.R))
        {
            emotion = "Surprised";
        }
        if (Input.GetKey(KeyCode.T))
        {
            emotion = "Happy";
        }
        if (Input.GetKey(KeyCode.Y))
        {
            emotion = "Angry";
        }
        if (Input.GetKey(KeyCode.U))
        {
            emotion = "Normal";
        }
    }
    */

}
