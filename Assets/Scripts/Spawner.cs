using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    #region Global Variables
    [Header("Timer Settings")]
    [SerializeField] private float      spawnTime;                          // * Maximum amount of time to wait before trying to spawn an object (In seconds)
    private float                       timer;                              // * A timer for counting time between frames

    [Header("Spawn Settings")]
    [SerializeField] private int        maxObjects;                         // * The maximum objects this spawner can store
    [SerializeField] private GameObject spawnee;                            // * The object to be spawned
    private int                         currObjects = 0;                    // * The current objects stored by the spawner

    [Header("Raycasts")]
    [SerializeField] private Collider[] collisions;                         // * This is used for storing each instance of an object
    private RaycastHit[]                hit         = new RaycastHit[1];    // * A raycasthit for determining where to instantiate an object
    #endregion

    #region Gizmos
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(transform.position, transform.localScale);
    }
    #endregion
    
    #region Default Unity Methods
    void Start()
    {
        collisions = new Collider[maxObjects];
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (currObjects < maxObjects && timer > spawnTime)
        {
            int ran = Random.Range(0, 5000);
            if (currObjects >= 3 && ran > 4000 || currObjects < 3 && ran > 2000)
            {
                Object obj = Instantiate(spawnee, RandomLocation(), RandomRotation());
                obj.GetComponent<BasicAI>().spawner = gameObject;
            }
            timer = 0;
        }
    }

    void LateUpdate()
    {
        currObjects = CubeCast();
    }
    #endregion

    #region Helper Methods
    /// <summary>
    /// Creates a cube and provides that data to `collisions`
    /// </summary>
    /// <returns>The number of objects in the collision</returns>
    private int CubeCast()
    {
        return Physics.OverlapBoxNonAlloc(gameObject.transform.position, transform.localScale / 2, collisions, Quaternion.identity, 1);
    }

    /// <summary>
    /// Generates a random rotation
    /// </summary>
    /// <returns>A Vector3 of that random rotation</returns>
    private Vector3 RandomLocation()
    {
        // Generates the random X and Z coordinate
        float randomX = Random.Range(-gameObject.transform.localScale.x / 2, gameObject.transform.localScale.x / 2);
        float randomZ = Random.Range(-gameObject.transform.localScale.z / 2, gameObject.transform.localScale.z / 2);

        // Generates a raycast that points down and tries to connect with terrain 
        // `temp` is an integer of the count of objects collided with (Should always be 1)
        int temp = Physics.RaycastNonAlloc(new Vector3(randomX, gameObject.transform.localScale.y / 2, randomZ) + gameObject.transform.position, Vector3.down, hit, gameObject.transform.localScale.y, 1 << 3);

        // If temp connected with something, we return the point of that connection
        if (temp != 0) return hit[0].point;

        // Else, we log this as an error and instantiate at the local origin of the gameObject
        Debug.LogError($"Spawner {gameObject.name} failed to instantiate an object! The raycast didn't connect. Instantiating at the origin...");
        return gameObject.transform.position;
    }

    /// <summary>
    /// Generates a random Quaternion rotation around the y-axis
    /// </summary>
    /// <returns>A Quaternion of rotation around the y-axis</returns>
    private Quaternion RandomRotation()
    {
        return Quaternion.Euler(0, Random.Range(0f, 360f), 0);
    }
    #endregion
}
