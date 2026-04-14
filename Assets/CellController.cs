using UnityEngine;
using System.Collections;

public class CellController : MonoBehaviour
{
    public enum CellState { Normal, Safe, Lava, Broken }
    public CellState state;

    private MeshRenderer meshRenderer;
    private Collider cellCollider;
    private Coroutine breakTimer;
    
    private bool isPlayerOnMe = false;
    private GameManager gameManager;

    void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        cellCollider = GetComponent<Collider>();
        gameManager = FindObjectOfType<GameManager>();
    }

    public void SetState(CellState newState, Material mat)
    {
        state = newState;
        if (mat != null) meshRenderer.material = mat;

        if (newState == CellState.Broken)
        {
            cellCollider.enabled = false;
            meshRenderer.enabled = false;
        }
        else
        {
            cellCollider.enabled = true;
            meshRenderer.enabled = true;
        }

        if (newState == CellState.Safe && breakTimer != null)
        {
            StopCoroutine(breakTimer);
            breakTimer = null;
        }
    }

    public void StartBreakTimer()
    {
        if (state != CellState.Safe) return;
        if (breakTimer != null) StopCoroutine(breakTimer);
        
        breakTimer = StartCoroutine(CountdownAndBreak());
    }

    public void StopBreakTimer()
    {
        if (breakTimer != null)
        {
            StopCoroutine(breakTimer);
            breakTimer = null;
        }
    }

    IEnumerator CountdownAndBreak()
    {
        bool isLavaPhase = (gameManager != null && gameManager.IsLavaPhase());
        
        float breakTime = isLavaPhase ? 1f : 5f;
        
        yield return new WaitForSeconds(breakTime);
        StartCoroutine(BreakCell());
    }

    IEnumerator BreakCell()
    {
        state = CellState.Broken;
        meshRenderer.material.color = Color.black; 
        yield return new WaitForSeconds(0.1f);
        SetState(CellState.Broken, null);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isPlayerOnMe = true;
            
            if (state == CellState.Safe)
            {
                StartBreakTimer();
            }
            else if (state == CellState.Lava)
            {
                KillPlayer();
            }
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isPlayerOnMe = false;
            StopBreakTimer();
        }
    }

    void KillPlayer()
    {
        GameManager gm = FindObjectOfType<GameManager>();
        if (gm != null) gm.GameOver();
    }
}
