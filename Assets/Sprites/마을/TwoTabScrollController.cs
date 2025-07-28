using UnityEngine;
using UnityEngine.UI;

public class TwoTabScrollController : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] Button shopButton;       // 가게 버튼
    [SerializeField] Button workshopButton;   // 작업실 가구 디자인 버튼

    [Header("ScrollRect & Contents")]
    [SerializeField] ScrollRect scrollRect;
    [SerializeField] GameObject shopContent;
    [SerializeField] GameObject workshopContent;

    void Awake()
    {
        shopButton.onClick.AddListener(ShowShop);
        workshopButton.onClick.AddListener(ShowWorkshop);
    }

    void Start()
    {
        // 처음엔 가게 표시 (원하면 Workshop으로 바꿔도 됨)
        ShowShop();
    }

    void ShowShop()
    {
        shopContent.SetActive(true);
        workshopContent.SetActive(false);

        SetScrollContent(shopContent);
    }

    void ShowWorkshop()
    {
        workshopContent.SetActive(true);
        shopContent.SetActive(false);

        SetScrollContent(workshopContent);
    }

    void SetScrollContent(GameObject go)
    {
        // ScrollRect가 현재 활성 컨텐츠를 바라보도록 교체
        var rt = go.GetComponent<RectTransform>();
        scrollRect.content = rt;

        // 레이아웃 강제 갱신 후 맨 위로
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(rt);
        scrollRect.verticalNormalizedPosition = 1f;
    }
}

