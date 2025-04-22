using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldingObject : MonoBehaviour
{
    public GameObject ObjectHolder; // Prefab držáku
    private GameObject ActualHolder; // Aktuální držák
    public GameObject Camera; // Odkaz na kameru
    private GameObject AtractedObject; // Objekt, který má být p?itahován
    public float forceAmount = 10f; // Síla pro p?itahování objektu
    public float range = 10f; // Maximální vzdálenost pro raycast

  
    void Start()
    {
    }

    public void CreateHolder(Vector3 hitPoint, GameObject attracted)
    {
        // Instantiate the holder at the hit point
        ActualHolder = Instantiate(ObjectHolder, hitPoint, Quaternion.identity); // Using hitPoint for position

        ActualHolder.tag = "ObjectHolder";

        // Optionally set the parent, but keep its world position
        ActualHolder.transform.SetParent(gameObject.transform, false); // Set parent to keep local position

        ActualHolder.transform.position = hitPoint;
        if (ActualHolder.transform.localPosition.z<=2)
        {
            ActualHolder.transform.localPosition = new Vector3(ActualHolder.transform.localPosition.x, ActualHolder.transform.localPosition.y,2);
        }
      
        AtractedObject = attracted; // Store the attracted object
    }

    public void DeleteHolder()
    {
        if (ActualHolder != null)
        {
          
            Destroy(ActualHolder); // Zni? aktuální držák
           
            ActualHolder = null; // Nastavit ActualHolder na null
           
        }
        else
        {
           
        }
    }

    public void Atracting(GameObject hitted)
    {
        if (hitted != null)
        {
            Rigidbody rb = hitted.GetComponent<Rigidbody>();
            if (rb != null)
            {
                // P?itáhnout objekt sm?rem k držáku
                Vector3 direction = ActualHolder.transform.position - hitted.transform.position;
                rb.AddForce(direction.normalized * forceAmount, ForceMode.Impulse);
               
            }
            else
            {
              
            }
        }
        else
        {
           
        }
    }

    void Update()
    {
        // Pokud existuje držák a objekt k p?itahování
        if (ActualHolder != null && AtractedObject != null)
        {
            ActualHolder.transform.rotation = Camera.transform.rotation; // Nastavit rotaci držáku na rotaci kamery
            Atracting(AtractedObject); // P?itahování objektu
        }
    }
}
