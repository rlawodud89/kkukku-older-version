using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpecialPanel : MonoBehaviour
{
    public List<SpecialMaterialUISlot> slots;
    public Button resultBtn;
    public GameObject CompletePanel;
    public Image CompleteImage;
    public TextMeshProUGUI CompleteText;

    private GameManager gameManager;
    private ItemScript CanBlanket;
    private List<ItemScript> specialBlankets;

    void Start()
    {
        gameManager = GameManager.getInstance();
        resultBtn.gameObject.SetActive(false);

        specialBlankets = gameManager.Get_Special_Blankets();

        foreach (var slot in slots)
        {
            slot.OnSlotChanged += SlotCheck;
        }

        resultBtn.onClick.RemoveAllListeners();
        resultBtn.onClick.AddListener(ClickResultBtn);
    }

    void SlotCheck()
    {
        if (!isSlotFull()) return;

        CanBlanket = CheckRecipe();
        if (CanBlanket != null)
        {
            resultBtn.image.sprite = CanBlanket.image;
            resultBtn.gameObject.SetActive(true);
        }

        foreach (var slot in slots)
        {
            slot.ClearData();
        }
    }

    void ClickResultBtn()
    {
        gameManager.Add_InventoryItem(CanBlanket.itemName, 1);
        resultBtn.gameObject.SetActive(false);
        gameObject.SetActive(false);

        CompletePanel.SetActive(true);
        CompleteImage.sprite = CanBlanket.image;
        CompleteText.text = CanBlanket.itemName + "이 완성되었습니다!";
    }

    public void ClickCompleteBtn()
    {
        CompletePanel.SetActive(false);
    }

    private bool isSlotFull()
    {
        foreach (var slot in slots)
        {
            if (slot.item == null) return false;
        }

        return true;
    }

    private ItemScript CheckRecipe()
    {
        foreach (var specialBlanket in specialBlankets)
        {
            // recipe에 있는 재료 하나씩 확인
            foreach (var recipeItem in specialBlanket.recipe)
            {
                var invItem = slots.Find(x => x.item.itemName == recipeItem.itemName);

                // 없거나 개수가 부족하면 false
                if (invItem == null || invItem.count < recipeItem.count)
                {
                    return null;
                }
            }

            return specialBlanket; // 모든 재료 충분
        }

        return null;
    }
}
