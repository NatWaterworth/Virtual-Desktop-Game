using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Respawns an object after a certain period of time
public class ObjectRespawner : MonoBehaviour
{
    Vector3 startingPosition;

    [SerializeField]
    float respawnTime;
    [SerializeField]
    float timer;

    [SerializeField]
    float speed;

    [SerializeField]
    Vector3 direction;

    private void Start()
    {
        startingPosition = transform.position;
        if (direction == Vector3.zero)
            direction = transform.forward;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.unscaledDeltaTime;
        transform.position += speed * transform.forward;
        if (respawnTime <= timer)
        {
            transform.position = startingPosition;
            //reset timer
            timer = 0;
        }
    }
}
