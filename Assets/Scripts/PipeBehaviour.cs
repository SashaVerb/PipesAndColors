using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PipeBehaviour : MonoBehaviour
{
    // Нужно для растягивания трубы от присоски до другой присоски
    [SerializeField] SuckerBehaviour firstSucker;
    [SerializeField] SuckerBehaviour secondSucker;
    [SerializeField] float destroyingSpeed = 10;

    // Нужно для качки воды по трубам
    [SerializeField] GameObject firstWater;
    [SerializeField] GameObject secondWater;
    [SerializeField] float suckingSpeed = 2;
    [SerializeField] FullPipe fullPipe;
    [SerializeField] SpriteRenderer upBorder;
    [SerializeField] SpriteRenderer downBorder;
    [SerializeField] GameObject successEffect;

    private bool firstSuckerHasColor = false;
    private bool secondSuckerHasColor = false;
    private bool sameColors = false;

    private bool isDestroying = false;// Состояние самоуничтожения трубы
    private bool isSucking = false;// Состояние проверки двух бассейнов
    private bool firstIsUnsucking = false;// Состояние откачки в первую присоску
    private bool secondIsUnsucking = false;// Состояние откачки во вторую присоску
    private bool isMistakeHappened = false;// Состояние, когда игрок ошибся
    private bool isRightState = false;

    private bool firstToSecond; //Если true, значит первая присоска должна притягиваться ко второй во время уничтожения

    private const float firstWaterStartPos = 0.5f;
    private const float secondWaterStartPos = -0.5f;

    Vector3 diff;
    private BoxCollider2D thisCollider;
    private SpriteRenderer spriteRenderer;
    private new AudioSource audio;

    private void Awake()
    {
        thisCollider = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        audio = GetComponent<AudioSource>();
        thisCollider.enabled = false;

        EventManager.OnAllPipesDestroy.AddListener(StartDestroyingFirstSucker);
    }
    private void Update()
    {
        if(isSucking)
        {
            float stlpX = secondWater.transform.localPosition.x;
            float diff = AddMath.MoveTowards(stlpX, firstWaterStartPos, suckingSpeed * Time.deltaTime) - stlpX;
            firstWater.transform.localPosition -= new Vector3(diff, 0);
            secondWater.transform.localPosition += new Vector3(diff, 0);
            
            firstWater.transform.localScale += new Vector3(diff * 2f, 0);
            secondWater.transform.localScale += new Vector3(diff * 2f, 0);

            if ((firstWater.transform.localScale.x + secondWater.transform.localScale.x) > 1f)
            {
                isSucking = false;

                if(sameColors)
                {
                    secondWater.transform.localPosition = firstWater.transform.localPosition = Vector3.zero;
                    secondWater.transform.localScale = firstWater.transform.localScale = new Vector3(1, firstWater.transform.localScale.y, 1);

                    firstSucker.BecomeInteractable();
                    secondSucker.BecomeInteractable();

                    var effect = Instantiate(successEffect, firstWater.transform.position + new Vector3(0, 0, -5), Quaternion.identity)
                        .GetComponent<ParticleSystem>().main;
                    effect.startColor = firstWater.GetComponent<SpriteRenderer>().color;

                    GameManager.ReactOnRightPipe();
                    isRightState = true;
                }
                else
                {
                    BlinkRed();
                    isMistakeHappened = true;
                    firstIsUnsucking = true;
                    secondIsUnsucking = true;
                }
            }
        }
        else
        {
            if (isDestroying)// Уничтожение
            {
                if (firstToSecond)
                {
                    firstSucker.transform.position = Vector3.MoveTowards(firstSucker.transform.position, secondSucker.transform.position, destroyingSpeed * Time.deltaTime);
                }
                else
                {
                    secondSucker.transform.position = Vector3.MoveTowards(secondSucker.transform.position, firstSucker.transform.position, destroyingSpeed * Time.deltaTime);
                }

                if (Vector3.Distance(firstSucker.transform.position, secondSucker.transform.position) < 0.1f)
                {
                    fullPipe.DestroyFullPipe();
                }
            }

            //Растягивание трубы
            diff = Vector3.Lerp(firstSucker.transform.position, secondSucker.transform.position, 0.5f) - transform.position;
            diff.Set(diff.x, diff.y, 0);

            transform.position += diff;

            transform.rotation = Quaternion.FromToRotation(Vector3.right, firstSucker.transform.position - secondSucker.transform.position);

            transform.localScale = new Vector3((firstSucker.transform.localPosition - secondSucker.transform.localPosition).magnitude, transform.localScale.y, 1);

            if (firstIsUnsucking)
            {
                float ftlpX = firstWater.transform.localPosition.x;
                float diff = AddMath.MoveTowards(ftlpX, firstWaterStartPos, suckingSpeed * Time.deltaTime) - ftlpX;
                firstWater.transform.localPosition += new Vector3(diff, 0);

                firstWater.transform.localScale -= new Vector3(diff * 2f, 0);

                if (ftlpX == firstWaterStartPos)
                {
                    ResetFirstWater();
                    firstIsUnsucking = false;
                    if(isMistakeHappened && !secondIsUnsucking)
                    {
                        StartDestroying(false);
                    }
                }
            }

            if (secondIsUnsucking)
            {
                float stlpX = secondWater.transform.localPosition.x;
                float diff = AddMath.MoveTowards(stlpX, secondWaterStartPos, suckingSpeed * Time.deltaTime) - stlpX;
                secondWater.transform.localPosition += new Vector3(diff, 0);

                secondWater.transform.localScale += new Vector3(diff * 2f, 0);

                if (stlpX == secondWaterStartPos)
                {
                    ResetSecondWater();
                    secondIsUnsucking = false;
                    if (isMistakeHappened && !firstIsUnsucking)
                    {
                        StartDestroying(false);
                    }
                }
            }
        }
    }
    public void SetWaterColor(Color color, bool fromFirstSucker)
    {
        color = DarkerColor(color);

        if (fromFirstSucker)
        {
            firstWater.GetComponent<SpriteRenderer>().color = color;
            firstSuckerHasColor = true;
        }
        else
        {
            secondWater.GetComponent<SpriteRenderer>().color = color;
            secondSuckerHasColor = true;
        }

        if (firstSuckerHasColor && secondSuckerHasColor)
        {
            sameColors = ColorsAreEqual(firstWater.GetComponent<SpriteRenderer>().color, secondWater.GetComponent<SpriteRenderer>().color);

            StartSucking();
        }
    }
    public void UnsetWaterColor(bool unsetFirstSucker)
    {
        if(unsetFirstSucker)
        {
            firstSuckerHasColor = false;
        }
        else
        {
            secondSuckerHasColor = false;
        }
    }
    void StartSucking()
    {
        audio.PlayOneShot(audio.clip);
        isSucking = true;
        firstIsUnsucking = false;
        secondIsUnsucking = false;
        thisCollider.enabled = true;

        firstSucker.BecomeNotInteractable();
        secondSucker.BecomeNotInteractable();
    }
    public void StartDestroyingForSuckers(bool firstSuckerIsReason = true)
    {
        thisCollider.enabled = true;

        isDestroying = true;
        isSucking = false;
        if (firstToSecond = firstSuckerIsReason)
        {
            firstSucker.TakeOff();
        }
        else
        {
            secondSucker.TakeOff();
        }

        firstSucker.BecomeNotInteractable();
        secondSucker.BecomeNotInteractable();

        if (sameColors)
        {
            GameManager.ReactOnBreakingRightPipe();
            isRightState = false;
        }
        else
        {
            EventManager.OnPlayerPickedVoid.Invoke();
        }
    }

    public void StartDestroying(bool firstSuckerIsReason = true)
    {
        thisCollider.enabled = true;

        isDestroying = true;
        isSucking = false;
        if(firstToSecond = firstSuckerIsReason)
        {
            firstSucker.TakeOff();
        }
        else
        {
            secondSucker.TakeOff();
        }

        firstSucker.BecomeNotInteractable();
        secondSucker.BecomeNotInteractable();

        if(isRightState)
        {
            GameManager.ReactOnBreakingRightPipe();
            isRightState = false;
        }
        else
        {
            EventManager.OnMistakeHappen.Invoke();
        }
    }

    private void StartDestroyingFirstSucker()
    {
        if(firstSuckerHasColor && secondSuckerHasColor && !isDestroying)
        {
            thisCollider.enabled = true;

            isDestroying = true;
            isSucking = false;

            firstToSecond = true;
            firstSucker.TakeOff();

            firstSucker.BecomeNotInteractable();
            secondSucker.BecomeNotInteractable();

            if (isRightState)
            {
                GameManager.ReactOnBreakingRightPipe();
                isRightState = false;
            }
        }
    }

    public void StartUnsucking(bool firstSuckerIsReason)
    {
        thisCollider.enabled = false;

        if (firstSuckerIsReason)
        {
            secondIsUnsucking = true;
            ResetFirstWater();
        }
        else
        {
            firstIsUnsucking = true;
            ResetSecondWater();
        }

        if (isRightState)
        {
            GameManager.ReactOnBreakingRightPipe();
            isRightState = false;
        }
    }
    public void StartUnsuckingBoth()
    {
        firstIsUnsucking = true;
        secondIsUnsucking = true;
        if (isRightState)
        {
            GameManager.ReactOnBreakingRightPipe();
            isRightState = false;
        }
    }
    private Color DarkerColor(Color color)
    {
        color.r -= 0.15f;
        color.g -= 0.15f;
        color.b -= 0.15f;
        return color;
    }
    private void ResetSecondWater()
    {
        secondWater.transform.localScale = new Vector3(0, secondWater.transform.localScale.y, 1);
        secondWater.transform.localPosition = new Vector3(secondWaterStartPos, 0, 0);
    }

    private void ResetFirstWater()
    {
        firstWater.transform.localScale = new Vector3(0, firstWater.transform.localScale.y, 1);
        firstWater.transform.localPosition = new Vector3(firstWaterStartPos, 0, 0);
    }
    
    private bool ColorsAreEqual(Color a, Color b)
    {
        a = a - b;
        return (-0.005f < a.r && a.r < 0.005f) && (-0.005f < a.g && a.g < 0.005f) && (-0.005f < a.b && a.b < 0.005f);
    }

    public void BlinkRed()
    {
        if(!DOTween.IsTweening(spriteRenderer))
        {
            spriteRenderer.DOColor(Color.red, 0.15f)
                .SetLink(gameObject)
                .SetLoops(8, LoopType.Yoyo);

            upBorder.DOColor(Color.red, 0.15f)
                .SetLink(gameObject)
                .SetLoops(8, LoopType.Yoyo);
            upBorder.transform.DOScaleY(upBorder.transform.localScale.y * 1.5f, 0.15f)
                .SetLink(gameObject)
                .SetLoops(8, LoopType.Yoyo);

            downBorder.DOColor(Color.red, 0.15f)
                .SetLink(gameObject)
                .SetLoops(8, LoopType.Yoyo);
            downBorder.transform.DOScaleY(downBorder.transform.localScale.y * 1.5f, 0.15f)
                .SetLink(gameObject)
                .SetLoops(8, LoopType.Yoyo);
        }
    }
    public bool TryTakeOff()
    {
        Collider2D[] hitColliders = Physics2D.OverlapBoxAll(transform.position, transform.localScale, transform.eulerAngles.z, LayerMask.GetMask("Pipe"));
        bool thereNoUpperPipe = true;
        if (hitColliders.Length == 1) {// Оно считает коллайдер трубы что этот метод и вызывает
            return true;
        }
        else
        {
            foreach(var collider in hitColliders)
            {
                var pipe = collider.GetComponent<PipeBehaviour>();
                if (pipe.GetOrder() > GetOrder())
                {
                    pipe.BlinkRed();
                    thereNoUpperPipe = false;
                }
            }
            return thereNoUpperPipe;
        }
    }

    public int GetOrder()
    {
        return fullPipe.GetOrder();
    }
}
