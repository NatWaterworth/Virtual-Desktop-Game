using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//responds to input audio levels with a visual response
public class AudioActivatedObject : MonoBehaviour
{
    Material mat;

    [SerializeField]
    [Range(0, 8)]
    int frequencyBand; 
    // Start is called before the first frame update
    void Start()
    {
        if (GetComponent<MeshRenderer>() != null && GetComponent<MeshRenderer>().material != null)
        {
            mat = GetComponent<MeshRenderer>().material;
        }
        
    }

    public void AlterMaterialEmission(float amplitude)
    {
        //Debug.Log("AMP: "+amplitude*5);
        if (mat != null)
        {
            float h = 360, s = 100, v = 100;
            //Color.RGBToHSV(mat.GetColor("_EmissionColor"),out h,out s, out v);

            mat.SetColor("_EmissionColor", Color.red*amplitude*5);
        }
    }

    public float GetBand()
    {
        return frequencyBand;
    }
}
