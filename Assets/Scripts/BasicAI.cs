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

    [SerializeField] private float timer = 0;
    [SerializeField] private int rotation = 0;
    [SerializeField] private float movement = 0;

    [SerializeField] private int movementSpeed;

    private bool isMoving = false;
    private bool rotated = false;

    void Start()
    {
        creature = gameObject;
    }
    
    void Update()
    {
        if (!isMoving)
        {
            timer += Time.deltaTime;
        }

        if (timer >= 99999999)
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
            movement = Random.Range(1, 50);

            transform.Rotate(0, rotation, 0);
            rotated = true;
        }

        if (movement > 0)
        {
            float step = movementSpeed * Time.deltaTime;
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
