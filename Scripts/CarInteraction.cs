using UnityEngine;
using TMPro;

public class CarInteraction : MonoBehaviour
{
    public Transform driverSeat;        // Pozice sedadla řidiče
    public GameObject car;              // GameObject auta (hlavní objekt)
    public GameObject player;           // Hráčův GameObject
    public CarController carController; // Skript pro ovládání auta
    public GameObject CarCam;

    private bool isPlayerNear = false;  // Kontrola, zda je hráč poblíž auta
    private bool isPlayerInCar = false; // Kontrola, zda je hráč v autě
    public Rigidbody carRigidbody;     // Rigidbody auta pro zastavení pohybu
    public TMP_Text textGetin;
    void Start()
    {
        // Získání Rigidbody auta
        carRigidbody = car.GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (isPlayerNear && Input.GetKeyDown(KeyCode.F)) // Stisknutí E
        {
            if (isPlayerInCar)
            {
                ExitCar();
            }
            else
            {
                EnterCar();
            }
        }
    }

    private void EnterCar()
    {
        player.SetActive(false);

        player.transform.position = driverSeat.position; // Přesun na sedadlo
        player.transform.parent = car.transform;

        carController.enabled = true; // Aktivace ovládání auta
        if (carRigidbody != null)
        {
         
            carRigidbody.isKinematic = false;
        }
        isPlayerInCar = true;
        CarCam.SetActive(true);
    }

    private void ExitCar()
    {
        player.SetActive(true);
        player.transform.parent = null;

        carController.enabled = false; // Deaktivace ovládání auta
        CarCam.SetActive(false);

        // Zastavení auta
        if (carRigidbody != null)
        {
            carRigidbody.linearVelocity = Vector3.zero;        // Nulování rychlosti
            carRigidbody.angularVelocity = Vector3.zero; // Nulování rotace
            carRigidbody.isKinematic = true;
        }

        isPlayerInCar = false;
        player.transform.rotation = Quaternion.identity;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = true;
            textGetin.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = false;
            textGetin.gameObject.SetActive(false);
        }
    }
}
