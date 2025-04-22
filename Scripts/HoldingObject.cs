using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldingObject : MonoBehaviour
{
    public GameObject ObjectHolder; // Prefab dr��ku
    private GameObject ActualHolder; // Aktu�ln� dr��k
    public GameObject Camera; // Odkaz na kameru
    private GameObject AtractedObject; // Objekt, kter� m� b�t p?itahov�n
    public float forceAmount = 10f; // S�la pro p?itahov�n� objektu
    public float range = 10f; // Maxim�ln� vzd�lenost pro raycast

  
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
          
            Destroy(ActualHolder); // Zni? aktu�ln� dr��k
           
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
                // P?it�hnout objekt sm?rem k dr��ku
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
        // Pokud existuje dr��k a objekt k p?itahov�n�
        if (ActualHolder != null && AtractedObject != null)
        {
            ActualHolder.transform.rotation = Camera.transform.rotation; // Nastavit rotaci dr��ku na rotaci kamery
            Atracting(AtractedObject); // P?itahov�n� objektu
        }
    }
}
