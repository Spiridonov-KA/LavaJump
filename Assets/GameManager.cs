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
    public float safeCellPercent = 0.3f;

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

            int remaining = (int)(allCells.Length * safeCellPercent);
            int totalCells = allCells.Length;
            int[] indices = new int[totalCells];
            for (int i = 0; i < totalCells; i++) indices[i] = i;

            for (int i = 0; i < remaining && i < totalCells; i++)
            {
                int swapIdx = Random.Range(i, totalCells);
                (indices[i], indices[swapIdx]) = (indices[swapIdx], indices[i]); // swap

                allCells[indices[i]].SetState(CellController.CellState.Safe, safeMat);
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
