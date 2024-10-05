using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventManager : MonoBehaviour
{
    public static UnityEvent<int> PipeDestroyed = new UnityEvent<int>();
    public static UnityEvent EnemyHitedPipe = new UnityEvent();
    public static UnityEvent OnGameOver = new UnityEvent();
    public static UnityEvent OnMistakeHappen = new UnityEvent();
    public static UnityEvent OnGameStart = new UnityEvent();
    public static UnityEvent OnGoingToMenu = new UnityEvent();
    public static UnityEvent OnAllPipesDestroy = new UnityEvent();
    public static UnityEvent OnPlayerPickedVoid = new UnityEvent();
    public static UnityEvent OnGameWin = new UnityEvent();
    public static UnityEvent OnActivateWinMenu = new UnityEvent();

    static public void ClearAllEvents()
    {
        PipeDestroyed.RemoveAllListeners();
        EnemyHitedPipe.RemoveAllListeners();
        OnGameOver.RemoveAllListeners();
        OnMistakeHappen.RemoveAllListeners();
        OnGameStart.RemoveAllListeners();
        OnGoingToMenu.RemoveAllListeners();
        OnAllPipesDestroy.RemoveAllListeners();
        OnPlayerPickedVoid.RemoveAllListeners();
        OnGameWin.RemoveAllListeners();
        OnActivateWinMenu.RemoveAllListeners();
    }
}
