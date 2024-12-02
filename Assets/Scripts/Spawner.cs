using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    // TODO:
    // * Handle bumpy terrain (Not a priority currently)

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

        if (timer > 4)
        {
            ran = Random.Range(0, 5000);
            if (currObjects >= 3 && ran > 4000 || currObjects < 3 && ran > 2000)
            {
                Instantiate(spawnee, randomLocation(), Quaternion.identity);
            }
            timer = 0;
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

    private Vector3 randomLocation()
    {
        float randomX = Random.Range(-gameObject.transform.localScale.x / 2, gameObject.transform.localScale.x / 2);
        float randomZ = Random.Range(-gameObject.transform.localScale.z / 2, gameObject.transform.localScale.z / 2);
        return new Vector3(randomX, -transform.position.y, randomZ) + transform.position;
    }
}
