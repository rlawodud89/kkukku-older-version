using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ItemTree : MonoBehaviour
{
    public Button item1;
    public Button item2;
    public List<Sprite> ItemSprites;

    private int count1;
    private int count2;
    private static int MAXCOUNT = 5;

    // Start is called before the first frame update
    void Start()
    {
        //랜덤으로 아이템 버튼 표시
        int random = Random.Range(0, 2);
        item1.gameObject.SetActive(random == 1);
        random = Random.Range(0, 2);
        item2.gameObject.SetActive(random == 1);

        if(item1.gameObject.activeSelf || item2.gameObject.activeSelf)
        {
            //아이템 이미지 리스트 불러오기
            Sprite[] loadedSprites = Resources.LoadAll<Sprite>("Sleeping_Garden/Objects/Items");
            ItemSprites = new List<Sprite>(loadedSprites);

            //랜덤으로 아이템 선택해서 버튼에 표시
            int randomIndex = Random.Range(0, ItemSprites.Count);
            Sprite selectedSprite = ItemSprites[randomIndex];
            item1.GetComponent<Image>().sprite = selectedSprite;
            randomIndex = Random.Range(0, ItemSprites.Count);
            selectedSprite = ItemSprites[randomIndex];
            item2.GetComponent<Image>().sprite = selectedSprite;
        }

        count1 = 0;
        count2 = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ClickItem1()
    {
        count1++;
        if(count1 == MAXCOUNT)
        {
            count1 = 0;
            item1.gameObject.SetActive(false);
        }
    }

    public void ClickItem2()
    {
        count2++;
        if(count2 == MAXCOUNT)
        {
            count2 = 0;
            item2.gameObject.SetActive(false);
        }
    }
}
