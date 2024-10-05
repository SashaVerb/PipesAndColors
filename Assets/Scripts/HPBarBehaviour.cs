using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPBarBehaviour : MonoBehaviour
{
    [SerializeField] AudioClip looseSound;
    [SerializeField] Image[] hpImages;
    [SerializeField] Image[] pickingBar;
    [SerializeField] float fadingSpeed;
    [SerializeField] Color pickColor;
    [SerializeField] Color backColor;
    private bool[] hpController;
    private List<Image> lostHp;
    bool isLoosingHp;
    bool lostAllHp = false;
    int playerPickedVoidCounter = 0;
    const int playerPickedVoidActivate = 5;

    private new AudioSource audio;
    private void Awake()
    {
        audio = GetComponent<AudioSource>();

        hpController = new bool[hpImages.Length - 1];// Последнее хп обрабатывается отдельно
        for(int i = 0; i < hpController.Length; i++)
        {
            hpController[i] = true;
        }
        lostHp = new List<Image>(hpImages.Length / 2);

        isLoosingHp = false;
        EventManager.EnemyHitedPipe.AddListener(LooseHP);
        EventManager.OnPlayerPickedVoid.AddListener(ReactOnPickingVoid);
    }

    private void Update()
    {
        if(isLoosingHp)
        {
            for(int i = 0; i < lostHp.Count; i++)
            {
                lostHp[i].color -= new Color(0, 0, 0, fadingSpeed * Time.deltaTime);

                if (lostHp[i].color.a <= 0)
                {
                    lostHp.RemoveAt(i--);// Чтобы не пропустить следующий элемент
                }
            }
            if(lostHp.Count < 0)
            {
                isLoosingHp=false;
            }
        }
    }
    private void ReactOnPickingVoid()
    {
        pickingBar[playerPickedVoidCounter].color = backColor;
        
        ++playerPickedVoidCounter;
        if(playerPickedVoidCounter >= playerPickedVoidActivate)
        {
            LooseHP();
            playerPickedVoidCounter = 0;

            foreach(var image in pickingBar)
            {
                image.color = pickColor;
            }
        }
    }
    public void LooseHP()
    {
        audio.Play();

        for(int i = 0; i < hpController.Length; i++)
        {
            if (hpController[i])
            {
                lostHp.Add(hpImages[i]);
                hpController[i] = false;
                isLoosingHp = true;
                return;
            }
        }

        // Если мы дошли сюда, значит потеряно последнее хп
        if(!lostAllHp)
        {
            lostHp.Add(hpImages[hpImages.Length - 1]);
            isLoosingHp = true;

            audio.PlayOneShot(looseSound);
            EventManager.OnGameOver.Invoke();
            lostAllHp = true;
        }
    }
}
