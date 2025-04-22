using UnityEngine;

public class TreeSegment : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;
    private Rigidbody rb;
    private Vector3[] originalVertices;
    public GameObject UpgradeTreePrefab;
    public Transform RootParent;

    void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody>();

        Mesh mesh = GetComponent<MeshFilter>().mesh;
        mesh = Instantiate(mesh); // Copy the mesh to avoid modifying the shared asset
        GetComponent<MeshFilter>().mesh = mesh;

        // Store the original vertices for resizing the mesh later
        originalVertices = mesh.vertices;
    }

    public void DeleteEmptys()
    {
        GameObject[] holdableObjects = GameObject.FindGameObjectsWithTag("Holdable");

        foreach (GameObject obj in holdableObjects)
        {
            // Destroy the object if it has no children
            if (obj.transform.childCount == 0)
            {
                Destroy(obj);
            }
        }
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        UpdateSize(currentHealth, maxHealth);
        ChangeFirstMaterialToMatchSecond();

        if (currentHealth <= 0)
        {
            UpdateParentMass();
            DetachSegment();
            UpdateParentMass();
            DetachSegment();
            UpdateParentMass();
            DetachSegment();
            DeleteEmptys();
            Destroy(gameObject);
        }
    }

    public void ChangeFirstMaterialToMatchSecond()
    {
        Renderer renderer = GetComponent<Renderer>();

        if (renderer.materials.Length >= 2)
        {
            Material[] materials = renderer.materials;
            materials[0].CopyPropertiesFromMaterial(materials[1]);
            renderer.materials = materials;
        }
    }

    public void UpdateSize(float currentHP, float maxHP)
    {
        float sizeFactor = Mathf.Clamp01(currentHP / maxHP);
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        Vector3[] vertices = new Vector3[originalVertices.Length];

        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = new Vector3(
                originalVertices[i].x * sizeFactor,
                originalVertices[i].y,
                originalVertices[i].z * sizeFactor
            );
        }

        mesh.vertices = vertices;
        mesh.RecalculateBounds();
    }

    void DetachSegment()
    {
        Transform upperPart = new GameObject("UpperPart").transform;
        upperPart.gameObject.AddComponent<TreeInfo>();
        upperPart.position = transform.position;
        upperPart.tag = "Holdable";
        upperPart.GetComponent<TreeInfo>().UpgradePrefab = UpgradeTreePrefab;

        MoveChildrenToUpperPart(transform, upperPart);

        Rigidbody upperRb = upperPart.gameObject.AddComponent<Rigidbody>();
        int childCount = GetChildCount(upperPart);

        upperRb.mass = Mathf.Max(childCount * 2.5f, 110f);
        upperRb.angularDamping = 2f;
        upperRb.linearDamping = 3f;
        upperRb.useGravity = true;
      
    }

    void MoveChildrenToUpperPart(Transform parent, Transform upperPart)
    {
        foreach (Transform child in parent)
        {
            child.SetParent(upperPart);
            
            upperPart.tag = "Holdable";
        }
    }

    public int GetChildCount(Transform parent)
    {
        int counter = 0;

        foreach (Transform child in parent)
        {
            counter++;
            counter += GetChildCount(child); // Count all descendants
        }

        return counter;
    }

    void UpdateParentMass()
    {
        Transform rootParent = GetRootParent(transform);
        int remainingChildCount = GetChildCount(rootParent);

        Rigidbody rootRb = rootParent.GetComponent<Rigidbody>();
        if (rootRb != null)
        {
            rootRb.mass = Mathf.Max(remainingChildCount * 2.5f, 110f);
            TreeInfo treeInfo = rootParent.GetComponent<TreeInfo>();
            treeInfo.totalPrice = treeInfo.CalculateTreePrice(rootParent);
            
        }
    }

    Transform GetRootParent(Transform current)
    {
        while (current.parent != null)
        {
            current = current.parent;
          
        }
        return current;
    }
}
