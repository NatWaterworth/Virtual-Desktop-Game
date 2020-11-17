using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    /*
     * Spawns matching objects ahead of the player which move towards the player
     */
    float timer = 0;

    [SerializeField]
    float beat = 1;

    [SerializeField]
    float objectSpeed = 5;

    const int spawnPointsNum = 3;

    [SerializeField]
    MatchObject[] matchObjects = new MatchObject[3];

    [SerializeField]
    Transform[] spawnPositions = new Transform[spawnPointsNum];

    int objectMatchIndex = 0;
    int objectMatchSpawnPos = 0;

    Transform player;

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
            //set spawn position and match object type
            objectMatchIndex = Random.Range(0, matchObjects.Length);
            objectMatchSpawnPos = Random.Range(0, spawnPositions.Length);

            //spawn object
            MatchObject obj = Instantiate(matchObjects[objectMatchIndex], spawnPositions[objectMatchSpawnPos].position,Quaternion.identity,null);
            obj.SetSpeed(objectSpeed);
            obj.SetDirection(player.position - this.transform.position);
            timer = 0;
        }

    }
}
