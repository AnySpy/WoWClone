/*
    A custom player controller based on the First Person Controller asset.
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using Unity.VisualScripting;
using UnityEditor.Callbacks;
using UnityEngine;

// TODO: Make the camera not go through terrain.

public class PlayerController : MonoBehaviour
{
    #region Camera Settings
    [Header("Camera Settings")]
    [SerializeField] private Camera playerCamera;
    [Range(0f, 5f)]
    [SerializeField] private float  scrollSensitivity       = 2f;
    [Range(0f, 5f)]
    [SerializeField] private float  mouseSensitivity        = 2f;
    [Range(0f, 30f)]
    [SerializeField] private float  maxDistance             = 20f;
    [Range(50f, 120f)]
    [SerializeField] private float  fov                     = 90f;
    private KeyCode                 lmb                     = KeyCode.Mouse0;
    private KeyCode                 rmb                     = KeyCode.Mouse1;
    private float                   yaw                     = 0.0f;
    private float                   pitch                   = 0.0f;
    private float                   distance                = 5.0f;
    private float                   maxLookAngle            = 89f;
    #endregion

    #region Movement Settings
    [Header("Movement Settings")]
    [Range(0f, 10f)]
    [SerializeField] private float  walkSpeed               = 5f;
    [Range(0f, 10f)]
    [SerializeField] private float  jumpPower               = 5f;
    private KeyCode                 jumpKey                 = KeyCode.Space;
    private bool                    isGrounded              = false;
    private bool                    playerCanMove           = true;
    private float                   maxVelocityChange       = 10f;
    private Rigidbody               rb;
    #endregion

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        playerCamera.fieldOfView = fov;
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;

        distance = Vector3.Distance(playerCamera.transform.position, transform.position);

        Vector3 offset = playerCamera.transform.position - transform.position;
        pitch = Mathf.Asin(offset.y / distance) * Mathf.Rad2Deg;
        yaw = Mathf.Atan2(offset.x, offset.z) * Mathf.Rad2Deg + 180f;
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
            distance -= Input.GetAxis("Mouse ScrollWheel") * scrollSensitivity;
            distance = Mathf.Clamp(distance, 0, maxDistance);

            Vector3 direction = new Vector3(0, 0, -distance);
            Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);

            playerCamera.transform.position = transform.position + rotation * direction;
            playerCamera.transform.LookAt(transform.position);
        }

        CheckGround();

        if (Input.GetKeyDown(jumpKey))
        {
            Jump();
        }
    }

    void FixedUpdate()
    {
        if (playerCanMove)
        {
            Vector3 targetVelocity = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

            targetVelocity = transform.TransformDirection(targetVelocity) * walkSpeed;

            Vector3 velocity = rb.velocity;
            Vector3 velocityChange = targetVelocity - velocity;
            velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
            velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
            velocityChange.y = 0;

            rb.AddForce(velocityChange, ForceMode.VelocityChange);
        }
    }

    private void CheckGround()
    {
        Vector3 origin = new Vector3(transform.position.x, transform.position.y - (transform.localScale.y * 0.5f), transform.position.z);
        Vector3 direction = transform.TransformDirection(Vector3.down);
        float distance = 0.75f;

        if (Physics.Raycast(origin, direction, out RaycastHit hit, distance))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }

    private void Jump()
    {
        if (isGrounded)
        {
            rb.AddForce(0f, jumpPower, 0f, ForceMode.Impulse);
            isGrounded = false;
        }
    }
}
