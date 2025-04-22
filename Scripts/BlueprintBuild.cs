using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BlueprintBuild : MonoBehaviour
{
    [SerializeField] public int NeededPriceWood = 100;
    [SerializeField] private GameObject Cube;
    [SerializeField] private TextMeshProUGUI progressText;

    private Transform playerTransform;
    private float currentWoodValue = 0f;
    private List<GameObject> woodInside = new List<GameObject>();

    private void Start()
    {
        // Zkus najít hráče podle tagu "Player"
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }
     
    }

    private void Update()
    {
        RotateProgressText();
    }

    private void RotateProgressText()
    {
        if (progressText != null && playerTransform != null)
        {
            progressText.transform.LookAt(playerTransform);
            progressText.transform.Rotate(0, 180, 0);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Tree"))
        {
            Transform root = other.transform;
            while (root.parent != null)
            {
                root = root.parent;
            }

            TreeInfo treeInfo = root.GetComponent<TreeInfo>();
            if (treeInfo != null && !woodInside.Contains(root.gameObject))
            {
                float treePrice = Mathf.Round(treeInfo.totalPrice);
                currentWoodValue += treePrice;
                woodInside.Add(root.gameObject);
                UpdateProgress();
                TryBuild();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Tree"))
        {
            Transform root = other.transform;
            while (root.parent != null)
            {
                root = root.parent;
            }

            TreeInfo treeInfo = root.GetComponent<TreeInfo>();
            if (treeInfo != null && woodInside.Contains(root.gameObject))
            {
                float treePrice = Mathf.Round(treeInfo.totalPrice);
                currentWoodValue -= treePrice;
                woodInside.Remove(root.gameObject);
                UpdateProgress();
            }
        }
    }

    private void UpdateProgress()
    {
        float progress = Mathf.Clamp01(currentWoodValue / NeededPriceWood) * 100f;
        if (progressText != null)
        {
            progressText.text = Mathf.RoundToInt(progress).ToString() + "%";
        }
    }

    private void TryBuild()
    {
        if (currentWoodValue >= NeededPriceWood)
        {
            Instantiate(Cube, transform.position, transform.rotation);

            foreach (GameObject wood in woodInside)
            {
                if (wood != null)
                {
                    Destroy(wood);
                }
            }

            Destroy(gameObject);
        }
    }
}
