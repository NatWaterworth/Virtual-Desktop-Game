using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Checks player inputs for matching with match objects
public class MatchArea : MonoBehaviour
{
    //have all match objects pair them with inputs using inputs triggers match object.
    [System.Serializable]
    struct InputObjectPair
    {
        public string name;
        public GameObject matchObject;      
    }

    [SerializeField]
    InputObjectPair[] matches;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
