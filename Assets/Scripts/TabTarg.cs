using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEditor.PackageManager;
using UnityEngine;

using Random = UnityEngine.Random;

#region Creature Class
/// <summary>
/// An object for creatures. It stores the distance, the object, and whether we have seen them yet
/// Used for our CreatureQueue class
/// </summary>
[System.Serializable]
public class Creature
{
    #region Variables
    [SerializeField] 
    private GameObject Obj;                                         // * The gameobject itself
    public float                        Distance    {get; set;}     // * The distance from the player to the object. For sorting.
    public GameObject                   Object      => Obj;         // * The object, again, so I can serialize the field (Fuck you, Unity >:( )
    public bool                         Seen        {get; set;}     // * Whether we've seen this object yet in our Pruning operations.
    #endregion

    /// <summary>
    /// Overridden constructor
    /// </summary>
    /// <param name="creature">A GameObject</param>
    /// <param name="distance">A float</param>
    public Creature(GameObject creature, float distance)
    {
        this.Distance = distance;
        this.Obj = creature;
    }

    /// <summary>
    /// Default destructor
    /// </summary>
    ~Creature() { /* Intentionally left empty */ }
}
#endregion

#region CreatureQueue Class
/// <summary>
/// A custom implementation of a queue for tab-targeting purposes
/// </summary>
static class CreatureQueue
{
    private static int              currentIndex    = 0;                        // * The currently stored index. Used for iterating through the list on Tab.
    public static List<Creature>    creatures       = new List<Creature>();     // * The list of Creatures used for tab-targeting.

    /// <summary>
    /// Checks the first creature in the list.
    /// </summary>
    /// <returns>null if there is no creatures in the list. Otherwise, the currently referrenced creature.</returns>
    public static Creature Peek()
    {
        if (creatures.Count == 0 || currentIndex >= creatures.Count)
        {
            return null;
        }
        return creatures[currentIndex];
    }

    /// <summary>
    /// Gets the next creature in the list.
    /// </summary>
    /// <returns>null if there are no creatures. Otherwise, the result of peek on the next index.</returns>
    public static Creature NextCreature()
    {
        if (creatures.Count == 0)
        {
            Debug.LogWarning("CreatureQueue is empty.");
            return null;
        }
        currentIndex = (currentIndex + 1) % creatures.Count;
        return Peek();
    }

    /// <summary>
    /// Appends a creature to the end of the list.
    /// </summary>
    /// <param name="creature">A Creature object</param>
    public static void Append(Creature creature)
    {
        creature.Seen = true;
        creatures.Add(creature);
    }

    /// <summary>
    /// Checks if a creature is already in the list. Appends it if not.
    /// </summary>
    /// <param name="creature">A Creature object</param>
    public static void CheckList(Creature creature)
    {
        foreach (var existing in creatures)
        {
            if (existing.Object.name == creature.Object.name)
            {
                existing.Seen = true;
                return;
            }
        }
        Append(creature);
        return;
    }

    /// <summary>
    /// Prunes the list. Removes every creature that hasn't been "Seen."
    /// </summary>
    public static void Prune()
    {
        creatures.RemoveAll(creature => !creature.Seen);

        foreach (var creature in creatures)
        {
            creature.Seen = false;
        }

        currentIndex = Mathf.Min(currentIndex, creatures.Count - 1);
    }
}
#endregion

public class TabTarg : MonoBehaviour
{
    #region Targeting Settings
    [Header("Targeting Settings")]
    [SerializeField] private Creature       target;                                 // * The currently selected target
    [SerializeField] private float          softAggro;                              // * The softAggro radius
    [SerializeField] private float          hardAggro;                              // * The hard aggro radius
    #endregion
    
    #region Private Fields
    [Header("Internal Data")]
    private RaycastHit[] rayHits            = new RaycastHit[50];                   // * An array of data about colliders our rays have connected with.
    private Collider[]   softHits           = new Collider[50];                     // * An array of colliders in our softAggro radius.
    private Collider[]   hardHits           = new Collider[10];                     // * An array of colliders in our hardAggro radius.
    private bool         targeting          = false;                                // * A flag to represent whether we are currently targeting something.
    private int          hardCollisions;                                            // * A count of the number of collisions in out hardAggro radius
    #endregion

    #region Default Unity Methods
    void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            // ? Could just reference softCollisionsCount as global and make this prettier.
            int softCollisionsCount = Soft();

            if (softCollisionsCount != 0)
            {
                RayGen(softCollisionsCount);
            }
            target = CreatureQueue.NextCreature();
        }

        hardCollisions = Physics.OverlapSphereNonAlloc(gameObject.transform.position, hardAggro, hardHits, 1);
    }
    #endregion

    #region Helper Methods
    /// <summary>
    /// Generates the count of objects in the softAggro radius
    /// </summary>
    /// <returns>int of the number of objects in the softAggro radius</returns>
    int Soft()
    {
        return Physics.OverlapSphereNonAlloc(gameObject.transform.position, softAggro, softHits, 1);
    }

    /// <summary>
    /// Generates a ray for every hit inside the softAggro radius.
    /// </summary>
    /// <param name="soft">The count of objects in the softAggro radius</param>
    void RayGen(int soft)
    {
        for (int i = 0; i < soft; ++i)
        {
            Vector3 deg = (softHits[i].transform.position - transform.position).normalized;
            
            int hitCount = Physics.RaycastNonAlloc(transform.position, deg, rayHits, softAggro, 1);

            for (int j = 0; j < hitCount; ++j)
            {
                CreatureQueue.CheckList(new Creature(rayHits[j].collider.gameObject, rayHits[j].distance));
            }
        }

        if (!targeting)
        {
            targeting = true;
            target = CreatureQueue.NextCreature();
        }
        else
        {
            // If we've reached the end of the list, I want to generate a new list.
            // ? What the fuck does this mean???????
            CreatureQueue.Prune();
        }
    }
    #endregion

    #region Sorting Algorithm
    /// <summary>
    /// Implementation of QuickSort.
    /// </summary>
    /// <param name="arr">An array of floats</param>
    /// <param name="lo">The first index in the array</param>
    /// <param name="hi">The last index in the array</param>
    void SortArrs(Creature[] arr, int lo, int hi)
    {
        if (lo >= hi || lo < 0) return;
        int p = Partition(arr, lo, hi);

        SortArrs(arr, lo, p - 1);
        SortArrs(arr, p + 1, hi);
    }

    /// <summary>
    /// A partitioning algorithm for QuickSort using random partitioning
    /// </summary>
    /// <param name="arr">An array of floats</param>
    /// <param name="lo">The first index in the array</param>
    /// <param name="hi">The last index in the array</param>
    /// <returns></returns>
    int Partition(Creature[] arr, int lo, int hi)
    {
        int pivot = Random.Range(lo, hi + 1);
        float pivotVal = arr[pivot].Distance;

        Swap(lo, pivot);

        int i = lo + 1;

        for (int j = lo + 1; j <= hi; ++j)
        {
            if (arr[j].Distance < pivotVal)
            {
                Swap(i, j);
                ++i;
            }
        }

        Swap(lo, i - 1);

        return i - 1;
    }

    /// <summary>
    /// A basic swapping algorithm for two related arrays.
    /// </summary>
    /// <param name="i">The first element to be swapped</param>
    /// <param name="j">The second element to be swapped</param>
    void Swap(int i, int j)
    {
        Creature temp = CreatureQueue.creatures[i];
        CreatureQueue.creatures[i] = CreatureQueue.creatures[j];
        CreatureQueue.creatures[j] = temp;
    }
    #endregion
}