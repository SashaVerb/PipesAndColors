using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Warning : MonoBehaviour
{
    [SerializeField] Transform mask;

    private const float offset = 0.5f;
    private float delay = 1f;

    public void SetDelay(float delay)
    {
        this.delay = delay;
    }

    private void Start()
    {
        if(delay == 0f)
        {
            Disappear();
        }
        else
        {
            if (Mathf.Abs(transform.position.x) > Mathf.Abs(GameManager.cameraSize.x))
            {
                transform.position = new Vector3(Mathf.Sign(transform.position.x) * (GameManager.cameraSize.x - offset), transform.position.y, transform.position.z);
            }
            else
            {
                transform.position = new Vector3(transform.position.x, Mathf.Sign(transform.position.y) * (GameManager.cameraSize.y - offset), transform.position.z);
            }

            transform.DOScale(transform.localScale * 2, 0.2f)
                .SetLink(gameObject)
                .SetLoops(-1, LoopType.Yoyo);
            mask.DOScaleY(1.8f, delay)
                .SetLink(gameObject)
                .SetEase(Ease.Linear)
                .OnComplete(Disappear);
        }
    }
    private void Disappear()
    {
        gameObject.SetActive(false);
    }
}
