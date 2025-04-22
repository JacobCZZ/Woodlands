using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class SoldingPlace : MonoBehaviour
{
    public float playerMoney = 0f; // Adjust player money if needed
    public TextMeshProUGUI moneyText;
    public ShopManager shopManager;
    private void OnTriggerEnter(Collider other)
    {
        // Check if the object or its parent is tagged "Tree"
        if (other.CompareTag("Tree"))
        {
            // Start at the current object and move up through parents
            Transform root = other.transform;
            while (root.parent != null)
            {
                root = root.parent;
            }

            // Check if the root object has the TreeInfo script
            TreeInfo treeInfo = root.GetComponent<TreeInfo>();
            if (treeInfo != null)
            {
                // Retrieve the total price from the root TreeInfo
                float treePrice = treeInfo.totalPrice;

                // Add the price to the player's money

                playerMoney +=Mathf.Round( treePrice);
                Debug.Log("pred součtem");
                float ben = playerMoney + shopManager.playerMoney;
                Debug.Log("po součtu");
                
               
                shopManager.playerMoney = ben;
                shopManager.UpdateMoneyUI();
                // Destroy the root tree object
                Destroy(root.gameObject);

                
            }
        }
    }
}
