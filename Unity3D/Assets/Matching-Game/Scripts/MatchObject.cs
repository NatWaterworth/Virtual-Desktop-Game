using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchObject : MonoBehaviour
{

    bool canBeMatched = false;

    const int matchScoreIncrease = 10;
    
    float speed; // control object velocity magnitude
    Rigidbody rb; // rigibody of match object to apply velocity to
    Vector3 dir; //direction of movement
    Vector3 size = Vector3.one; //size of matching object
    bool beenMatched = false;

    // Start is called before the first frame update
    void Start()
    {
        //Ensure the match object has a rigidbody to apply movement to
        if (GetComponent<Rigidbody>() != null)
            rb = GetComponent<Rigidbody>();
        else
            rb = gameObject.AddComponent<Rigidbody>();

        SetPhysics();
        SpawnEffect("SpawnEffect");
        //Apply growing effect - scaling up to size
        StartCoroutine(GrowToSize());
    }
    //Spawn VFX to help mask the instantiation of matching object
    void SpawnEffect(string path)
    {
        //Spawn Effect
        if (Resources.Load<GameObject>(path) != null)
        {
            Instantiate(Resources.Load<GameObject>(path), transform.position, Quaternion.identity, null);
        }
        else
        {
            Debug.LogWarning("No Spawn Effect found for " + this);
        }      
    }

    //Ensure matching object can move under physics condiitons
    void SetPhysics()
    {
        if (rb != null)
        {
            rb.useGravity = false;
            rb.isKinematic = false;
        }
    }

    //Physics Update
    private void FixedUpdate()
    {
        rb.velocity = dir * speed;
        transform.rotation *= Quaternion.Euler(Vector3.one);
    }

    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }

    public void SetDirection(Vector3 newDir)
    {
        dir = newDir;
    }

    private void OnTriggerEnter(Collider other)
    {
        //This object is in a region in which it can be matched
        if (other.GetComponent<MatchArea>())
        {
            canBeMatched = true;
            if (!beenMatched)
            {
                SuccessfulMatch();
            }
        }
    }


    private void OnTriggerExit(Collider other)
    {
        //This object is leaving a region in which it can be matched
        if (other.GetComponent<MatchArea>() && !beenMatched)
        {
            canBeMatched = false;
            NoMatch();
        }
    }

    void NoMatch()
    {
        //Play Sound
        FindObjectOfType<SoundManager>().PlaySoundEffect("Incorrect");

        //Increase Player Score
        GameManager.Instance.ResetCombo();

        //apply matched visual effect
        StartCoroutine(ShrinkAndDestroy());//testing
    }

    void SuccessfulMatch()
    {
        //add points
        //increase combo
        beenMatched = true;

        //Play Sound
        FindObjectOfType<SoundManager>().PlaySoundEffect("Correct");

        //Increase Player Score
        GameManager.Instance.IncreaseScore(matchScoreIncrease);

        //apply matched visual effect
        StartCoroutine(ShrinkAndDestroy());//testing

        //Effect
        SpawnEffect("SuccessfulMatch");
    }

    IEnumerator GrowToSize()
    {
        //Play Sound
        FindObjectOfType<SoundManager>().PlaySoundEffect("Spawn");

        //set initial size to 0 so it must grow.
        transform.localScale = Vector3.one * .1f;

        while (transform.lossyScale.magnitude < size.magnitude)
        {
            transform.localScale *= 1.4f;
            yield return new WaitForSeconds(0.05f);
        }
        yield return null;
    }

    IEnumerator ShrinkAndDestroy()
    {
        //assuming it already has size.

        while (transform.lossyScale.magnitude > .05f)
        {
            Debug.Log("Shrinking");
            transform.localScale /= 2f;
            yield return new WaitForSeconds(0.05f);
        }
        Destroy(this.gameObject);
        Debug.Log("Shrank!");
        yield return null;
    }
}
