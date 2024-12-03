using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

using Random = UnityEngine.Random;

public class BasicAI : MonoBehaviour
{
    [SerializeField] private GameObject creature;
    [SerializeField] public GameObject spawner;

    [SerializeField] private float timer = 0;
    [SerializeField] private int rotation = 0;
    [SerializeField] private float movement = 0;
    [SerializeField] private float xLeash = 0;
    [SerializeField] private float zLeash = 0;

    [SerializeField] private int movementSpeed;

    private bool isMoving = false;
    private bool rotated = false;

    // TODO:
    // ! Fix them falling over and flying everywhere lmfao
    // * Make movement rotate around the y-axis before rotating.

    void Start()
    {
        creature = gameObject;
        xLeash = spawner.transform.localScale.x / 2;
        zLeash = spawner.transform.localScale.z / 2;
    }
    
    void Update()
    {
        if (!isMoving)
        {
            timer += Time.deltaTime;
        }

        if (timer >= 4)
        {
            isMoving = true;
            Movement();
        }
    }

    /// <summary>
    /// Rotates an object and then "steps" them forward each frame.
    /// </summary>
    void Movement()
    {
        if (!rotated) 
        {
            rotation = Random.Range(0, 360);
            movement = Random.Range(1, 25);

            transform.Rotate(0, rotation, 0);
            rotated = true;
        }

        if (movement > 0)
        {
            float step = movementSpeed * Time.deltaTime;
            Vector3 newPos = transform.position + transform.forward * step;
            if (newPos.x > spawner.transform.position.x + xLeash || newPos.x < spawner.transform.position.x - xLeash) transform.Rotate(0, transform.rotation.y + Random.Range(91, 270), 0);
            if (newPos.z > spawner.transform.position.z + zLeash || newPos.z < spawner.transform.position.z - zLeash) transform.Rotate(0, transform.rotation.y + Random.Range(91, 270), 0);
            if (step > movement) step = movement;

            transform.Translate(Vector3.forward * step);
            movement -= step;
        }
        else
        {
            isMoving = false;
            rotated = false;
            timer = 0;
        }
    }
}
