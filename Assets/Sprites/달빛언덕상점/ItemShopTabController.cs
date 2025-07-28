using UnityEngine;
using UnityEngine.UI;

public class ItemShopTabController : MonoBehaviour
{
    [SerializeField] ScrollRect scrollRect;   // ← ScrollView Drag

    [Header("Top‑level 탭 버튼")]
    [SerializeField] Button designTabBtn;
    [SerializeField] Button materialTabBtn;

    [Header("Material 서브 탭 버튼")]
    [SerializeField] GameObject materialSubTabBar; // Yarn/Cotton/Deco 버튼 묶음
    [SerializeField] Button yarnBtn;
    [SerializeField] Button cottonBtn;
    [SerializeField] Button decoBtn;

    [Header("Content Panels")]
    [SerializeField] GameObject designContent;
    [SerializeField] GameObject yarnContent;
    [SerializeField] GameObject cottonContent;
    [SerializeField] GameObject decoContent;

    void Awake()
    {
        // ── 탑 탭 ──
        designTabBtn.onClick.AddListener(ShowDesignTab);
        materialTabBtn.onClick.AddListener(ShowMaterialTab);

        // ── 서브 탭 ──
        yarnBtn.onClick.AddListener(() => ShowMaterialSub(yarnContent));
        cottonBtn.onClick.AddListener(() => ShowMaterialSub(cottonContent));
        decoBtn.onClick.AddListener(() => ShowMaterialSub(decoContent));
    }

    void Start()
    {
        ShowMaterialTab();
        ShowMaterialSub(yarnContent); // 기본: 실
    }

    void ShowDesignTab()
    {
        materialSubTabBar.SetActive(false);
        designContent.SetActive(true);
        yarnContent.SetActive(false);
        cottonContent.SetActive(false);
        decoContent.SetActive(false);

        SetScrollContent(designContent);
    }

    void ShowMaterialTab()
    {
        materialSubTabBar.SetActive(true);
        designContent.SetActive(false);

        // 재료 패널 중 아무것도 안 켜져 있으면 실(Yarn) 기본 선택
        if (!yarnContent.activeSelf && !cottonContent.activeSelf && !decoContent.activeSelf)
        {
            ShowMaterialSub(yarnContent);  // ← 기본 패널 다시 켬
        }
        else
        {
            // 이미 켜져 있는 것이 있으면 ScrollRect.content 갱신만
            if (yarnContent.activeSelf) SetScrollContent(yarnContent);
            else if (cottonContent.activeSelf) SetScrollContent(cottonContent);
            else if (decoContent.activeSelf) SetScrollContent(decoContent);
        }
    }


    void ShowMaterialSub(GameObject target)
    {
        yarnContent.SetActive(target == yarnContent);
        cottonContent.SetActive(target == cottonContent);
        decoContent.SetActive(target == decoContent);

        SetScrollContent(target);
    }

    void SetScrollContent(GameObject go)
    {
        var rt = go.GetComponent<RectTransform>();
        scrollRect.content = rt;

        // 레이아웃 갱신 후 맨 위로
        LayoutRebuilder.ForceRebuildLayoutImmediate(rt);
        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 1f;
    }
}
