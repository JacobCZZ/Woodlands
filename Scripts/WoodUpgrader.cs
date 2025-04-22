using UnityEngine;

public class WoodUpgrader : MonoBehaviour
{
     GameObject plankSegmentPrefab; // Prefab for individual plank segments
  float maxSegmentCount;
   
    float currentSegmentCount;
    Transform currentParentSegment;
    Transform root;

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object or its parent is tagged "Tree"
        if (other.CompareTag("Tree"))
        {
            // Start at the current object and move up through parents
            root = other.transform;
            while (root.parent != null)
            {
                root = root.parent;
            }
          
            // Check if the root object has the TreeInfo script
            TreeInfo treeInfo = root.GetComponent<TreeInfo>();
            if (treeInfo != null)
            {
                if (treeInfo.UpgradePrefab!= null)
                {
                    plankSegmentPrefab = treeInfo.UpgradePrefab;
                    maxSegmentCount = Mathf.Round(treeInfo.totalPrice / 2);
                    GenerateSegments();
                    
                    Destroy(root.gameObject);
                }
                
            }
        }
    }

    void GenerateSegments()
    {
        // Check if prefab and parent segment are set
       
        if (plankSegmentPrefab == null) return;
        // Create an initial empty parent for the planks
        Transform upperPart = new GameObject("UpperPart").transform;
       
      
        upperPart.position = root.position;
        upperPart.tag = "Holdable";
        Rigidbody upperRb = upperPart.gameObject.AddComponent<Rigidbody>();
        
        currentParentSegment = upperPart;
   
        // Generate the segments
        for (currentSegmentCount = 0; currentSegmentCount < maxSegmentCount; currentSegmentCount++)
        {
            // Instantiate the next segment with an offset based on its height
            GameObject nextSegment = Instantiate(plankSegmentPrefab, currentParentSegment.position, Quaternion.identity);
            nextSegment.transform.parent = currentParentSegment;

            // Calculate offset based on the segment's height
            BoxCollider collider = nextSegment.GetComponent<BoxCollider>();
            float segmentHeight = collider.size.y;
            nextSegment.transform.localPosition = new Vector3(0,1 ,0);
            if (currentSegmentCount==0)
            {
                nextSegment.transform.localPosition = new Vector3(0, 0, 0);
              
            }

            // Store the current position of the segment
         


            // Update the current parent to the newly created segment
            currentParentSegment = nextSegment.transform;
         
        }
        upperPart.transform.Rotate(0, -90, -90);


        upperRb.mass = Mathf.Max(maxSegmentCount * 2.5f, 60f);
        upperRb.angularDamping = 2f;
        upperRb.linearDamping = 3f;
        upperRb.useGravity = true;
        TreeInfo fakt = upperPart.gameObject.AddComponent<TreeInfo>();
        fakt.basePricePerUnit = 30;
        fakt.totalPrice = fakt.totalPrice * 5;
    }
}
