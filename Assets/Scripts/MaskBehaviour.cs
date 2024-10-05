using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaskBehaviour : MonoBehaviour
{
    [SerializeField] float scalingSpeed = 1f;
    [SerializeField] float scaleMax = 8.5f;

    private bool isActive;
    private bool scalingIsOver;
    private Vector3 targetScale;

    private void Awake()
    {
        isActive = true;
        scalingIsOver = true;
        targetScale = new Vector3(scaleMax, scaleMax, 0);
    }

    private void Update()
    {
        if(!scalingIsOver)
        {
            if(isActive)
            {
                transform.localScale = Vector3.MoveTowards(transform.localScale, targetScale, scalingSpeed * Time.deltaTime);
                if(transform.localScale == targetScale)
                {
                    scalingIsOver = true;
                }
            }
            else
            {
                transform.localScale = Vector3.MoveTowards(transform.localScale, Vector3.zero, scalingSpeed * Time.deltaTime);
                if (transform.localScale == Vector3.zero)
                {
                    scalingIsOver = true;
                }
            }
        }
    }
    public void ChangeStatus()
    {
        if (scalingIsOver)
        {
            if(!isActive) 
            {
                Vector3 clickPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                transform.position = new Vector3(clickPos.x, clickPos.y, 0);
            }
            scalingIsOver = false;
        }
        isActive = !isActive;
    }
}
