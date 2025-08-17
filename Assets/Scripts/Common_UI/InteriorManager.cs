using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class InteriorManager : MonoBehaviour
{
    public bool interiorMode = false;

    public GameObject tileButton;
    public GameObject InteriorInventoryButton;
    public GameObject InteriorExitButton;
    public GameObject roomInteriorPanel;
    public GameObject shopInteriorPanel;
    public GameObject ItemButtonPrefab;
    public GameObject roomScrollContent;
    public GameObject shopScrollContent;
    public GameObject tilePanel;

    private ClickInteriorItem clickInteriorItem;

    // 게임 메니저
    private GameManager gameManager;

    // 현재 활성화된 씬
    string currentSceneName;

    // 갖고 있는 인테리어 아이템 목록
    [HideInInspector] public List<(InteriorScript item, int count)> interiorItems = new List<(InteriorScript, int)>();

    // 테스트용
    public InteriorScript furnitureItem;
    public InteriorScript workerItem;
    public InteriorScript tileItem;


    private Transform itemParent;
    private Vector3 spawnPos = new Vector3(-1.8f, 0.8f, 20f);

    // 이동 버튼
    private GameObject Home_Button;
    private GameObject RoomBtn;

    // Start is called before the first frame update
    void Start()
    {
        clickInteriorItem = FindObjectOfType<ClickInteriorItem>();
        gameManager = GameManager.getInstance();

        // 테스트로 인벤토리에 아이템 직접 추가 
        /*
        bool isAdded=gameManager.Add_InteriorItem("나무보관함",1);

        if(isAdded)
        {
            Debug.Log("Interior item added successfully.");
        }
        else
        {
            Debug.LogWarning("Failed to add interior item.");
        }  */

        // 인벤토리 아이템 가져오기 
        interiorItems = gameManager.Get_RoomInterior_Inventory();
        // 테스트
        //interiorItems.Add((furnitureItem, 1));
        //interiorItems.Add((workerItem, 1));
        //interiorItems.Add((tileItem, 1));

        itemParent = GameObject.Find("Pixels")?.transform;
    }

    // Update is called once per frame
    void Update()
    {
        currentSceneName = SceneManager.GetActiveScene().name;
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
                GameObject ItemButton = Instantiate(ItemButtonPrefab, roomScrollContent.transform);
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

        foreach (var (item, count) in interiorItems)
        {
            if (item.interiorType == InteriorType.FLOOR_TILE || item.interiorType == InteriorType.WALL_TILE)
            {
                GameObject ItemButton = Instantiate(ItemButtonPrefab, roomScrollContent.transform);
                // ItemButton에 item 정보 설정

                ItemButton.transform.Find("NameText").GetComponent<TextMeshProUGUI>().text = item.interiorName;  // 이름 설정
                ItemButton.transform.Find("InteriorItemImage").GetComponent<Image>().sprite = item.image;  // 아이콘 설정
                ItemButton.transform.Find("AmountText").GetComponent<TextMeshProUGUI>().text = "×" + count.ToString();  // 개수 설정

                ItemButton.GetComponent<Button>().onClick.AddListener(() => ClickTileItem(item));
            }
        }
    }

    public void SetEmployeeItem()
    {
        Clear();

        foreach (var (item, count) in interiorItems)
        {
            if (item.interiorType == InteriorType.WORKER)
            {
                GameObject ItemButton = Instantiate(ItemButtonPrefab, roomScrollContent.transform);
                // ItemButton에 item 정보 설정

                ItemButton.transform.Find("NameText").GetComponent<TextMeshProUGUI>().text = item.interiorName;  // 이름 설정
                ItemButton.transform.Find("InteriorItemImage").GetComponent<Image>().sprite = item.image;  // 아이콘 설정
                ItemButton.transform.Find("AmountText").GetComponent<TextMeshProUGUI>().text = "×" + count.ToString();  // 개수 설정

                // ItemButton.GetComponent<Button>().onClick.AddListener(() => ClickInteriorItem(item));
            }
        }
    }

    public void Clear()
    {
        foreach (Transform child in roomScrollContent.transform)
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
        click.initialPosition = spawnPos;

        // 인벤토리 -> 작업실로
        bool isUsed = gameManager.Use_RoomInteriorItem(item.name, spawnPos.x, spawnPos.y);

        if (isUsed)
        {
            Debug.Log($"Used Interior Item: {item.name}");
        }
        else
        {
            Debug.LogWarning($"Failed to use Interior Item: {item.name}");
        }

        // 인벤토리 아이템 다시 얻어오기 
        interiorItems = gameManager.Get_RoomInterior_Inventory();

    }

    // 타일 클릭 시
    public void ClickTileItem(InteriorScript item)
    {
        PanelClose();

        // 타일 디자인 바꾸기
    }

    public void PanelOpen()
    {
        if (currentSceneName == "Work_Room")
        {
            if (roomInteriorPanel != null)
            {
                roomInteriorPanel.SetActive(true);
                // 다른 버튼들 안보이게
                InteriorInventoryButton.SetActive(false);
                InteriorExitButton.SetActive(false);
                tileButton.SetActive(false);

                SetFurnitureItem();
            }
        }
        else if (currentSceneName == "Work_Shop")
        {
            if (shopInteriorPanel != null)
            {
                shopInteriorPanel.SetActive(true);
            }
        }
    }

    public void PanelClose()
    {
        if (currentSceneName == "Work_Room")
        {
            if (roomInteriorPanel != null)
            {
                roomInteriorPanel.SetActive(false);

                // 다른 버튼들 안보이게
                InteriorInventoryButton.SetActive(true);
                InteriorExitButton.SetActive(true);
                tileButton.SetActive(true);
            }
        }
        else if (currentSceneName == "Work_Shop")
        {
            if (shopInteriorPanel != null)
            {
                shopInteriorPanel.SetActive(false);
            }
        }
    }

    public void TilePanelOpen(){
        tilePanel.SetActive(true);

        // 다른 버튼들 안보이게
        InteriorInventoryButton.SetActive(false);
        InteriorExitButton.SetActive(false);
        tileButton.SetActive(false);
    }

    public void TilePanelClose(){
        tilePanel.SetActive(false);

        // 다른 버튼들 보이게
        InteriorInventoryButton.SetActive(true);
        InteriorExitButton.SetActive(true);
        tileButton.SetActive(true);
    }

    // 인테리어 메뉴 버튼 눌렀을 때
    public void ClickInteriorButton()
    {
        interiorMode = true;

        tileButton.SetActive(true);
        InteriorInventoryButton.SetActive(true);
        InteriorExitButton.SetActive(true);

        if (currentSceneName == "Work_Room")
        {
            Home_Button = GameObject.Find("Home_Button");
            Home_Button.SetActive(false);

        }
        else if (currentSceneName == "Work_Shop")
        {
            RoomBtn = GameObject.Find("RoomBtn");
            RoomBtn.SetActive(false);
        }

        //tilePanel.SetActive(true);
    }

    // 나가기 버튼 눌렀을 때
    public void ClickExitInteriorButton()
    {
        interiorMode = false;

        tileButton.SetActive(false);
        InteriorInventoryButton.SetActive(false);
        InteriorExitButton.SetActive(false);

        PanelClose();

        if (Home_Button != null)
        {
            Home_Button.SetActive(true);
        }
        else if (RoomBtn != null)
        {
            RoomBtn.SetActive(true);
        }

        tilePanel.SetActive(false);
    }
}
