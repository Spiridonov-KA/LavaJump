using UnityEngine;

public class GridSpawner : MonoBehaviour
{
    [Header("Настройки сетки")]
    public GameObject cellPrefab;
    public int gridSize = 30;
    public float spacing = 1.1f;

    void Start()
    {
        if (cellPrefab == null)
        {
            Debug.LogError("Не назначен префаб клетки! Перетащите Cell в поле Cell Prefab в Inspector.");
            return;
        }

        for (int x = 0; x < gridSize; x++)
        {
            for (int z = 0; z < gridSize; z++)
            {
                Vector3 spawnPosition = new Vector3(x * spacing, 0f, z * spacing);

                Instantiate(cellPrefab, spawnPosition, Quaternion.identity);
            }
        }

        Debug.Log("Сетка создана! Всего клеток: " + (gridSize * gridSize));
    }
}
