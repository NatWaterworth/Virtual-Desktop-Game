using UnityEngine;
using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class PlayerControllerScript: MonoBehaviour 
{
    // 1. Declare Variables

    Thread receiveThread; //1
    UdpClient client; //2
    int port; //3

    public GameObject Player; //4
    AudioSource jumpSound; //5
    bool jump; //6


    // 2. Initialize variables

    void Start()
    {
        port = 5065; //1 
        jump = false; //2 
        jumpSound = gameObject.GetComponent<AudioSource>(); //3

        InitUDP(); //4
    }


    // 3. InitUDP

    private void InitUDP()
    {
        print("UDP Initialized");

        receiveThread = new Thread(new ThreadStart(ReceiveData)); 
        receiveThread.IsBackground = true; //Runs parallel to the game
        receiveThread.Start(); 
    }


    // 4. Receive Data

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
                print(">> " + text);

                jump = true;

            }
            catch (Exception e)
            {
                print(e.ToString()); //log exceptions to console
            }
        }
    }


    // 5. Make the Player Jump

    public void Jump()
    {
        Player.GetComponent<Animator>().SetTrigger("Jump"); //Trigger player jump animation
        jumpSound.PlayDelayed(44100); // Play Jump Sound with a 1 second delay to match the animation
    }

    // 6. Check for variable value, and make the Player jump!

    void Update()
    {
        if (jump)
        {
            Jump();
            jump = false;
        }
    }


}
