using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    // TODO:
    // * Make the instantiated objects obey the laws of physics
    // * Make them spawn randomly through the box and with a random rotations

    [SerializeField] private float timer;
    [SerializeField] private GameObject spawnee;
    [SerializeField] private Collider[] collisions = new Collider[10];
    [SerializeField] private int currObjects = 0;
    [SerializeField] private int ran = 0;

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(transform.position, transform.localScale);
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer > 4 && currObjects >= 3)
        {
            ran = Random.Range(0, 5000);
            if(ran > 4000)
            {
                Instantiate(spawnee, gameObject.transform.position, Quaternion.identity);
                timer = 0;
            }
            else timer = 0;
        }
        else if (timer > 4 && currObjects < 3)
        {
            ran = Random.Range(0, 5000);
            if (ran > 2000)
            {
                Instantiate(spawnee, gameObject.transform.position, Quaternion.identity);
                timer = 0;
            }
            else timer = 0;
        }
    }

    void LateUpdate()
    {
        CubeCast();
    }

    private void CubeCast()
    {
        currObjects = Physics.OverlapBoxNonAlloc(gameObject.transform.position, transform.localScale / 2, collisions, Quaternion.identity, 1);
    }
}
