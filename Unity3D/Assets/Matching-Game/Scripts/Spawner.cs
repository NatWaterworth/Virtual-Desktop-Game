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
    SpawnSet[] subSpawners;
    const int colourSpawner = 0, emotionSpawner = 1;

    int totalWeight;

    //used to hold spawned objects and destroy them on level reset
    GameObject objectSpawnHolder;

    [System.Serializable]
    struct SpawnSet
    {
        public SubSpawner spawner;
        //A value between indicating weight of spawner activity
        public int weight;
        public int cummulitiveWeight;
    }

    // Start is called before the first frame update
    void Start()
    {    
        //set player reference
        player = GameManager.Instance.GetPlayerTransform();

        //set spawned object holder
        objectSpawnHolder = new GameObject("SpawnedObjectHolder");

        //check player transform has been correctly assigned
        if (player == null)
            Debug.LogError("Player Transform not assigned for:" + this);

        SetTotalSpawnWeight();

    }

    void SetTotalSpawnWeight()
    {
        totalWeight = 0;
        for(int i =0; i < subSpawners.Length; i++)
        { 
            //add spawner weight to total
            totalWeight += subSpawners[i].weight;
            //assign new weight to spawner equal to its upper threshold based on all previous spawners weights
            subSpawners[i].cummulitiveWeight = totalWeight;
        }
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
                int objectMatchWeight = Random.Range(0, totalWeight);

                //go through each spawner and check if weight is less than threshold
                for (int i = 0; i < subSpawners.Length; i++)
                {
                    if (subSpawners[i].cummulitiveWeight > objectMatchWeight)
                    {
                        //Spawn Object from one of the spawners
                        subSpawners[i].spawner.SpawnObject(objectSpeed, player, objectSpawnHolder.transform);
                        break;
                    }
                }
                
                //Reset timer
                timer = 0;
            }
        }
    }

    public void SetEmotionSpawnerRate(int weight)
    {
        subSpawners[emotionSpawner].weight = weight;
    }

    public void SetColourSpawnerRate(int weight)
    {
        subSpawners[colourSpawner].weight = weight;
    }

    //Resets all spawned objects
    public void ResetSpawner()
    {
        MatchObject[] spawnedObjects = objectSpawnHolder.GetComponentsInChildren<MatchObject>();
        foreach(MatchObject obj in spawnedObjects)
        {
            Destroy(obj.gameObject);
        }
        timer = 0;
    }

    //Toggle Spawner ON/OFF
    public void SetSpawnerActive(bool isOn)
    {
        //Set total spawn weight based on the set spawners.
        SetTotalSpawnWeight();
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
       //Debug.Log("delay: " + Vector3.Distance(transform.position, player.transform.position) / objectSpeed + " dis: "+ Vector3.Distance(transform.position, player.transform.position) + "speed: "+ objectSpeed);
        return Vector3.Distance(transform.position, player.transform.position) / objectSpeed;
    }

}
