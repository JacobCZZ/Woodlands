using UnityEngine;
using UnityEngine.UI;

public class BuildingMode : MonoBehaviour
{
    public float cellSize = 1f;
    public GameObject plotPlane;

    public GameObject[] buildablePrefabs;
    public GameObject[] previewPrefabs;
    public Button[] uiButtons;
    public int[] prefabCosts; // Přidané pole s cenami

    private GameObject currentPrefab;
    private GameObject previewPrefab;
    private GameObject previewObject;
    private bool[,] grid;
    private Vector3 plotCenter;
    private int gridWidth, gridHeight;
    [SerializeField] private MouseLook mouseLook;
    public bool isOut = true;

    private ShopManager shopManager; // Odkaz na správce peněz

    private void Start()
    {
        Vector3 plotSize = plotPlane.GetComponent<Renderer>().bounds.size;
        plotCenter = plotPlane.GetComponent<Renderer>().bounds.center;

        gridWidth = Mathf.RoundToInt(plotSize.x / cellSize);
        gridHeight = Mathf.RoundToInt(plotSize.z / cellSize);

        grid = new bool[gridWidth, gridHeight];

        for (int i = 0; i < uiButtons.Length; i++)
        {
            int index = i;
            uiButtons[i].onClick.AddListener(() => SelectPrefab(index));
        }

        shopManager = FindObjectOfType<ShopManager>(); // Najde ShopManager ve scéně
    }

    private void Update()
    {
        if (currentPrefab == null) return;

        if (isOut)
        {
            Destroy(previewObject);
            previewObject = null;
        }

        Vector3 mousePosition = GetMouseWorldPosition();
        Vector2Int gridPosition = GetGridPosition(mousePosition);
        Vector2Int size = GetPrefabSize(currentPrefab);
        bool canPlace = IsWithinGrid(gridPosition, size) && IsPlaceable(gridPosition, size);

        if (previewObject != null)
        {
            foreach (Renderer renderer in previewObject.GetComponentsInChildren<Renderer>())
            {
                renderer.material.color = canPlace ? new Color(0, 1, 0, 0.5f) : new Color(1, 0, 0, 0.5f);
            }

            UpdatePreviewObject(gridPosition, size);
        }

        if (canPlace && Input.GetMouseButtonDown(0) && previewObject != null)
        {
            int index = System.Array.IndexOf(buildablePrefabs, currentPrefab);
            if (index >= 0 && index < prefabCosts.Length)
            {
                int cost = prefabCosts[index];
                if (shopManager != null && shopManager.playerMoney >= cost)
                {
                    shopManager.playerMoney -= cost;
                    shopManager.UpdateMoneyUI();
                    PlaceObject(gridPosition, size);
                }
                // Pokud není dost peněz, nic se nestane (lze přidat info hlášku)
            }

            Destroy(previewObject);
            previewObject = null;
            Cursor.lockState = CursorLockMode.None;
            if (mouseLook != null)
            {
                mouseLook.enabled = false;
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            Destroy(previewObject);
            previewObject = null;
            if (mouseLook != null)
            {
                mouseLook.enabled = true;
                Cursor.lockState = CursorLockMode.Locked;

            }
        }
    }

    private void SelectPrefab(int index)
    {
        currentPrefab = buildablePrefabs[index];
        previewPrefab = previewPrefabs[index];

        if (previewObject != null)
        {
            Destroy(previewObject);
        }

        previewObject = Instantiate(previewPrefab);
        previewObject.GetComponent<Collider>().enabled = false;

        Cursor.lockState = CursorLockMode.Locked;
        if (mouseLook != null)
        {
            mouseLook.enabled = true;
        }
    }

    private Vector3 GetMouseWorldPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
        {
            return hit.point;
        }
        return Vector3.zero;
    }

    private Vector2Int GetGridPosition(Vector3 worldPosition)
    {
        float relativeX = (worldPosition.x - plotCenter.x + (gridWidth * cellSize) / 2f);
        float relativeZ = (worldPosition.z - plotCenter.z + (gridHeight * cellSize) / 2f);

        int x = Mathf.FloorToInt(relativeX / cellSize);
        int y = Mathf.FloorToInt(relativeZ / cellSize);

        return new Vector2Int(x, y);
    }

    private bool IsWithinGrid(Vector2Int position, Vector2Int size)
    {
        return position.x >= 0 && position.y >= 0 &&
               position.x + size.x <= gridWidth &&
               position.y + size.y <= gridHeight;
    }

    private bool IsPlaceable(Vector2Int position, Vector2Int size)
    {
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                if (grid[position.x + x, position.y + y])
                {
                    return false;
                }
            }
        }
        return true;
    }

    private void UpdatePreviewObject(Vector2Int gridPosition, Vector2Int size)
    {
        if (previewObject != null)
        {
            Vector3 position = new Vector3(
                (gridPosition.x - gridWidth / 2f) * cellSize + (size.x * cellSize) / 2f,
                plotPlane.GetComponent<Renderer>().bounds.min.y - 0.5f,
                (gridPosition.y - gridHeight / 2f) * cellSize + (size.y * cellSize) / 2f
            );

            previewObject.transform.position = position + plotCenter;
        }
    }

    private void PlaceObject(Vector2Int gridPosition, Vector2Int size)
    {
        Vector3 position = new Vector3(
            (gridPosition.x - gridWidth / 2f) * cellSize + (size.x * cellSize) / 2f,
            plotPlane.GetComponent<Renderer>().bounds.min.y - 0.5f,
            (gridPosition.y - gridHeight / 2f) * cellSize + (size.y * cellSize) / 2f
        );

        GameObject placedObject = Instantiate(currentPrefab, position + plotCenter, Quaternion.identity);

        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                grid[gridPosition.x + x, gridPosition.y + y] = true;
            }
        }
    }

    private Vector2Int GetPrefabSize(GameObject prefab)
    {
        PrefabSize prefabSize = prefab.GetComponent<PrefabSize>();
        if (prefabSize != null)
        {
            return prefabSize.size;
        }
        return Vector2Int.one;
    }
}
