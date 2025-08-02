using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoragePanel : MonoBehaviour
{
    public Transform ScrollContent;     // ScrollView > Viewport > Content
    public GameObject ItemPrefab;  // 그냥 Image + 자식 Image 프리팹

    public int itemCount = 20;


    void Start()
    {
        InitScroll();
    }

    public void InitScroll()
    {
        for (int i = 0; i < itemCount; i++)
        {
            GameObject item = Instantiate(ItemPrefab, ScrollContent);
            item.name = $"Slots_{i + 1}";
        }
    }
}
