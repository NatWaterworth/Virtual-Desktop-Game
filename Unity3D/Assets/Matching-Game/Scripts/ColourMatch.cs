using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColourMatch : MatchObject
{
    //Match variables
    [SerializeField]
    string colourName;
    [SerializeField]
    string sideOfScreen;

    [SerializeField]
    Mesh[] alternateMeshes;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        //Set a random mesh from stored possible meshes
        if (GetComponent<MeshFilter>() != null && PseudoRandomNumberGenerator.instance!=null)
            GetComponent<MeshFilter>().mesh = alternateMeshes[PseudoRandomNumberGenerator.instance.GetRandomNumber()%alternateMeshes.Length];
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    protected override void OnTriggerStay(Collider other)
    {
        base.OnTriggerStay(other);
        if (other.GetComponent<MatchArea>())
        {
           // Debug.Log("Colour: "+colourName+ ".  Side: "+sideOfScreen);
        }
    }

    protected override void OnTriggerExit(Collider other)
    {
        base.OnTriggerExit(other);
    }

    public string GetColour()
    {
        return colourName;
    }

    public string GetScreenSide()
    {
        return sideOfScreen;
    }

    //Set colourName based on objects material colour
    public void SetColour()
    {
        
    }

    public void SetScreenSide(string side)
    {
        sideOfScreen = side;
    }


}
