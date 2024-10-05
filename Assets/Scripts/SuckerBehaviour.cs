using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SuckerBehaviour : MonoBehaviour
{
    [SerializeField] PipeBehaviour pipe;
    [SerializeField] bool isActive = true;
    [SerializeField] bool isFirst = true;
    [SerializeField] AudioClip[] clips; 

    private bool isInteractable = true;

    private Collider2D poolCollider = null;
    private Vector3 offset;

    private Collider2D thisCollider;
    private new AudioSource audio;
    public void BecomeNotInteractable()
    {
        isInteractable = false;
    }
    public void BecomeInteractable()
    {
        isInteractable = true;
    }
    public void TakeOff()
    {
        if(poolCollider != null)
        {
            ActivatePool();
            poolCollider = null;
        }
    }
    private void Start()
    {
        thisCollider = GetComponent<Collider2D>();
        audio = GetComponent<AudioSource>();

        if (!isActive)
        {
            audio.PlayOneShot(clips[1]);

            PutOnPull();
            ActivatePool();
            pipe.SetWaterColor(GetPoolColor(), isFirst);
        }
        else
        {
            transform.position = AddMath.SetZto(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            offset = Vector3.zero;
            thisCollider.layerOverridePriority++;
        }
    }
    private void OnMouseDown()
    {
        if (isInteractable && !PauseMenu.isPaused)
        {
            if (isActive)// Когда мы хотим поставить присоску куда-то
            {
                PutOnPull();

                if (poolCollider != null)// Когда мы ставим присоску в бассейн
                {
                    if (!GetIsPoolRevealed())// Ставим присоску, если бассейн не раскрыт. Иначе ничего не происходит
                    {
                        audio.PlayOneShot(clips[0]);

                        ActivatePool();
                        pipe.SetWaterColor(GetPoolColor(), isFirst);

                        isActive = false;
                        GameManager.PlayerPickedSucker = false;
                        thisCollider.layerOverridePriority--;
                    }
                }
                else
                {
                    pipe.StartDestroyingForSuckers(isFirst);

                    isActive = false;
                    isInteractable = false;
                    GameManager.PlayerPickedSucker = false;
                    thisCollider.layerOverridePriority--;
                }
            }
            else// Если мы подбираем присоску
            {
                if (!GameManager.PlayerPickedSucker && pipe.TryTakeOff())
                {
                    offset = AddMath.SetZto(Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position);
                    ActivatePool();
                    pipe.StartUnsucking(isFirst);
                    poolCollider = null;

                    isActive = true;
                    GameManager.PlayerPickedSucker = true;
                    thisCollider.layerOverridePriority++;
                }
            }
        }
    }
    private void OnMouseOver()
    {
        if(isInteractable && Input.GetMouseButtonDown(1)) {
            pipe.StartDestroyingForSuckers(isFirst);

            isActive = false;
            isInteractable = false;
            GameManager.PlayerPickedSucker = false;
            thisCollider.layerOverridePriority--;
        }
    }
    private void PutOnPull()
    {
        poolCollider = null;

        List<Collider2D> hitColliders = new List<Collider2D>();
        Physics2D.OverlapCircle(transform.position, transform.localScale.x * 0.25f, new ContactFilter2D().NoFilter(), hitColliders);
        foreach (var i in hitColliders)
        {
            if (i.tag == "Pool")
            {
                poolCollider = i;
                break;
            }
        }
        hitColliders.Clear();
    }
    private void Update()
    {
        if (isActive)
            transform.position = AddMath.SetZto(Camera.main.ScreenToWorldPoint(Input.mousePosition) - offset);
    }
    private void OnDestroy()
    {
        if (poolCollider != null)
        {
            ActivatePool();
        }
    }
    private void ActivatePool()
    {
        poolCollider.gameObject.GetComponent<PoolBehaviour>().PoolActivate();
    }
    private bool GetIsPoolRevealed()
    {
        return poolCollider.gameObject.GetComponent<PoolBehaviour>().IsRevealed();
    }
    private Color GetPoolColor()
    {
        return poolCollider.gameObject.GetComponent<SpriteRenderer>().color;
    }
}
