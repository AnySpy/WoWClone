/*
    A custom player controller based on the First Person Controller asset.
*/

using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private KeyCode lmb = KeyCode.Mouse0;
    private KeyCode rmb = KeyCode.Mouse1;
    public Camera playerCamera;
    public float fov = 60f;
    public float mouseSensitivity = 2f;
    public float scrollSensitivity= 2f;
    public float maxLookAngle = 90f;
    public bool lockCursor = false;
    private float yaw = 0.0f;
    private float distance = 5.0f;
    private float pitch = 0.0f;

    void Awake()
    {
        playerCamera.fieldOfView = fov;
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;

        Vector3 offset = playerCamera.transform.position - transform.position;
        distance = offset.magnitude;
        Quaternion initialRotation = Quaternion.LookRotation(offset.normalized);

        yaw = initialRotation.eulerAngles.y;
        pitch = initialRotation.eulerAngles.x;
    }

    void Update()
    {
        if (Input.GetKey(lmb) || Input.GetKey(rmb))
        {
            Cursor.lockState = CursorLockMode.Locked;

            yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
            pitch += Input.GetAxis("Mouse Y") * mouseSensitivity;

            pitch = Mathf.Clamp(pitch, -maxLookAngle, maxLookAngle);

            Vector3 direction = new Vector3(0, 0, -distance);
            Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);

            if (Input.GetKey(rmb))
            {
                transform.rotation = Quaternion.Euler(new Vector3(0, yaw, 0));
            }

            playerCamera.transform.position = transform.position + rotation * direction;
            playerCamera.transform.LookAt(transform.position);
        }
        else Cursor.lockState = CursorLockMode.Confined;

        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            distance += Input.GetAxis("Mouse ScrollWheel") * scrollSensitivity;
            
            if (distance >= 0) distance = 0;

            Vector3 direction = new Vector3(0, 0, -distance);
            Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);

            playerCamera.transform.position = transform.position + rotation * direction;
            playerCamera.transform.LookAt(transform.position);
        }
    }
}
