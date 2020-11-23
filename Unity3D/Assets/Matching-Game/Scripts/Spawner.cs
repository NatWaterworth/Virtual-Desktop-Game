using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    /*
     * Spawns matching objects ahead of the player which move towards the player
     */

    //number of spawners
    const int numSpawners = 2;

    //Timer to keep track of when objects spawn
    float timer = 0;

    //beat of song - rate of objects spawning
    [SerializeField]
    float beat = 1;

    //speed at which objects approach player
    [SerializeField]
    float objectSpeed = 5;

    //Player Transform for sending objects towards player
    Transform player;

    //Sub spawners to spawn match objects
    [SerializeField]
    SubSpawner[] subSpawners = new SubSpawner[numSpawners];

    // Start is called before the first frame update
    void Start()
    {    
        player = GameManager.Instance.GetPlayerTransform();

        //check player transform has been correctly assigned
        if (player == null)
            Debug.LogError("Player Transform not assigned for:" + this);
    }

    // Update is called once per frame
    void Update()
    {
        //Increment time per frame
        timer += Time.deltaTime;

        if(timer >= beat)
        {
            //Pick random spawn
            int objectMatchIndex = Random.Range(0, numSpawners);
            //Spawn Object from one of the spawners - at random.
            subSpawners[objectMatchIndex].SpawnObject(objectSpeed, player);
            //Reset timer
            timer = 0;
        }

    }

}
