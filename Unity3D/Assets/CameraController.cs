using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    //to indicate which state camera is in: play or menu
    bool playmode;

    //Speed at which camera lerps between two rotations
    [SerializeField]
    [Min(0.001f)]
    float lerpSpeed = 0.05f;

    [SerializeField]
    float xRotMenu = 135, xRotPlay = 35;


    // Update is called once per frame
    void Update()
    {
        
    }

    //Set camera to transition to new rotation
    public void SetCameraToPlayMode(bool play)
    {
        playmode = play;
        if (play)
            StartCoroutine(LerpToPlayRotation());
        else
            StartCoroutine(LerpToMenuRotation());
    }

    IEnumerator LerpToPlayRotation()
    {
        float perc = 0;
        //while difference between current and desired state is greater than .5
        while(perc<1)
        {
            transform.rotation = Quaternion.Euler(Mathf.Lerp(xRotMenu,xRotPlay,Mathf.Clamp01(perc)), transform.eulerAngles.y, transform.eulerAngles.z);
            perc += lerpSpeed;
            yield return new WaitForSeconds(.05f);
        }
        transform.rotation = Quaternion.Euler(xRotPlay, transform.eulerAngles.y, transform.eulerAngles.z);
    }

    IEnumerator LerpToMenuRotation()
    {
        float perc = 0;
        //while difference between current and desired state is greater than .5
        while (perc < 1)
        {
            transform.rotation = Quaternion.Euler(Mathf.Lerp(xRotPlay,xRotMenu, Mathf.Clamp01(perc)), transform.eulerAngles.y, transform.eulerAngles.z);
            perc += lerpSpeed;
            yield return new WaitForSeconds(.05f);
        }
        transform.rotation = Quaternion.Euler(xRotMenu, transform.eulerAngles.y, transform.eulerAngles.z);
    }
}
