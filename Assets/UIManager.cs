using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("UI Элементы")]
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI statusText;
    public TextMeshProUGUI gameOverText;

    private GameManager gameManager;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    void Update()
    {
        if (gameManager != null)
        {
            float timeLeft = gameManager.GetTimeLeft();
            timerText.text = $"Время: {Mathf.CeilToInt(timeLeft)}";
        }
    }

    public void UpdateStatus(bool isLava)
    {
        if (isLava)
        {
            statusText.text = "ЛАВА!";
            statusText.color = Color.red;
        }
        else
        {
            statusText.text = "БЕГИ!";
            statusText.color = Color.yellow;
        }
    }

    public void ShowGameOver()
    {
        gameOverText.enabled = true;
        statusText.enabled = false;
        timerText.enabled = false;
    }
}
