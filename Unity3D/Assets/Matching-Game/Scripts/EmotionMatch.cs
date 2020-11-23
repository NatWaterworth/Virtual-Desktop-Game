using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmotionMatch : MatchObject
{
    //Match variables
    [SerializeField]
    string emotion;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    protected override void OnTriggerStay(Collider other)
    {
        base.OnTriggerStay(other);
    }

    protected override void OnTriggerExit(Collider other)
    {
        base.OnTriggerExit(other);
    }

    public string GetEmotion()
    {
        return emotion;
    }
}
