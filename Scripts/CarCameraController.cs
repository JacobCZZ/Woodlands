using UnityEngine;

public class CarCameraController : MonoBehaviour
{
    public float rotationSpeed = 50f;      // Rychlost otáčení kamery
    public float maxRotationAngle = 70f;  // Maximální úhel natočení kamery (±)

    private float currentRotation = 0f;   // Aktuální natočení kamery

    void Update()
    {
        // Získání vstupu z myši
        float horizontalInput = Input.GetAxis("Mouse X");

        // Výpočet nové rotace
        currentRotation += horizontalInput * rotationSpeed * Time.deltaTime;

        // Omezení úhlu rotace
        currentRotation = Mathf.Clamp(currentRotation, -maxRotationAngle, maxRotationAngle);

        // Nastavení rotace kamery
        transform.localRotation = Quaternion.Euler(0, currentRotation, 0);
    }
}
