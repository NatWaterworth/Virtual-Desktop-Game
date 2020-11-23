using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Checks player inputs for matching with match objects
public class MatchArea : MonoBehaviour
{
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
            colourName = "Orange";
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
            colourName = "Orange";
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
