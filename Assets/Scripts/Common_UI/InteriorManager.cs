using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InteriorManager : MonoBehaviour
{
    public bool interiorMode = false;

    public GameObject InteriorInventoryButton;
    public GameObject InteriorExitButton;
    public GameObject interiorPanel;
    public GameObject ItemButtonPrefab;
    public GameObject scrollContent;
    public GameObject tilePanel;

    private ClickInteriorItem clickInteriorItem;

    // 게임 메니저
    private GameManager gameManager;

    // 갖고 있는 인테리어 아이템 목록
    private List<(InteriorScript item, int count)> interiorItems = new List<(InteriorScript, int)>();

    public InteriorScript item;

    private Transform itemParent;
    private Vector3 spawnPos = new Vector3(-1.8f, 0.8f, 20f);

    // Start is called before the first frame update
    void Start()
    {
        clickInteriorItem = FindObjectOfType<ClickInteriorItem>();
        gameManager = GameManager.getInstance();

        //interiorItems = gameManager.Get_RoomInterior_Inventory();
        // 테스트
        interiorItems.Add((item, 1));

        itemParent = GameObject.Find("Pixels")?.transform;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void CategorySelect(string category)
    {
        switch (category)
        {
            case "가구":
                //Debug.Log("가구 category selected.");
                SetFurnitureItem();
                break;
            case "타일":
                //Debug.Log("타일 category selected.");
                SetTileItem();
                break;
            case "직원":
                //Debug.Log("직원 category selected.");
                SetEmployeeItem();
                break;
        }
    }

    public void SetFurnitureItem()
    {
        Clear();

        //GameObject ItemButton4 = Instantiate(ItemButtonPrefab, scrollContent.transform);
        foreach (var (item, count) in interiorItems)
        {
            if (item.interiorType == InteriorType.ROOM_INTERIROR)
            {
                GameObject ItemButton = Instantiate(ItemButtonPrefab, scrollContent.transform);
                // ItemButton에 item 정보 설정

                ItemButton.transform.Find("NameText").GetComponent<TextMeshProUGUI>().text = item.interiorName;  // 이름 설정
                ItemButton.transform.Find("InteriorItemImage").GetComponent<Image>().sprite = item.image;  // 아이콘 설정
                ItemButton.transform.Find("AmountText").GetComponent<TextMeshProUGUI>().text = "×" + count.ToString();  // 개수 설정

                ItemButton.GetComponent<Button>().onClick.AddListener(() => ClickInteriorItem(item));
            }
        }
    }

    public void SetTileItem()
    {
        Clear();
        GameObject ItemButton = Instantiate(ItemButtonPrefab, scrollContent.transform);
        GameObject ItemButton2 = Instantiate(ItemButtonPrefab, scrollContent.transform);
        GameObject ItemButton3 = Instantiate(ItemButtonPrefab, scrollContent.transform);
    }

    public void SetEmployeeItem()
    {
        Clear();
    }

    public void Clear()
    {
        foreach (Transform child in scrollContent.transform)
        {
            Destroy(child.gameObject);
        }
    }

    // 가구 클릭 시 
    public void ClickInteriorItem(InteriorScript item)
    {
        PanelClose();
        // 작업실위에 생성
        GameObject itemObject = Instantiate(item.prefab, spawnPos, item.prefab.transform.rotation, itemParent);
        var click = itemObject.GetComponent<ClickInteriorItem>();
        click.Select();

    }

    public void PanelOpen()
    {
        if (interiorPanel != null)
        {
            interiorPanel.SetActive(true);
            SetFurnitureItem();
        }
    }

    public void PanelClose()
    {
        if (interiorPanel != null)
        {
            interiorPanel.SetActive(false);
        }
    }

    public void ClickInteriorButton()
    {
        interiorMode = true;

        InteriorInventoryButton.SetActive(true);
        InteriorExitButton.SetActive(true);
        tilePanel.SetActive(true);
    }

    public void ClickExitInteriorButton()
    {
        interiorMode = false;

        InteriorInventoryButton.SetActive(false);
        InteriorExitButton.SetActive(false);
        tilePanel.SetActive(false);
        PanelClose();

        clickInteriorItem.ClickExitInteriorButton();
    }
}
