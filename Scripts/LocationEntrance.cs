using UnityEngine;

public class LocationEntrance : MonoBehaviour
{
    public GameObject are; // GameObject, který reprezentuje vstupní oblast
    public GameObject canvasObject; // Empty Object v Canvasu, který bude aktivován nebo deaktivován po stisknutí T
    public GameObject hintTextObject; // GameObject v Canvasu, který obsahuje text s instrukcí pro hráèe
    public BuildingMode buildingMode; // Reference na BuildingMode skript

    private bool playerInside = false; // Kontrola, jestli je hráè v oblasti
    [SerializeField] private MouseLook mouseLook;
  
    void Update()
    {
        // Pokud je hráè v oblasti a zmáèkne T
        if (playerInside && Input.GetKeyDown(KeyCode.T))
        {
            if (canvasObject != null)
            {
                if (canvasObject.activeSelf)
                {
                    // Zavøe Canvas a zamkne kurzor
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
                    // Otevøe Canvas a odemkne kurzor
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
        // Detekuje, jestli hráè vstoupil do oblasti
        if (other.CompareTag("Player"))
        {
            playerInside = true;

            if (hintTextObject != null)
            {
                hintTextObject.SetActive(true); // Aktivuje text s instrukcí
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        // Detekuje, jestli hráè opustil oblast
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
                hintTextObject.SetActive(false); // Deaktivuje text s instrukcí

            }
        }
    }
}
