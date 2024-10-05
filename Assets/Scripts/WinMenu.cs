using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinMenu : MonoBehaviour
{
    [SerializeField] GameObject winMenu;
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] TextMeshProUGUI copyText;

    private new AudioSource audio;

    private void Awake()
    {
        audio = GetComponent<AudioSource>();
        EventManager.OnActivateWinMenu.AddListener(Activate);
    }
    public void Activate()
    {
        winMenu.SetActive(true);
        Time.timeScale = 0f;
        timerText.text = copyText.text;

        audio.Play();

        GameManager.PlayerPickedSucker = true;
    }
    public void Restart()
    {
        EventManager.OnGameOver.Invoke();
    }
    public void Continue()
    {
        if(SceneManager.GetActiveScene().buildIndex == 3)
        {
            EventManager.OnGoingToMenu.Invoke();
        }
        else
        {
            EventManager.OnGameWin.Invoke();
        }
    }

    public void GoToMenu()
    {
        EventManager.OnGoingToMenu.Invoke();
    }
}
