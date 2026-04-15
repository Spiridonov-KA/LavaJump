using UnityEngine;

public class GridSpawner : MonoBehaviour
{
    [Header("Настройки сетки")]
    public GameObject cellPrefab;
    public int gridSize = 30;
    public float spacing = 1.1f;
    
    [SerializeField] private GameManager gameManager;

    private Transform gridParent;

    void Awake()
    {
        gridParent = new GameObject("GridContainer").transform;
        gridParent.SetParent(this.transform);
        
        if (gameManager == null)
            gameManager = FindObjectOfType<GameManager>();
    }

    void Start()
    {
        if (cellPrefab == null)
        {
            Debug.LogError("Не назначен префаб клетки!");
            return;
        }

        for (int x = 0; x < gridSize; x++)
        {
            for (int z = 0; z < gridSize; z++)
            {
                Vector3 spawnPos = new Vector3(
                    x * spacing, 
                    0f, 
                    z * spacing
                );

                Instantiate(cellPrefab, spawnPos, Quaternion.identity, gridParent);
            }
        }

        Debug.Log($"Сетка создана! Клеток: {gridSize * gridSize}");
        
        if (gameManager != null)
        {
            gameManager.OnGridReady();
        }
    }
}