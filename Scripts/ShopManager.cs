using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopManager : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject shopUI;
    public TMP_Text playerMoneyText;
    public TMP_Text shopHintText; // Hint text for "Press P to Shop"
    public GameObject inventoryUI; // Reference to Inventory UI
    public float playerMoney;

    [Header("Shop Items")]
    public ShopItem[] shopItems;

    private bool isShopOpen = false;
    private bool isInShopArea = false;
    private InventoryManager inventoryManager;
    [SerializeField] private MouseLook mouseLook;

    void Start()
    {
        shopUI.SetActive(false);
        shopHintText.gameObject.SetActive(false); // Hint text starts hidden
        UpdateMoneyUI();

        inventoryManager = GetComponent<InventoryManager>();

        if (inventoryManager == null)
        {
            Debug.LogError("InventoryManager not found! Ensure the player has the InventoryManager component.");
        }

        foreach (var item in shopItems)
        {
            if (item.buyButton != null)
            {
                item.buyButton.onClick.AddListener(() => BuyItem(item));
            }

            if (item.priceText != null)
            {
                item.priceText.text = $"Price: {item.price}";
            }
        }
    }

    void Update()
    {
        if (isInShopArea && Input.GetKeyDown(KeyCode.P))
        {
            ToggleShop();
        }

       
    }

    public void ToggleShop()
    {
        if (isShopOpen)
        {
            CloseShop();
        }
        else
        {

            if (inventoryUI != null && inventoryUI.activeSelf)
            {
                inventoryManager.ToggleInventory(); // Close inventory if it's open
            }
           
            isShopOpen = true;
            shopUI.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        if (mouseLook != null)
        {
            mouseLook.enabled = !isShopOpen; // Aktivace/deaktivace MouseLook
        }
    }

    public void CloseShop()
    {
        isShopOpen = false;
        shopUI.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

  

    public void BuyItem(ShopItem item)
    {
        if (playerMoney >= item.price && !item.isBought)
        {
            playerMoney -= item.price;

            if (item.isAxe)
            {
                UnlockAxe(item.index);
            }
            else if (item.isVehicle)
            {
                UnlockVehicle(item.index);
            }

            item.boughtPanel.SetActive(true);
            item.buyButton.interactable = false;
            item.isBought = true;

            UpdateMoneyUI();
            Debug.Log($"Item {item.itemName} purchased and unlocked with index {item.index}!");
        }
        else
        {
            Debug.Log("Not enough money or item already purchased.");
        }
    }

    public void UpdateMoneyUI()
    {
        playerMoneyText.text = $"{playerMoney}";
    }

    private void UnlockAxe(int index)
    {
        if (inventoryManager != null)
        {
            Debug.Log($"Unlocking axe with index {index}.");
            inventoryManager.UnlockAxe(index);
        }
        else
        {
            Debug.LogError("InventoryManager is not attached to the player!");
        }
    }

    private void UnlockVehicle(int index)
    {
        if (inventoryManager != null)
        {
            Debug.Log($"Unlocking vehicle with index {index}.");
            inventoryManager.UnlockVehicle(index);
        }
        else
        {
            Debug.LogError("InventoryManager is not attached to the player!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Shop"))
        {
            isInShopArea = true;
            shopHintText.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Shop"))
        {
            isInShopArea = false;
            shopHintText.gameObject.SetActive(false);

            if (isShopOpen)
            {
                ToggleShop(); // Close the shop when leaving the area
            }
        }
    }
}

[System.Serializable]
public class ShopItem
{
    public string itemName;
    public Button buyButton;
    public GameObject boughtPanel;
    public TMP_Text priceText; // Reference to TextMeshPro for displaying price
    public int price;
    public bool isBought = false;
    public bool isAxe;
    public bool isVehicle;
    public int index; // Index of the item for unlocking
}
