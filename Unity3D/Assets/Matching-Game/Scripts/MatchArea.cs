using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

//Checks player inputs for matching with match objects
public class MatchArea : MonoBehaviour
{
    
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

    // Start is called before the first frame update
    void Start()
    {
        //Setup port for receiving information
        port = 5065;
        InitUDP(); 
    }

    
    //Initialises Thread to run parallel to the game
    private void InitUDP()
    {
        print("UDP Initialized");

        receiveThread = new Thread(new ThreadStart(ReceiveData));
        receiveThread.IsBackground = true; //Runs parallel to the game
        receiveThread.Start();
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
                print("OpenCV-Python Information: " + text);

                CheckInput(text);

            }
            catch (Exception e)
            {
                print(e.ToString()); //log exceptions to console
            }
        }
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
    }

    // Update is called once per frame
    void Update()
    {
        //Testing Inputs to ensure matching works.
        //All inputs will be provided by Python port.

        //Reset Inputs each frame.
        colourName = "";
        sideOfScreen = "";
        emotion = "";

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
            emotion = "Sad";
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


}
