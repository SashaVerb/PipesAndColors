using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TransitionBehaviour : MonoBehaviour
{
    [SerializeField] float duration = 2f;
    [SerializeField] CanvasScaler canvas;
    private Vector2 screenSize;

    private RectTransform rTransform;
    private new AudioSource audio;
    private void Awake()
    {
        audio = GetComponent<AudioSource>();
        rTransform = GetComponent<RectTransform>();

        float radious = canvas.referenceResolution.magnitude;
        rTransform.sizeDelta = new Vector2(radious, radious);
        screenSize = (rTransform.sizeDelta + canvas.referenceResolution) * 0.5f;

        EventManager.OnGameOver.AddListener(Restart);
        EventManager.OnGoingToMenu.AddListener(GoToMenu);
        EventManager.OnGameWin.AddListener(StartGame);
    }

    private void Start()
    {
        Time.timeScale = 1f;
        EndTransition();
    }

    public void Restart()
    {
        rTransform.DOAnchorPos(Vector2.zero, duration)
            .SetUpdate(true)
            .SetLink(gameObject)
            .OnComplete(SceneLoader.RestartLevel);
    }

    public void StartGame()
    {
        rTransform.DOAnchorPos(Vector2.zero, duration)
            .SetUpdate(true)
            .SetLink(gameObject)
            .OnComplete(SceneLoader.NextLevel);
    }
    public void GoToMenu()
    {
        rTransform.DOAnchorPos(Vector2.zero, duration)
            .SetUpdate(true)
            .SetLink(gameObject)
            .OnComplete(SceneLoader.MenuLevel);
    }
    public void WinGame()
    {
        audio.Play();
        rTransform.DOAnchorPos(Vector2.zero, duration)
            .SetUpdate(true)
            .SetLink(gameObject)
            .OnComplete(SceneLoader.MenuLevel);
    }
    public void EndTransition()
    {
        Vector2 direction = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
        if(direction.x > direction.y)
        {
            rTransform.DOAnchorPos(new Vector2(Mathf.Sign(direction.x), direction.y) * screenSize, duration)
                .SetLink(gameObject)
                .OnComplete(EventManager.OnGameStart.Invoke);
        }
        else
        {
            rTransform.DOAnchorPos(new Vector2(direction.x, Mathf.Sign(direction.y)) * screenSize, duration)
                .SetLink(gameObject)
                .OnComplete(EventManager.OnGameStart.Invoke);
        }
    }
}
