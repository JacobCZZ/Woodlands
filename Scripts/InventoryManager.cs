using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    [Header("Inventory UI")]
    public GameObject inventoryUI;

    [Header("Axe Management")]
    public List<AxeData> axes = new List<AxeData>();

    [Header("Vehicle Management")]
    public List<VehicleData> vehicles = new List<VehicleData>();

    private AxeData currentlyEquippedAxe = null;
    private bool isInventoryOpen = false;
    public PlayerInteraction playerInteraction;
    public ShopManager shopManager;

    [SerializeField] private MouseLook mouseLook; // Přidána reference na MouseLook

    void Start()
    {
        playerInteraction = gameObject.GetComponent<PlayerInteraction>();

        // Setup axes
        foreach (var axe in axes)
        {
            axe.equipButton.onClick.AddListener(() => EquipAxe(axe));
            axe.unequipButton.onClick.AddListener(() => UnequipAxe(axe));
            UpdateAxeUI(axe, false);
            if (axe.axeObject != null)
            {
                axe.axeObject.SetActive(false);
                var axeScript = axe.axeObject.GetComponent<Axe>();
                if (axeScript != null)
                {
                    axeScript.IsActual = false;
                }
            }
        }

        // Setup vehicles
        foreach (var vehicle in vehicles)
        {
            vehicle.spawnButton.onClick.AddListener(() => SpawnVehicle(vehicle));
            vehicle.despawnButton.onClick.AddListener(() => DespawnVehicle(vehicle));
            UpdateVehicleUI(vehicle, false);

            // Make sure all vehicles are initially inactive and locked
            if (vehicle.vehicleObject != null)
            {
                vehicle.vehicleObject.SetActive(false);
            }

            if (vehicle.lockedPanel != null)
            {
                vehicle.lockedPanel.SetActive(true); // Initially locked
            }
        }

        inventoryUI.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            ToggleInventory();
        }
    }

    public void UnlockAxe(int index)
    {
        if (index >= 0 && index < axes.Count)
        {
            axes[index].lockedPanel.SetActive(false);
            axes[index].equipButton.gameObject.SetActive(true);
            Debug.Log($"Axe at index {index} unlocked!");
        }
    }

    public void UnlockVehicle(int index)
    {
        if (index >= 0 && index < vehicles.Count)
        {
            if (vehicles[index].lockedPanel != null)
            {
                vehicles[index].lockedPanel.SetActive(false); // Hide the locked panel
            }

            if (vehicles[index].spawnButton != null)
            {
                vehicles[index].spawnButton.gameObject.SetActive(true); // Show the spawn button
            }
            Debug.Log($"Vehicle at index {index} unlocked!");
        }
    }

    private void EquipAxe(AxeData axe)
    {
        if (currentlyEquippedAxe != null)
        {
            UnequipAxe(currentlyEquippedAxe);
        }

        currentlyEquippedAxe = axe;

        foreach (var inventoryAxe in axes)
        {
            if (inventoryAxe.axeObject != null)
            {
                inventoryAxe.axeObject.SetActive(false);
                var axeScript = inventoryAxe.axeObject.GetComponent<Axe>();
                if (axeScript != null)
                {
                    axeScript.IsActual = false;
                }
            }
        }

        if (axe.axeObject != null)
        {
            var axeScript = axe.axeObject.GetComponent<Axe>();
            if (axeScript != null)
            {
                axeScript.IsActual = true;
            }
        }

        UpdateAxeUI(axe, true);
    }

    private void UnequipAxe(AxeData axe)
    {
        if (currentlyEquippedAxe == axe)
        {
            currentlyEquippedAxe = null;
        }

        if (axe.axeObject != null)
        {
            axe.axeObject.SetActive(false);
            var axeScript = axe.axeObject.GetComponent<Axe>();
            if (axeScript != null)
            {
                axeScript.IsActual = false;
            }
        }

        if (playerInteraction != null)
            playerInteraction.ActualAxe = null;

        UpdateAxeUI(axe, false);
    }

    private void SpawnVehicle(VehicleData vehicle)
    {
        if (vehicle.vehicleObject != null)
        {
            if (vehicle.spawnPoint != null)
            {
                vehicle.vehicleObject.transform.position = vehicle.spawnPoint.position;
                vehicle.vehicleObject.transform.rotation = vehicle.spawnPoint.rotation;
            }

            vehicle.vehicleObject.SetActive(true);
            UpdateVehicleUI(vehicle, true);
        }
    }

    private void DespawnVehicle(VehicleData vehicle)
    {
        if (vehicle.vehicleObject != null)
        {
            vehicle.vehicleObject.SetActive(false);
            UpdateVehicleUI(vehicle, false);
        }
    }

    private void UpdateVehicleUI(VehicleData vehicle, bool isSpawned)
    {
        vehicle.spawnButton.gameObject.SetActive(!isSpawned && !vehicle.lockedPanel.activeSelf);
        vehicle.despawnButton.gameObject.SetActive(isSpawned);
    }

    private void UpdateAxeUI(AxeData axe, bool isEquipped)
    {
        axe.equipButton.gameObject.SetActive(!isEquipped && !axe.lockedPanel.activeSelf);
        axe.unequipButton.gameObject.SetActive(isEquipped);
    }

    public void ToggleInventory()
    {
        bool isInventoryOpen = !inventoryUI.activeSelf;
        inventoryUI.SetActive(isInventoryOpen);
        shopManager.CloseShop();

        if (mouseLook != null)
        {
            mouseLook.enabled = !isInventoryOpen; // Aktivace/deaktivace MouseLook
        }

        if (isInventoryOpen)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}

[System.Serializable]
public class AxeData
{
    public Button equipButton;
    public Button unequipButton;
    public GameObject lockedPanel;
    public GameObject axeObject;
}

[System.Serializable]
public class VehicleData
{
    public Button spawnButton;
    public Button despawnButton;
    public GameObject lockedPanel;
    public GameObject vehicleObject;
    public Transform spawnPoint;
}
