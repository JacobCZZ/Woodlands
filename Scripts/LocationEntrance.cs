using UnityEngine;

public class LocationEntrance : MonoBehaviour
{
    public GameObject are; // GameObject, kter� reprezentuje vstupn� oblast
    public GameObject canvasObject; // Empty Object v Canvasu, kter� bude aktivov�n nebo deaktivov�n po stisknut� T
    public GameObject hintTextObject; // GameObject v Canvasu, kter� obsahuje text s instrukc� pro hr��e
    public BuildingMode buildingMode; // Reference na BuildingMode skript

    private bool playerInside = false; // Kontrola, jestli je hr�� v oblasti
    [SerializeField] private MouseLook mouseLook;
  
    void Update()
    {
        // Pokud je hr�� v oblasti a zm��kne T
        if (playerInside && Input.GetKeyDown(KeyCode.T))
        {
            if (canvasObject != null)
            {
                if (canvasObject.activeSelf)
                {
                    // Zav�e Canvas a zamkne kurzor
                    canvasObject.SetActive(false);
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                    if (mouseLook != null)
                    {
                        mouseLook.enabled = true; // Aktivace/deaktivace MouseLook
                    }

                }
                else
                {
                    // Otev�e Canvas a odemkne kurzor
                    canvasObject.SetActive(true);
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                    if (mouseLook != null)
                    {
                        mouseLook.enabled = false; // Aktivace/deaktivace MouseLook
                    }
                }
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        buildingMode.isOut = false;
        // Detekuje, jestli hr�� vstoupil do oblasti
        if (other.CompareTag("Player"))
        {
            playerInside = true;

            if (hintTextObject != null)
            {
                hintTextObject.SetActive(true); // Aktivuje text s instrukc�
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        // Detekuje, jestli hr�� opustil oblast
        if (other.CompareTag("Player"))
        {
            playerInside = false;

            if (canvasObject != null)
            {
                canvasObject.SetActive(false); // Deaktivuje Canvas objekt
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false; // Skryje kurzor
            }
            if (mouseLook != null)
            {
                mouseLook.enabled = true; // Aktivace/deaktivace MouseLook
                buildingMode.isOut = true;
            }
            if (hintTextObject != null)
            {
                hintTextObject.SetActive(false); // Deaktivuje text s instrukc�

            }
        }
    }
}
