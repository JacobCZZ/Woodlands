    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public float mouseSensitivity = 100f;  // Citlivost my�i
    public Transform playerBody;           // Odkaz na t?lo hr�?e (bude rotovat horizont�ln?)

    float xRotation = 0f;  // Uchov�n� vertik�ln� rotace
    public Camera mainCamera;
    public float renderDistance = 100f;

    void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        //mainCamera.farClipPlane = renderDistance;
        // Uzam?en� kurzoru do st?edu obrazovky
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // Z�sk�n� vstupu od my�i
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Ot�?en� hr�?e horizont�ln? (okolo osy Y)
        playerBody.Rotate(Vector3.up * mouseX);

        // Ot�?en� kamery vertik�ln? (okolo osy X)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);  // Omezen� vertik�ln�ho pohybu kamery

        // Aplikace rotace kamery
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }
}
