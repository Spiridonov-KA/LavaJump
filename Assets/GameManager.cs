using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    [Header("Материалы")]
    public Material normalMat;
    public Material safeMat;
    public Material lavaMat;

    [Header("Настройки")]
    public float phaseDuration = 30f;
    public int safeCellCount = 5;

    private float timer = 0f;
    private bool isLavaPhase = false;
    private CellController[] allCells;
    
    private bool isGameOver = false;
    private UIManager uiManager;

    void Start()
    {
        StartCoroutine(FindCellsAfterDelay());
    }

    IEnumerator FindCellsAfterDelay()
    {
        yield return null;
        
        allCells = FindObjectsOfType<CellController>();
        
        Debug.Log("Найдено клеток: " + allCells.Length);
        
        if (allCells.Length == 0)
        {
            Debug.LogError("Клетки не найдены!");
            yield break;
        }
        
        foreach (CellController cell in allCells)
        {
            if (cell != null) cell.SetState(CellController.CellState.Normal, normalMat);
        }

        Debug.Log("Игра запущена.");
        uiManager = FindObjectOfType<UIManager>();
	if (uiManager != null) uiManager.UpdateStatus(false);
    }

    void Update()
    {
        if (isGameOver)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                RestartGame();
            }
            return;
        }

        timer += Time.deltaTime;
        if (timer >= phaseDuration)
        {
            timer = 0f;
            SwitchPhase();
        }
    }
    
	public float GetTimeLeft()
	{
	    return phaseDuration - timer;
	}
	
	public bool IsLavaPhase()
	{
	    return isLavaPhase;
	}

    public void GameOver()
    {
        if (isGameOver) return;
        
        isGameOver = true;
        Time.timeScale = 0;
        Debug.Log("GAME OVER! Нажми R для рестарта.");
        if (uiManager != null) uiManager.ShowGameOver();
    }

    void RestartGame()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void SwitchPhase()
    {
        isLavaPhase = !isLavaPhase;

        if (isLavaPhase)
        {
            Debug.Log("Фаза: ЛАВА!");
            
            if (uiManager != null) uiManager.UpdateStatus(true);
            
            foreach (CellController c in allCells)
                if (c != null) c.SetState(CellController.CellState.Lava, lavaMat);

            List<int> availableIndices = new List<int>();
            for (int i = 0; i < allCells.Length; i++)
                availableIndices.Add(i);

            for (int i = 0; i < safeCellCount && availableIndices.Count > 0; i++)
            {
                int randIdx = Random.Range(0, availableIndices.Count);
                int realIdx = availableIndices[randIdx];
                
                allCells[realIdx].SetState(CellController.CellState.Safe, safeMat);
                availableIndices.RemoveAt(randIdx);
            }
        }
        else
        {
            Debug.Log("Фаза: Нормальный пол.");
            
            if (uiManager != null) uiManager.UpdateStatus(false);
            foreach (CellController c in allCells)
            {
                if (c != null)
                {
                    c.SetState(CellController.CellState.Normal, normalMat);
                    c.StopBreakTimer();
                }
            }
        }
    }
}
