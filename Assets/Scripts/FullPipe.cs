using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class FullPipe : MonoBehaviour
{
    private SortingGroup sortingGroup;
    private void Awake()
    {
        sortingGroup = GetComponent<SortingGroup>();
        sortingGroup.sortingOrder = GameManager.GetPipesCount();
        GameManager.IncreasePipesCount();

        EventManager.PipeDestroyed.AddListener(UpdateLayerId);
    }
    public void UpdateLayerId(int freeLayer)
    {
        if(freeLayer < sortingGroup.sortingOrder)
        {
            --sortingGroup.sortingOrder;
        }
    }
    public void DestroyFullPipe()
    {
        EventManager.PipeDestroyed.RemoveListener(UpdateLayerId);

        EventManager.PipeDestroyed.Invoke(sortingGroup.sortingOrder);
        Destroy(gameObject);
    }
    public int GetOrder()
    {
        return sortingGroup.sortingOrder;
    }
}
