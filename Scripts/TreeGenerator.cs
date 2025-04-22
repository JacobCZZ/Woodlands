
using Unity.VisualScripting;
using UnityEngine;

public class TrunkGenerator : MonoBehaviour
{
    public GameObject SegmentPrefab; // Prefab for the next trunk segment

    public int maxSegments = 5; // Maximum number of segments to generate
    public GameObject Leaves;
    public int BranchesAfterSegment =20;
    public float sizeDecreaseFactor = 0.9f; // Factor to reduce size of each subsequent segment
    Quaternion actualRotation = Quaternion.identity;
    private int currentSegmentCount = 0; // Track the number of segments generated
    private int currentSegmentCalculator=0;
    private Vector3 CurrentScale=new Vector3(1,1,1) ;
    float height ;
    public int MinSegmentCountScale=15;
    public int MaxSegmentCountScale=20;


    void Start()
    {
      
        if (currentSegmentCount==0)
        {
            CurrentScale = SegmentPrefab.transform.localScale;
        }
       
        GenerateSegments();
        Destroy(this);
    }

    void GenerateSegments()
    {
        if (SegmentPrefab is null)
        {
         
            return;
        }

        if (currentSegmentCount < maxSegments)
        {
            if (currentSegmentCount % BranchesAfterSegment == 0 && currentSegmentCount != 0)
            {
                // Declare variables to store the random offsets
                float randomXOffset = 0;
                float randomZOffset = 0;
                float x;
                float z;
                for (int i = 0; i < Random.Range(1,4); i++)
                {
                    // Generate random values only in the first iteration
                    if (i == 0)
                    {
                        Vector2 offsets = GetRandomOffsets();
                        randomXOffset = offsets.x;
                        randomZOffset = offsets.y;
                        // Adjust the rotations with the (potentially negated) random offsets
                        actualRotation.x = transform.rotation.x + randomXOffset; // Randomize x rotation
                        actualRotation.z = transform.rotation.z + randomZOffset; // Randomize z rotation
                    }
                    else if(i == 1)
                    {
                        // Negate the offsets for the second iteration
                        randomXOffset = -randomXOffset;
                        randomZOffset = -randomZOffset;
                        // Adjust the rotations with the (potentially negated) random offsets
                        actualRotation.x = transform.rotation.x + randomXOffset; // Randomize x rotation
                        actualRotation.z = transform.rotation.z + randomZOffset; // Randomize z rotation
                    }
                    else if (i == 2)
                    {
                        x = transform.rotation.x + randomXOffset;
                        z = transform.rotation.z + randomZOffset;
                        actualRotation.x = z;
                        actualRotation.z = x;

                        // Negate the offsets for the second iteration
                        randomXOffset = -randomXOffset;
                        
                    }else if (i == 3)
                    {
                          x = transform.rotation.x + randomXOffset;
                        z = transform.rotation.z + randomZOffset;
                        actualRotation.x = -z;
                        actualRotation.z = -x;
                    }
                 




                    // Calculate the next position
                   
                    Vector3 nextPosition =  new Vector3(0, 0, 0);

                    // Instantiate the next segment
                    GameObject nextSegment = Instantiate(SegmentPrefab, nextPosition, actualRotation);

                    // Update the next segment
                   
                    UpdateNextSegment(nextSegment);
                }

            }else if (currentSegmentCount>=maxSegments-1)
            {
                if (Leaves)
                {
                    Vector3 nextPosition = new Vector3(0, 0, 0);
                    GameObject leave = Instantiate(Leaves, nextPosition, actualRotation);
                    leave.transform.SetParent(transform, true); // Keeps global position and rotation
                    leave.transform.localPosition = Vector3.zero;
                }
              
            }
            else
            {
               
                Vector3 nextPosition =  new Vector3(0, 0, 0);
                GameObject nextSegment = Instantiate(SegmentPrefab, nextPosition, actualRotation);
                UpdateNextSegment(nextSegment);
            }
        }
    }

    private void UpdateNextSegment(GameObject nextSegment)
    {
        if (currentSegmentCalculator%Random.Range(MinSegmentCountScale,MaxSegmentCountScale)== 0&&currentSegmentCalculator !=0)
        {
            CurrentScale = new Vector3(CurrentScale.x * sizeDecreaseFactor, CurrentScale.y*sizeDecreaseFactor, CurrentScale.z * sizeDecreaseFactor);
            currentSegmentCalculator = 0;
        }
        //nextSegment.transform.localScale = CurrentScale; // Set the scale of the new segment
       
        nextSegment.transform.SetParent(transform, true); // Keeps global position and rotation
        nextSegment.transform.localPosition = Vector3.zero;


        // Increment segment count
        currentSegmentCount++;
        currentSegmentCalculator++;

        // Recursively generate the next segment
        nextSegment.AddComponent<TrunkGenerator>();
        TrunkGenerator NextSG = nextSegment.GetComponent<TrunkGenerator>();
        //NextSG.CurrentScale = CurrentScale;
        NextSG.SegmentPrefab = SegmentPrefab;
        NextSG.maxSegments = maxSegments;
        NextSG.actualRotation = actualRotation;
        NextSG.currentSegmentCount = currentSegmentCount;
        NextSG.currentSegmentCalculator = currentSegmentCalculator;
        NextSG.MaxSegmentCountScale = MaxSegmentCountScale;
        NextSG.MinSegmentCountScale = MinSegmentCountScale;
        NextSG.BranchesAfterSegment = BranchesAfterSegment;
        NextSG.Leaves = Leaves;
        



        nextSegment.transform.localScale = CurrentScale;
        float kokot = GetHeight(nextSegment);
      


     
        nextSegment.transform.localPosition = new Vector3(0, 0, 0);
        nextSegment.transform.localPosition += new Vector3(0, kokot, 0); 
        

    }
    public float GetHeight(GameObject NextSegment)
    {
        // Get the BoxCollider component
        BoxCollider boxCollider = NextSegment.GetComponent<BoxCollider>();
        if (boxCollider == null)
        {
        
            return 0f;
        }

        // Get the height from the size of the BoxCollider
        float height = boxCollider.size.y * NextSegment.transform.localScale.y;

     
        return height;
    }
    Vector2 GetRandomOffsets()
    {
        float randomXOffset;
        float randomZOffset;

        // Generate random X offset
        if (Random.value < 0.2f)
        {
            randomXOffset = Random.Range(-0.5f, -0.2f);
        }
        else
        {
            randomXOffset = Random.Range(0.2f, 0.5f);
        }

        // Generate random Z offset
        if (Random.value < 0.2f)
        {
            randomZOffset = Random.Range(-0.5f, -0.2f);
        }
        else
        {
            randomZOffset = Random.Range(0.2f, 0.5f);
        }

        return new Vector2(randomXOffset, randomZOffset);
    }




}
