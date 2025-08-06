using UnityEngine;
using UnityEngine.UI;

public class ItemTree : MonoBehaviour
{
    [SerializeField] Button item1;
    [SerializeField] Button item2;

    private ItemScript itemScript1;
    private ItemScript itemScript2;

    private int count1;
    private int count2;
    private static int MAXCOUNT = 5;

    private GameManager gameManager;

    void Start()
    {
        gameManager = GameManager.getInstance();

        //랜덤으로 아이템 버튼 표시
        int random = Random.Range(0, 2);
        item1.gameObject.SetActive(random == 1);
        random = Random.Range(0, 2);
        item2.gameObject.SetActive(random == 1);

        if(item1.gameObject.activeSelf || item2.gameObject.activeSelf)
        {
            itemScript1 = gameManager.Get_Random_Material();
            itemScript2 = gameManager.Get_Random_Material();

            item1.GetComponent<Image>().sprite = itemScript1.image;
            item2.GetComponent<Image>().sprite = itemScript2.image;
        }

        count1 = 0;
        count2 = 0;
    }

    public void ClickItem1()
    {
        count1++;
        if(count1 == MAXCOUNT)
        {
            count1 = 0;
            item1.gameObject.SetActive(false);
            gameManager.Add_InventoryItem(itemScript1.itemName, 1);
        }
    }

    public void ClickItem2()
    {
        count2++;
        if(count2 == MAXCOUNT)
        {
            count2 = 0;
            item2.gameObject.SetActive(false);
            gameManager.Add_InventoryItem(itemScript2.itemName, 1);
        }
    }
}
