using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemyBehavior : MonoBehaviour
{
    [SerializeField] float turboIncreasing = 5f;

    private float speed = 1f;
    private float turbo = 1f;
    private float delay = 1f;

    bool wasOnCamera = false;
    bool isStunned = true;
    bool hitedPipe = false;

    private static int layerOrder = 100;

    private new AudioSource audio;

    private void Awake()
    {   
        audio = GetComponent<AudioSource>();

        GetComponent<SpriteRenderer>().sortingOrder = layerOrder;
        if(GameManager.cameraSize.x <= Mathf.Abs(transform.position.x))
        {
            transform.rotation = Quaternion.FromToRotation(Vector3.up, Vector3.right * -Mathf.Sign(transform.position.x));
        }
        else
        {
            transform.rotation = Quaternion.FromToRotation(Vector3.up, Vector3.up * -Mathf.Sign(transform.position.y));
        }
    }
    private void Start()
    {
        StartCoroutine(Wating());
    }
    private void Update()
    {
        if(!isStunned)
        {
            if(Input.GetKey(KeyCode.Space))
            {
                transform.position += transform.TransformDirection((Vector3.up * speed * turbo) * Time.deltaTime);
                turbo += turboIncreasing * Time.deltaTime;
            }
            else
            {
                transform.position += transform.TransformDirection(Vector3.up * speed * Time.deltaTime);
                turbo = 1f;
            }
        }
    }
    private IEnumerator Wating()
    {
        yield return new WaitForSeconds(delay);
        isStunned = false;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Pipe")
        {
            audio.PlayOneShot(audio.clip);
            collision.GetComponent<PipeBehaviour>().StartDestroying(true);

            if (!hitedPipe)
            {
                EventManager.EnemyHitedPipe.Invoke();
                BecomeBlack();
            }
        }
    }
    private void BecomeBlack()
    {
        hitedPipe = true;
        GetComponent<SpriteRenderer>().color = Color.black;
    }
    private void OnBecameInvisible()// Когда враг выходит из камеры
    {
        if (wasOnCamera)
        {
            Destroy(transform.parent.gameObject);
        }
    }
    private void OnBecameVisible()// Когда враг входит в камеру
    {
        if(!isStunned)
        {
            wasOnCamera = true;
        }
    }
    public void SetSpeed(float speed)
    {
        this.speed = speed;
    }
    public void SetDelay(float delay)
    {
        this.delay = delay;
    }
    static public void setLayerOrder(int order)
    {
        layerOrder = order;
    }
}
