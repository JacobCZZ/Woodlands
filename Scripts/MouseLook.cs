    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public float mouseSensitivity = 100f;  // Citlivost myši
    public Transform playerBody;           // Odkaz na t?lo hrá?e (bude rotovat horizontáln?)

    float xRotation = 0f;  // Uchování vertikální rotace
    public Camera mainCamera;
    public float renderDistance = 100f;

    void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        //mainCamera.farClipPlane = renderDistance;
        // Uzam?ení kurzoru do st?edu obrazovky
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // Získání vstupu od myši
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Otá?ení hrá?e horizontáln? (okolo osy Y)
        playerBody.Rotate(Vector3.up * mouseX);

        // Otá?ení kamery vertikáln? (okolo osy X)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);  // Omezení vertikálního pohybu kamery

        // Aplikace rotace kamery
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }
}
