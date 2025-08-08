using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FabricDetailPanelController : MonoBehaviour
{
    public TextMeshProUGUI BlanketNameText;
    public Image BlanketImage;

    public List<Image> materialImageSlots; // 인스펙터에서 순서대로 연결
    public List<Text> materialQuantitySlots;

    public Sprite defaultMaterialSprite; // 기본 이미지 (인스펙터에서 연결)
    public GameManager gameManager;

    private void Start()
    {
        gameManager = GameManager.getInstance();
    }

    // RecipeEntry와 Materials가 있다고 가정
    public Sprite GetMaterialImage(RecipeEntry entry)
    {
        try
        {
            ItemScript material = gameManager.Get_Material(entry.itemName);
            return material.image;
        }
        catch (KeyNotFoundException)
        {
            Debug.LogWarning($"재료 '{entry.itemName}'이 Materials 딕셔너리에 없습니다.");
            return null;
        }
    }


    public void OpenPanel(ItemScript blanket)
    {

        gameObject.SetActive(true);

        BlanketNameText.text = blanket.itemName;
        BlanketImage.sprite = blanket.image;

        // 슬롯 초기화
        for (int i = 0; i < materialImageSlots.Count; i++)
        {
            if (i < blanket.recipe.Count && blanket.recipe[i] != null)
            {
                materialImageSlots[i].sprite = GetMaterialImage(blanket.recipe[i]);
                materialImageSlots[i].gameObject.SetActive(true);
                materialQuantitySlots[i].text = blanket.recipe[i].count.ToString();
                materialQuantitySlots[i].gameObject.SetActive(true);
            }
            else
            {
                // 슬롯은 그대로 활성화하고 기본 이미지로 표시
                materialImageSlots[i].sprite = defaultMaterialSprite;
                materialImageSlots[i].gameObject.SetActive(true);
                materialQuantitySlots[i].text = "0";
                materialQuantitySlots[i].gameObject.SetActive(true);
            }
        }
    }

    public void ClosePanel()
    {
        gameObject.SetActive(false);
    }
}
