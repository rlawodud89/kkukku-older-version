using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BlanketButtonPanel : MonoBehaviour
{
    public Transform ScrollContent;       // ScrollView > Viewport > Content
    public Button btnPrefab;
    public FabricDetailPanelController detailPanel;
    public StoragePanel storagePanel;
    public Make_Fabric makeFabric;


    void Start()
    {
        if (storagePanel == null)
        {
            storagePanel = FindObjectOfType<StoragePanel>();
        }

        if (makeFabric == null)
        {
            makeFabric = FindObjectOfType<Make_Fabric>();
        }

        storagePanel.InitScroll();
        InitScroll();
    }

    void InitScroll()
    {
        List<ItemScript> blanketList = BlanketManager.Instance.blanketList;

        if (blanketList == null)
        {
            Debug.LogError("Blanket list is null!");
            return;
        }

        int childCount = storagePanel.ScrollContent.childCount;

        for (int i = 0; i < blanketList.Count; i++)
        {
            if (i >= childCount)
            {
                Debug.LogWarning($"슬롯 부족: 필요한 {blanketList.Count}, 존재하는 {childCount}");
                break;
            }

            int index = i;
            ItemScript data = blanketList[index];

            Transform slot = storagePanel.ScrollContent.GetChild(index);

            Button btn = slot.GetComponentInChildren<Button>();
            Image btnImage = btn?.GetComponent<Image>();

            if (btnImage == null) Debug.LogError($"슬롯[{index}]에 이미지가 없습니다.");
            if (data == null) Debug.LogError($"blanketList[{index}] is null");

            if (btnImage != null)
                btnImage.sprite = data.image;

            if (btn != null)
            {
                btn.onClick.RemoveAllListeners();
                int capturedIndex = index;
                ItemScript capturedData = data;

                btn.onClick.AddListener(() =>
                {

                    if (makeFabric.isMaking)
                    {
                        Debug.Log("제작 중이므로 다른 블랭킷 선택 불가");
                        return;
                    }

                    if (makeFabric != null)
                    {
                        makeFabric.currentBlanket = capturedData;
                        Debug.Log($"makeFabric.currentBlanket 설정됨: {capturedData.name}");
                    }

                    if (detailPanel.gameObject.activeSelf)
                    {
                        // 패널이 이미 열려 있으면 기존 슬롯 초기화 후 새 데이터 갱신
                        detailPanel.ResetAllSlots();
                    }

                    detailPanel.OpenPanel(capturedData);
                });


            }
        }
    }
}
