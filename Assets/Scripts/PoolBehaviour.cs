using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PoolBehaviour : MonoBehaviour
{
    public UnityEvent OnPoolActivate;
    [SerializeField] GameObject pipe;
    private bool isRevealed = true;

    private void OnMouseDown()
    {
        if(!isRevealed && !GameManager.PlayerPickedSucker && !PauseMenu.isPaused)
        {
            Instantiate(pipe, AddMath.SetZto(Camera.main.ScreenToWorldPoint(Input.mousePosition)), Quaternion.identity);
            GameManager.PlayerPickedSucker = true;
        }
    }

    public void PoolActivate()
    {
        isRevealed = !isRevealed;
        OnPoolActivate.Invoke();
    }

    public bool IsRevealed() { return isRevealed; }

    public void SetColor(Color color)
    {
        GetComponent<SpriteRenderer>().color = color;
    }
}
