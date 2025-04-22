using UnityEngine;

public class ConveyorBelt : MonoBehaviour
{
    public float speed = 2f; // Speed of the conveyor belt
    Transform root;
    private void OnTriggerEnter(Collider other)
    {
       
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Tree"))
        {
            // Start at the current object and move up through parents
            root = other.transform;
            while (root.parent != null)
            {
                root = root.parent;
            }

        }
        if (root !=null)
        {
            root.transform.position += transform.forward * speed * Time.deltaTime;
        }
         
          
       
    }
}
