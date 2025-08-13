using UnityEngine;
using UnityEngine.UI;

public class TwoTabScrollController : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] Button shopButton;       // 가게 버튼
    [SerializeField] Button workshopButton;   // 작업실/디자인 버튼
    [SerializeField] Button tileButton;       // 타일 버튼

    [Header("ScrollRect & Contents")]
    [SerializeField] ScrollRect scrollRect;
    [SerializeField] GameObject shopContent;
    [SerializeField] GameObject workshopContent;
    [SerializeField] GameObject tileContent;

    // 옵션: 켜질 때 항상 Shop으로 리셋할지 / 스크롤 맨 위로 보낼지
    [SerializeField] bool resetToShopOnEnable = true;
    [SerializeField] bool scrollToTopOnEnable = true;

    void Awake()
    {
        shopButton.onClick.AddListener(ShowShop);
        workshopButton.onClick.AddListener(ShowWorkshop);
        tileButton.onClick.AddListener(ShowTileShop);
    }

    // 비활성→활성 될 때마다 호출됨
    void OnEnable()
    {
        if (resetToShopOnEnable)
            ShowShop();                 // ← 항상 가게 먼저

        if (scrollToTopOnEnable && scrollRect)
            scrollRect.verticalNormalizedPosition = 1f;
    }

    // Start는 이제 필요 없음(처음에도 OnEnable이 호출됨).
    // 남겨두고 싶다면 빈 메서드로 두세요.
    // void Start() {}

    void ShowShop()
    {
        if (!shopContent || !workshopContent || !tileContent) return;

        shopContent.SetActive(true);
        workshopContent.SetActive(false);
        tileContent.SetActive(false);
        SetScrollContent(shopContent);
    }

    void ShowWorkshop()
    {
        if (!shopContent || !workshopContent || !tileContent) return;

        workshopContent.SetActive(true);
        shopContent.SetActive(false);
        tileContent.SetActive(false);
        SetScrollContent(workshopContent);
    }

    void ShowTileShop()
    {
        if (!shopContent || !workshopContent || !tileContent) return;

        tileContent.SetActive(true);
        shopContent.SetActive(false);
        workshopContent.SetActive(false);
        SetScrollContent(tileContent);
    }

    void SetScrollContent(GameObject go)
    {
        var rt = go.GetComponent<RectTransform>();
        if (!rt || !scrollRect) return;

        scrollRect.content = rt;

        // 레이아웃 갱신 후 스크롤 맨 위로
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(rt);
        scrollRect.verticalNormalizedPosition = 1f;
    }
}
