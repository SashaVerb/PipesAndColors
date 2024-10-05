using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    static public void RestartLevel()
    {
        EventManager.ClearAllEvents();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    static public void NextLevel()
    {
        EventManager.ClearAllEvents();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    static public void MenuLevel()
    {
        EventManager.ClearAllEvents();
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }
}
