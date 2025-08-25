using UnityEngine;
using TMPro;

public class ImageToolTip : MonoBehaviour
{
    private GameObject tooltipText;

    void Start()
    {
        if (transform.parent.name == "Employee1(Clone)")
        {
            // 2. 부모 오브젝트의 자식 중에서 'TooltipText'라는 이름을 가진 오브젝트를 찾습니다.
            Transform tooltipTransform = transform.parent.Find("TooltipText");
            if (tooltipTransform != null)
            {
                tooltipText = tooltipTransform.gameObject;
                tooltipText.SetActive(false); // 처음에는 비활성화
            }
            else
            {
                Debug.LogWarning("TooltipText 오브젝트를 찾을 수 없습니다. 이름이 올바른지 확인하세요.");
            }
        }
    }

    // 마우스가 콜라이더 위로 올라왔을 때 호출됩니다.
    void OnMouseEnter()
    {
        // 툴팁 오브젝트가 존재할 때만 기능을 실행합니다.
        if (tooltipText != null)
        {
            tooltipText.SetActive(true);
        }
    }

    // 마우스가 콜라이더 밖으로 나갔을 때 호출됩니다.
    void OnMouseExit()
    {
        if (tooltipText != null)
        {
            tooltipText.SetActive(false);
        }
    }
}