using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using static UnityEngine.Tilemaps.TilemapRenderer;

public class GameManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;
    public static float timer = 0;
    [SerializeField] Color[] colors;

    private List<Color> deque;
    public static bool PlayerPickedSucker;
    private static int PipesCount;
    private static int PoolsCount;
    public static Vector2 cameraSize;

    private bool timerGo;

    private GameObject[] pools;
    private void Awake()
    {
        timer = 0;
        timerGo = false;
        PlayerPickedSucker = true;
        cameraSize = (Camera.main.ViewportToWorldPoint(Vector2.one) - Camera.main.ViewportToWorldPoint(Vector2.zero)) * 0.5f;

        EventManager.PipeDestroyed.AddListener(DecreasePipesCount);
        EventManager.OnGameStart.AddListener(StartGame);

        pools = GameObject.FindGameObjectsWithTag("FullPool");
        PoolsCount = pools.Length;

        deque = new List<Color>(PoolsCount);

        for (int i = 0; (i * 2 + 1) < deque.Capacity; i++)
        {
            deque.Add(colors[i]);
            deque.Add(colors[i]);
        }
        int sortOrder = 0;
        foreach (var pool in pools)
        {
            int i = Random.Range(0, deque.Count);
            pool.GetComponentInChildren<PoolBehaviour>().SetColor(deque[i]);
            pool.GetComponent<SortingGroup>().sortingOrder = sortOrder++;
            deque.RemoveAt(i);
        }
        PipesCount = sortOrder;
    }
    private void Update()
    {
        if(timerGo)
        {
            timer += Time.deltaTime * Time.timeScale;
            int minute = (int)(timer / 60), second = (int)(timer % 60);
            text.text = "";
            text.text += minute < 10 ? "0" + minute.ToString() : minute.ToString();
            text.text += ":";
            text.text += second < 10 ? "0" + second.ToString() : second.ToString();
        }
    }
    private void StartGame()
    {
        StartCoroutine(ActivateAllPools());
    }
    private IEnumerator ActivateAllPools()
    {
        foreach (var pool in pools)
        {
            pool.GetComponentInChildren<PoolBehaviour>().PoolActivate();
            yield return new WaitForSeconds(Random.Range(0.05f, 0.1f));
        }
        PlayerPickedSucker = false;
        PauseMenu.canBePaused = true;
        timerGo = true;
    }
    static public int GetPipesCount()
    {
        return PipesCount;
    }
    static public void IncreasePipesCount()
    {
        ++PipesCount;
    }
    static private void DecreasePipesCount(int a)
    {
        --PipesCount;
    }
    static public Vector2 RandomPointOnCameraEdge(float spawnOffset = 0)
    {
        Vector2 seed = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        if (Mathf.Abs(seed.x) >= Mathf.Abs(seed.y))
        {
            return cameraSize * (seed / Mathf.Abs(seed.x)) + new Vector2(spawnOffset * Mathf.Sign(seed.x), 0);
        }
        else
        {
            return cameraSize * (seed / Mathf.Abs(seed.y)) + new Vector2(0, spawnOffset * Mathf.Sign(seed.y));
        }
    }
    static public void ReactOnRightPipe()
    {
        PoolsCount -= 2;

        if (PoolsCount <= 0 ) 
        {
            EventManager.OnActivateWinMenu.Invoke();
        }
    }
    static public void ReactOnBreakingRightPipe()
    {
        PoolsCount += 2;
    }
}
