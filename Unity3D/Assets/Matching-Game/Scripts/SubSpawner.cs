using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubSpawner : MonoBehaviour
{

    //All matchable objects spawnable from this spawner
    [SerializeField]
    MatchObject[] matchObjects = new MatchObject[3];

    //All Spawn Positions that objects can spawn from
    [SerializeField]
    SpawnPoints[] spawnPositions;

    [System.Serializable]
    struct SpawnPoints
    {
        public string sideName;
        public Transform spawnPositions;
    }

    int objectMatchIndex = 0;
    int objectMatchSpawnPos = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /*Spawns an object in the game*/
    public void SpawnObject(float objectSpeed, Transform player)
    {
        //set spawn position and match object type using seed for current song.
        //objectMatchIndex = Random.Range(0, matchObjects.Length);
        //objectMatchSpawnPos = Random.Range(0, spawnPositions.Length);
        if (PseudoRandomNumberGenerator.instance != null)
        { 

            objectMatchIndex = PseudoRandomNumberGenerator.instance.GetRandomNumber() % matchObjects.Length;
            objectMatchSpawnPos = PseudoRandomNumberGenerator.instance.GetRandomNumber() % spawnPositions.Length;
    }
        //spawn object
        MatchObject obj = Instantiate(matchObjects[objectMatchIndex], spawnPositions[objectMatchSpawnPos].spawnPositions.position, Quaternion.LookRotation(-player.transform.position,Vector3.up), null);
        obj.SetSpeed(objectSpeed);
        obj.SetDirection(player.position - this.transform.position);

        if (obj.GetType().Equals(typeof(ColourMatch)))
            obj.GetComponent<ColourMatch>().SetScreenSide(spawnPositions[objectMatchSpawnPos].sideName);
    }
}
