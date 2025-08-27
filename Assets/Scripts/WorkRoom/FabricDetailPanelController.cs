using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FabricDetailPanelController : MonoBehaviour
{
    public TextMeshProUGUI BlanketNameText;
    public Image BlanketImage;

    public List<Image> materialImageSlots; // 인스펙터에서 순서대로 연결
    public List<TextMeshProUGUI> materialQuantitySlots;
    public List<TextMeshProUGUI> inv_materialSlots;

    public Sprite defaultMaterialSprite; // 기본 이미지 (인스펙터에서 연결)
    private GameManager gameManager;


    private void Start()
    {
        if (gameManager == null)
        {
            gameManager = GameManager.getInstance();
        }
    }


    public void OpenPanel(ItemScript blanket)
    {

        if (gameManager == null)
        {
            gameManager = GameManager.getInstance();
        }
        gameObject.SetActive(true);

        BlanketNameText.text = blanket.itemName;
        BlanketImage.sprite = blanket.image;

        // 슬롯 초기화
        for (int i = 0; i < materialImageSlots.Count; i++)
        {
            if (i < blanket.recipe.Count && blanket.recipe[i] != null)
            {
                materialImageSlots[i].sprite = gameManager.GetMaterialImage(blanket.recipe[i]);
                materialImageSlots[i].gameObject.SetActive(true);
                materialQuantitySlots[i].text = blanket.recipe[i].count.ToString();
                materialQuantitySlots[i].gameObject.SetActive(true);

                var inv = gameManager.Get_Material_Inventory(); // 전체 인벤토리
                var invItem = inv.Find(x => x.item.itemName == blanket.recipe[i].itemName); // 레시피 재료 이름 매칭

                int haveCount = invItem.item != null ? invItem.count : 0; // 없으면 0개
                inv_materialSlots[i].text = blanket.recipe[i].count == 0 ? "" : haveCount.ToString();

                if (blanket.recipe[i].count == 0)
                {
                    // 레시피에 필요 없는 재료 → 기본색
                    inv_materialSlots[i].color = Color.black;
                }
                else if (haveCount < blanket.recipe[i].count)
                {
                    // 필요한데 부족함 → 빨간색
                    inv_materialSlots[i].color = Color.red;
                }
                else
                {
                    // 충분함 → 기본색
                    inv_materialSlots[i].color = Color.black;
                }

            }
            else
            {
                // 슬롯은 그대로 활성화하고 기본 이미지로 표시
                materialImageSlots[i].sprite = defaultMaterialSprite;
                materialImageSlots[i].gameObject.SetActive(true);
                materialQuantitySlots[i].text = "";
                materialQuantitySlots[i].gameObject.SetActive(true);
            }
        }
    }

    public void ClosePanel()
    {
        gameObject.SetActive(false);
    }

    public void ResetAllSlots()
    {
        BlanketNameText.text = " ";
        BlanketImage.sprite = defaultMaterialSprite;

        for (int i = 0; i < materialImageSlots.Count; i++)
        {
            if (materialImageSlots[i] != null)
            {
                materialImageSlots[i].sprite = defaultMaterialSprite;
                materialImageSlots[i].gameObject.SetActive(true);
            }

            if (materialQuantitySlots.Count > i && materialQuantitySlots[i] != null)
            {
                materialQuantitySlots[i].text = "";
                materialQuantitySlots[i].gameObject.SetActive(true);
            }

            if (inv_materialSlots.Count > i && inv_materialSlots[i] != null)
            {
                inv_materialSlots[i].text = "";
            }
        }
    }

}
