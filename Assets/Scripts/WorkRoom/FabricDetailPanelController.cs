using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FabricDetailPanelController : MonoBehaviour
{
    public Text fabricNameText;
    public Image fabricImage;

    public List<Image> materialImageSlots; // 인스펙터에서 순서대로 연결
    public List<Text> materialQuantitySlots;

    public Sprite defaultMaterialSprite; // 기본 이미지 (인스펙터에서 연결)

    public void OpenPanel(FabricData fabric)
    {

        gameObject.SetActive(true);

        fabricNameText.text = fabric.fabricName;
        fabricImage.sprite = fabric.fabricSprite;

        // 슬롯 초기화
        for (int i = 0; i < materialImageSlots.Count; i++)
        {
            if (i < fabric.requiredMaterials.Length && fabric.requiredMaterials[i] != null)
            {
                materialImageSlots[i].sprite = fabric.requiredMaterials[i].materialIcon;
                materialImageSlots[i].gameObject.SetActive(true);
                materialQuantitySlots[i].text = fabric.requiredMaterials[i].materialquantity;
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
