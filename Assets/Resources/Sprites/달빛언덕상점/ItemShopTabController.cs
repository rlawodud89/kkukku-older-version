using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemShopTabController : MonoBehaviour
{
    [Header("Scroll (오른쪽 스크롤 뷰)")]
    [SerializeField] ScrollRect scrollRect;

    [Header("Buttons (스크롤 바깥 패널)")]
    [SerializeField] Button designButton;    // 디자인 버튼
    [SerializeField] Button itemButton;      // 아이템(아코디언 토글) 버튼

    [Header("Item SubPanel (아코디언: 실/솜/데코 묶음)")]
    [SerializeField] RectTransform itemSubPanel; // sizeDelta.y로 높이 변경
    [SerializeField] CanvasGroup itemSubPanelCg; // 없으면 자동 추가
    [SerializeField] float accordionDuration = 0.25f;
    [SerializeField] AnimationCurve accordionEase = null; // 비우면 EaseInOut
    [SerializeField] float spacingBelowSub = 8f;          // SubPanel과 디자인 버튼 간격

    [Tooltip("완전히 펼쳤을 때 SubPanel의 목표 높이(레이아웃 없으면 이 값 사용)")]
    [SerializeField] float expandedHeight = 180f;

    [Tooltip("가능하면 SubPanel의 자연 높이를 자동 측정")]
    [SerializeField] bool autoMeasureExpandedHeight = true;

    [Header("Sub Buttons (SubPanel 내부)")]
    [SerializeField] Button yarnBtn;
    [SerializeField] Button cottonBtn;
    [SerializeField] Button decoBtn;

    [Header("Content Panels (스크롤 콘텐츠 루트)")]
    [SerializeField] GameObject designContent;
    [SerializeField] GameObject yarnContent;
    [SerializeField] GameObject cottonContent;
    [SerializeField] GameObject decoContent;

    // --- 자식(서브버튼) 스태거/슬라이드 설정 ---
    [Header("Child Stagger / Slide (세로만)")]
    [SerializeField] float childShowSlideY = 14f;   // 보일 때: 위(+Y)에서 아래(원위치)로
    [SerializeField] float childHideSlideY = 16f;   // 숨길 때: 아래(+Y)→위(원위치)로
    [SerializeField] float childFadeTime = 0.18f; // 각 아이템 페이드/슬라이드 시간
    [SerializeField] float childStagger = 0.035f;// 아이템 간 지연
    [SerializeField] AnimationCurve childEase = null; // 비우면 Linear

    [Tooltip("프로젝트 좌표계에 따라 '위'가 +Y인지 -Y인지 토글")]
    [SerializeField] bool yUpIsPositive = true;

    [Tooltip("애니메이션 동안 X(가로)는 원래 위치로 고정")]
    [SerializeField] bool lockXDuringAnim = true;

    [SerializeField] bool resetToYarnOnEnable = true;      // 켜질 때 항상 '실'로
    [SerializeField] bool openAccordionOnEnable = true;    // 켜질 때 서브 패널 펼칠지
    [SerializeField] bool animateOnEnable = false;         // 켜질 때 애니메이션 사용할지(=false면 즉시 상태 세팅)


    // --- 내부 상태 ---
    RectTransform designButtonRT;
    bool itemExpanded = false;
    Coroutine accordionCo, childCo;
    float designBtnBaseY;
    float subPanelTargetH;
    GameObject _lastMaterial;

    // 서브버튼 캐시(원래 위치: localPosition 기반)
    readonly List<RectTransform> subItems = new List<RectTransform>();
    readonly Dictionary<RectTransform, Vector2> baseLocalPos = new Dictionary<RectTransform, Vector2>();

    float Y(float v) => yUpIsPositive ? v : -v;

    void OnEnable()
    {
        // 패널이 다시 켜질 때마다 기본 상태로
        if (resetToYarnOnEnable)
        {
            // 캐시가 비었거나 자식 수가 바뀌었을 수 있으니 재빌드
            if (subItems.Count == 0 || subItems.Count != itemSubPanel.childCount)
                BuildSubItemCache();

            _lastMaterial = yarnContent;
            ShowOnly(yarnContent);
            SetScrollContent(yarnContent);

            if (openAccordionOnEnable)
            {
                if (animateOnEnable) SetItemAccordion(true);
                else SetItemAccordionInstant(true);   // 즉시 펼침
            }
            else
            {
                if (animateOnEnable) SetItemAccordion(false);
                else SetItemAccordionInstant(false);  // 즉시 접힘
            }
        }
    }

    void OnDisable()
    {
        // 진행 중 코루틴 정리(깜빡임 방지)
        if (accordionCo != null) { StopCoroutine(accordionCo); accordionCo = null; }
        if (childCo != null) { StopCoroutine(childCo); childCo = null; }
    }
    // 아코디언을 애니 없이 즉시 상태로
    void SetItemAccordionInstant(bool expand)
    {
        itemExpanded = expand;
        itemSubPanel.gameObject.SetActive(expand);

        float h = expand ? subPanelTargetH : 0f;
        float a = expand ? 1f : 0f;

        SetSubPanelVisual(h, a);
        MoveDesignBy(h);

        // 자식 버튼들의 위치/알파도 즉시 정리
        InstantSetChildren(expand);
    }

    // 자식 버튼들 즉시 세팅(위→아래 슬라이드 컨셉 유지)
    void InstantSetChildren(bool expanded)
    {
        if (subItems.Count == 0) BuildSubItemCache();

        float showDy = Y(childShowSlideY);
        float hideDy = Y(childHideSlideY);

        for (int i = 0; i < subItems.Count; i++)
        {
            var rt = subItems[i];
            var basePos = baseLocalPos[rt];
            var cg = rt.GetComponent<CanvasGroup>();

            if (expanded)
            {
                // 펼친 기본 화면: 원래 위치 + 불투명
                rt.localPosition = new Vector3(basePos.x, basePos.y, rt.localPosition.z);
                if (cg) cg.alpha = 1f;
            }
            else
            {
                // 접힌 기본 화면: 위로 약간 올려두고 투명(다음에 내려오게)
                rt.localPosition = new Vector3(basePos.x, basePos.y + hideDy, rt.localPosition.z);
                if (cg) cg.alpha = 0f;
            }
        }
    }
    void Awake()
    {
        if (accordionEase == null) accordionEase = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
        if (childEase == null) childEase = AnimationCurve.Linear(0f, 0f, 1f, 1f);

        if (!itemSubPanelCg)
        {
            itemSubPanelCg = itemSubPanel.GetComponent<CanvasGroup>();
            if (!itemSubPanelCg) itemSubPanelCg = itemSubPanel.gameObject.AddComponent<CanvasGroup>();
        }

        // 시작: SubPanel 접힘
        SetSubPanelVisual(0f, 0f);
        itemSubPanel.gameObject.SetActive(false);

        // 디자인 버튼 기준 위치(여긴 anchoredPosition 유지)
        designButtonRT = designButton.GetComponent<RectTransform>();
        designBtnBaseY = designButtonRT.anchoredPosition.y;

        // 서브 아이템 원래 위치 캐시 (localPosition 사용)
        BuildSubItemCache();

        // 목표 높이 결정(가능하면 자동 측정)
        subPanelTargetH = autoMeasureExpandedHeight ? MeasureSubPanelHeightSafe() : expandedHeight;
        if (subPanelTargetH <= 0f) subPanelTargetH = expandedHeight;

        // 버튼 바인딩(인스펙터 OnClick 비우기)
        designButton.onClick.AddListener(ShowDesignContent);
        itemButton.onClick.AddListener(ToggleItemAccordion);
        yarnBtn.onClick.AddListener(() => ShowMaterialSub(yarnContent));
        cottonBtn.onClick.AddListener(() => ShowMaterialSub(cottonContent));
        decoBtn.onClick.AddListener(() => ShowMaterialSub(decoContent));
    }

    void Start()
    {
        _lastMaterial = yarnContent;    // 기본 재료
        ShowMaterialSub(_lastMaterial); // 기본 콘텐츠
        // SetItemAccordion(true);      // 시작부터 펼치려면
    }

    // ===== 아코디언 토글 =====
    void ToggleItemAccordion()
    {
        bool next = !itemExpanded;
        SetItemAccordion(next);
        if (next && _lastMaterial) ShowMaterialSub(_lastMaterial); // 겹침 방지
    }

    public void SetItemAccordion(bool expand)
    {
        if (accordionCo != null) StopCoroutine(accordionCo);
        accordionCo = StartCoroutine(Co_Accordion(expand));
    }

    IEnumerator Co_Accordion(bool expand)
    {
        itemExpanded = expand;
        itemSubPanel.gameObject.SetActive(true);

        // 자식 스태거 시작(보일 때: 위→아래, 숨길 때: 아래→위)
        if (childCo != null) StopCoroutine(childCo);
        childCo = StartCoroutine(Co_RevealChildren(expand));

        float fromH = itemSubPanel.sizeDelta.y;
        float toH = expand ? subPanelTargetH : 0f;

        float fromA = itemSubPanelCg.alpha;
        float toA = expand ? 1f : 0f;

        float t = 0f;
        while (t < accordionDuration)
        {
            t += Time.unscaledDeltaTime;
            float k = accordionEase.Evaluate(Mathf.Clamp01(t / accordionDuration));

            float h = Mathf.Lerp(fromH, toH, k);
            float a = Mathf.Lerp(fromA, toA, k);

            SetSubPanelVisual(h, a);
            MoveDesignBy(h);
            yield return null;
        }

        SetSubPanelVisual(toH, toA);
        MoveDesignBy(toH);

        // 닫을 때 자식 애니가 남아있으면 마저 기다림
        if (!expand && childCo != null) yield return childCo;

        if (!expand) itemSubPanel.gameObject.SetActive(false);
        accordionCo = null;
    }

    // SubPanel 높이/알파 적용(sizeDelta.y 사용)
    void SetSubPanelVisual(float height, float alpha)
    {
        var sd = itemSubPanel.sizeDelta;
        itemSubPanel.sizeDelta = new Vector2(sd.x, height);
        itemSubPanelCg.alpha = alpha;
    }

    // SubPanel 높이에 비례해 디자인 버튼을 아래로 이동(여긴 anchoredPosition 계속 사용)
    void MoveDesignBy(float subHeight)
    {
        designButtonRT.anchoredPosition = new Vector2(
            designButtonRT.anchoredPosition.x,
            designBtnBaseY - (subHeight + spacingBelowSub)
        );
    }

    // ===== 자식 스태거(원래 위치: localPosition 기반, 세로 슬라이드만) =====
    void BuildSubItemCache()
    {
        subItems.Clear();
        baseLocalPos.Clear();

        for (int i = 0; i < itemSubPanel.childCount; i++)
        {
            var rt = itemSubPanel.GetChild(i) as RectTransform;
            if (!rt || !rt.gameObject.activeSelf) continue;

            subItems.Add(rt);
            baseLocalPos[rt] = rt.localPosition; // ★ 원래 위치 = localPosition

            var cg = rt.GetComponent<CanvasGroup>();
            if (!cg) rt.gameObject.AddComponent<CanvasGroup>();
        }
    }

    IEnumerator Co_RevealChildren(bool show)
    {
        if (subItems.Count == 0) BuildSubItemCache();

        if (show)
        {
            // 보일 때: 각 버튼을 "원래 위치보다 위"에서 0 알파로 대기
            float dy = Y(childShowSlideY);

            foreach (var rt in subItems)
            {
                Vector2 basePos = baseLocalPos[rt];
                rt.localPosition = new Vector3(basePos.x, basePos.y + dy, rt.localPosition.z);
                rt.GetComponent<CanvasGroup>().alpha = 0f;
            }

            // 위→아래 순서로 하나씩 내려오며 페이드인
            for (int i = 0; i < subItems.Count; i++)
            {
                var rt = subItems[i];
                var cg = rt.GetComponent<CanvasGroup>();

                Vector2 basePos = baseLocalPos[rt];
                Vector2 fromPos = new Vector2(basePos.x, basePos.y + dy);
                Vector2 toPos = basePos;

                StartCoroutine(Co_TweenChildLocal(rt, cg, fromPos, toPos, 0f, 1f, childFadeTime));
                yield return new WaitForSeconds(childStagger);
            }
        }
        else
        {
            // 숨길 때: 아래→위 순서로 "위로" 올리며 페이드아웃
            float dy = Y(childHideSlideY);

            for (int i = subItems.Count - 1; i >= 0; i--)
            {
                var rt = subItems[i];
                var cg = rt.GetComponent<CanvasGroup>();

                Vector2 basePos = baseLocalPos[rt];
                Vector2 fromPos = basePos;
                Vector2 toPos = new Vector2(basePos.x, basePos.y + dy); // 위로

                StartCoroutine(Co_TweenChildLocal(rt, cg, fromPos, toPos, 1f, 0f, childFadeTime));
                yield return new WaitForSeconds(childStagger);
            }
        }
    }

    IEnumerator Co_TweenChildLocal(RectTransform rt, CanvasGroup cg,
        Vector2 fromPos, Vector2 toPos, float fromA, float toA, float dur)
    {
        float t = 0f;
        while (t < dur)
        {
            t += Time.unscaledDeltaTime;
            float k = childEase.Evaluate(Mathf.Clamp01(t / dur));

            // X는 고정(원래 위치), Y만 보간
            float newY = Mathf.LerpUnclamped(fromPos.y, toPos.y, k);
            float newX = lockXDuringAnim ? toPos.x : Mathf.LerpUnclamped(fromPos.x, toPos.x, k);

            var lp = rt.localPosition;
            rt.localPosition = new Vector3(newX, newY, lp.z);
            cg.alpha = Mathf.LerpUnclamped(fromA, toA, k);
            yield return null;
        }
        var lp2 = rt.localPosition;
        rt.localPosition = new Vector3(toPos.x, toPos.y, lp2.z);
        cg.alpha = toA;
    }

    // ===== 자연 높이 측정(레이아웃 없어도 대략 추정) =====
    float MeasureSubPanelHeightSafe()
    {
        bool wasActive = itemSubPanel.gameObject.activeSelf;
        itemSubPanel.gameObject.SetActive(true);

        float h = 0f;
        var vlg = itemSubPanel.GetComponent<VerticalLayoutGroup>();
        if (vlg)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(itemSubPanel);
            h = LayoutUtility.GetPreferredHeight(itemSubPanel);
        }
        else
        {
            if (itemSubPanel.childCount > 0)
            {
                Vector3[] corners = new Vector3[4];
                bool hasAny = false;
                float minY = 0f, maxY = 0f;

                for (int i = 0; i < itemSubPanel.childCount; i++)
                {
                    var child = itemSubPanel.GetChild(i) as RectTransform;
                    if (!child || !child.gameObject.activeInHierarchy) continue;

                    child.GetWorldCorners(corners);
                    for (int k = 0; k < 4; k++)
                    {
                        Vector3 local = itemSubPanel.InverseTransformPoint(corners[k]);
                        if (!hasAny) { minY = maxY = local.y; hasAny = true; }
                        else { if (local.y < minY) minY = local.y; if (local.y > maxY) maxY = local.y; }
                    }
                }
                if (hasAny) h = Mathf.Abs(maxY - minY);
            }
        }

        if (!wasActive) itemSubPanel.gameObject.SetActive(false);
        return h;
    }

    // ===== 콘텐츠 전환(겹침 방지: 하나만 켜기) =====
    void ShowOnly(GameObject target)
    {
        designContent.SetActive(target == designContent);
        yarnContent.SetActive(target == yarnContent);
        cottonContent.SetActive(target == cottonContent);
        decoContent.SetActive(target == decoContent);
    }

    void ShowDesignContent()
    {
        ShowOnly(designContent);
        SetScrollContent(designContent);

        // 디자인 클릭 시 자동 접힘
        SetItemAccordion(false);
    }

    void ShowMaterialSub(GameObject target)
    {
        ShowOnly(target);
        _lastMaterial = target;
        SetScrollContent(target);

        if (!itemExpanded) SetItemAccordion(true); // 자동 펼침 원치 않으면 제거
    }

    void SetScrollContent(GameObject go)
    {
        var rt = go.GetComponent<RectTransform>();
        scrollRect.content = rt;

        LayoutRebuilder.ForceRebuildLayoutImmediate(rt);
        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 1f;
    }
}
