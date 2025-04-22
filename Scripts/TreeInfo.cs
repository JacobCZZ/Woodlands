using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeInfo : MonoBehaviour
{
    public float basePricePerUnit = 6f; // Adjust base price per unit of scale
    public float totalPrice;
    public GameObject UpgradePrefab;

    void Start()
    {
        totalPrice = CalculateTreePrice(transform);
    
    }

    public float CalculateTreePrice(Transform segment)
    {
        float segmentPrice = 0f;

        // Calculate price for the current segment
       
        segmentPrice += basePricePerUnit * segment.lossyScale.x ;

        // Recursively calculate the price of all child segments
        foreach (Transform child in segment)
        {
            segmentPrice += CalculateTreePrice(child);
        }

        return segmentPrice;
    }

}
