using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    /*
     * Spawns matching objects ahead of the player which move towards the player
     */

    //Toggle for spawning objects
    bool spawning;

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
    SubSpawner[] subSpawners;

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
        if (spawning)
        {
            //Increment time per frame
            timer += Time.deltaTime;

            if (timer >= beat)
            {
                //Pick random spawn
                int objectMatchIndex = Random.Range(0, subSpawners.Length);
                //Spawn Object from one of the spawners - at random.
                subSpawners[objectMatchIndex].SpawnObject(objectSpeed, player);
                //Reset timer
                timer = 0;
            }
        }
    }

    //Toggle Spawner ON/OFF
    public void SetSpawnerActive(bool isOn)
    {
        spawning = isOn;
    }

    //Set spawner to match songs beat and object speed
    public void SetSpawnerForSong(float newSpeed, float newBeat)
    {
        objectSpeed = newSpeed;
        beat = newBeat;
    }

    //Returns the delay required for the first object match to line up with song beat
    public float GetSongDelay()
    {
        // time = distance / velocity
        Debug.Log("delay: " + Vector3.Distance(transform.position, player.transform.position) / objectSpeed + " dis: "+ Vector3.Distance(transform.position, player.transform.position) + "speed: "+ objectSpeed);
        return Vector3.Distance(transform.position, player.transform.position) / objectSpeed;
    }

}
