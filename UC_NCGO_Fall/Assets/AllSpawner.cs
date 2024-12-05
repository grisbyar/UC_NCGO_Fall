using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;


public class AllSpawner : NetworkBehaviour
{
    //it's supposed to walk around and spawn stuff
   [SerializeField] private GameObject[] spawnableItems; //array of prefabs
   [SerializeField] private float minSpawnTime = 2f; //minimum time between spawns
   [SerializeField] private float maxSpawnTime = 5f; //maximum time between spawns
   [SerializeField] private float moveSpeed = 5f; //how fast it moves
   [SerializeField] private int itemsPerBurst = 3; //number of items spat out in random directions
   [SerializeField] private float burstForce = 5f; //force applied to items
   [SerializeField] private Transform burstPoint; //place where it bursts
   [SerializeField] private GameObject ground; //ground
    private Vector3 targetPosition; // where it is
   private bool isSpawning = true;
    
    
    
    public override void OnNetworkSpawn()
    {
            //Debug.Log($"IsServer: {IsServer}, IsOwner: {IsOwner}");

        base.OnNetworkSpawn();
        if(IsServer)
        {
            PickRandomLocation(); // make sure there is a location set on initialization
            StartCoroutine(WalkAndSpawnObjects()); // tell to start spawning on game start
            
        }
    }
    //handles walking
    private void FixedUpdate()
    {
        if (!IsServer) return; // should account for server side

        // move to target position
        //keep where it is on the y axis
        Vector3 newPosition = Vector3.MoveTowards(transform.position, targetPosition, (moveSpeed * Time.deltaTime));
        transform.position = new Vector3(newPosition.x, transform.position.y, newPosition.z);
        

        // pick a random location
        if (Vector3.Distance(transform.position, targetPosition) < 0.5f)
        {
            PickRandomLocation();
        }
    }

    //look for random location to go to
     private void PickRandomLocation()
    {
        //Debug.Log("PickRandomLocation");
        // get the bounds of the ground
         this.ground = GameObject.FindWithTag("Ground");

        Renderer groundRenderer = this.ground.GetComponent<Renderer>();
        Bounds groundBounds = groundRenderer.bounds;

        // get a random location within the grounds
        float randomX = Random.Range(groundBounds.min.x, groundBounds.max.x);
        float randomZ = Random.Range(groundBounds.min.z, groundBounds.max.z);

        // set the position
        targetPosition = new Vector3(randomX, transform.position.y, randomZ);
    }
    //spawner coroutine
    private IEnumerator WalkAndSpawnObjects(){
        while(isSpawning){
            //Debug.Log("PickRandomLocation");
            yield return new WaitForSeconds(Random.Range(minSpawnTime, maxSpawnTime));
            BurstItems();
        }
    }
    
     private void SpawnRandomItem()
    {
        //Debug.Log("Choosing Items");
        // choose a prefab
        GameObject itemPrefab = spawnableItems[Random.Range(0, spawnableItems.Length)];
        
        // instantiate the item on the server
        GameObject spawnedItem = Instantiate(itemPrefab, burstPoint.position, Quaternion.identity);
        
        //  apply force for the burst effect
        Rigidbody rb = spawnedItem.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 burstDirection = (transform.forward + Random.insideUnitSphere * 0.2f).normalized;
            rb.AddForce(burstDirection * burstForce, ForceMode.Impulse);
        }

        // spawn on the network
        spawnedItem.GetComponent<NetworkObject>().Spawn();
    }
    private void BurstItems()
    {
        for (int i = 0; i < itemsPerBurst; i++)
        {
            //Debug.Log("Spawning Items");

            SpawnRandomItem();
        }
    }
    
    public override void OnNetworkDespawn()
    {
        isSpawning = false; // stop spawning when the object is despawned
    }
 
}
