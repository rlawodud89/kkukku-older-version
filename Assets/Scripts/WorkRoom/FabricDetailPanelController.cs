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

    public void OpenPanel(BlanketData blanket)
    {

        gameObject.SetActive(true);

        BlanketNameText.text = blanket.BlanketName;
        BlanketImage.sprite = blanket.BlanketSprite;

        // 슬롯 초기화
        for (int i = 0; i < materialImageSlots.Count; i++)
        {
            if (i < blanket.requiredMaterials.Length && blanket.requiredMaterials[i] != null)
            {
                materialImageSlots[i].sprite = blanket.requiredMaterials[i].data.MaterialSprite;
                materialImageSlots[i].gameObject.SetActive(true);
                materialQuantitySlots[i].text = blanket.requiredMaterials[i].count.ToString();
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
