using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] GameObject enemyPrefab;
    [SerializeField] float spawnOffset = 5f;
    [SerializeField] float monsterSpeed = 1f;
    [SerializeField] float delay = 6f;
    [SerializeField] Vector2 limiter;

    private int monsterBuffCounter = 0;
    private const int monsterBuffActivate = 3;

    private int allPipeDestroyCounter = 0;
    private const int allPipeDestroyActivate = 5;

    private RaycastHit2D[] buff = new RaycastHit2D[3];
    private void Awake()
    {
        EventManager.OnMistakeHappen.AddListener(SpawnWave);
    }

    public void SpawnWave()
    {
        var enemy = Instantiate(enemyPrefab, FindGoodPosition(), Quaternion.identity);
        enemy.GetComponentInChildren<EnemyBehavior>().SetSpeed(monsterSpeed);
        enemy.GetComponentInChildren<EnemyBehavior>().SetDelay(delay);
        enemy.GetComponentInChildren<Warning>().SetDelay(delay);

        monsterBuffCounter += 1;
        allPipeDestroyCounter += 1;

        if (monsterBuffCounter >= monsterBuffActivate)
        {
            delay = Mathf.MoveTowards(delay, 0f, 1f);
            monsterSpeed += 1f;

            monsterBuffCounter = 0;
        }

        //if (allPipeDestroyCounter >= allPipeDestroyActivate)
        //{
        //    EventManager.OnAllPipesDestroy.Invoke();

        //    allPipeDestroyCounter = 0;
        //}
    }

    public float GetSpeed()
    {
        return monsterSpeed;
    }

    public float GetDelay()
    {
        return delay;
    }
    public Vector3 FindGoodPosition()
    {
        Vector2 bestPos = new Vector2(0, GameManager.cameraSize.y + spawnOffset);
        int maxAmount = 0, curAmount;
        float randSign = Mathf.Sign(Random.value - 0.5f);

        Vector2 pos = bestPos;
        for (int i = 0; i < 3; ++i)
        {
            pos.x = GameManager.cameraSize.x * Random.Range(-limiter.x, limiter.x);

            curAmount = Physics2D.RaycastNonAlloc(pos, Vector2.down, buff, GameManager.cameraSize.y * 2, LayerMask.GetMask("Pipe"));

            if (curAmount > maxAmount || (curAmount == maxAmount && Random.value > 0.5f))
            {
                bestPos = pos * randSign;
                maxAmount = curAmount;
            }
        }

        pos = new Vector2(GameManager.cameraSize.x + spawnOffset, 0);

        for (int i = 0; i < 3; ++i)
        {
            pos.y = GameManager.cameraSize.y * Random.Range(-limiter.y, limiter.y);
            curAmount = Physics2D.RaycastNonAlloc(pos, Vector2.left, buff, GameManager.cameraSize.x * 2, LayerMask.GetMask("Pipe"));

            if (curAmount > maxAmount || (curAmount == maxAmount && Random.value > 0.5f))
            {
                bestPos = pos * randSign;
                maxAmount = curAmount;
            }
        }

        return bestPos;
    }
}
