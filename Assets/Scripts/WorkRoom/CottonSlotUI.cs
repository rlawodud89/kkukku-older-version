using UnityEngine;
using UnityEngine.UI;

public class CottonSlotUI : MonoBehaviour
{
    public Button button;
    public Text countText;

    private BlanketData currentData;

    public bool HasData(BlanketData data)
    {
        return currentData == data;
    }

    public bool HasAnyData()
    {
        return currentData != null;
    }

    public void SetData(BlanketData data)
    {
        if (data == null) return;

        if (currentData == data)
        {
            // 같은 데이터면 count만 증가
            currentData.FabricCount += 1;
            Debug.Log($"{currentData.BlanketName} 원단 수량 증가: {currentData.FabricCount}");
        }
        else
        {
            // 다른 데이터면 교체하고 초기 count 유지 또는 1로 초기화
            currentData = data;
            currentData.FabricCount = Mathf.Max(currentData.FabricCount, 1); // 최소 1로 세팅
            if (button != null)
                button.image.sprite = currentData.Fabric;

            Debug.Log($"새 데이터 세팅: {currentData.BlanketName} 수량: {currentData.FabricCount}");
        }

        UpdateCountText();
    }


    public void ClearSlot()
    {
        currentData = null;
        if (button != null)
            button.image.sprite = null;

        if (countText != null)
            countText.text = "";

        // 버튼 클릭 리스너는 필요시 따로 관리
    }

    private void UpdateCountText()
    {
        if (countText != null && currentData != null)
        {
            countText.text = currentData.FabricCount.ToString();
        }
        else if (countText != null)
        {
            countText.text = "";
        }
    }
}
