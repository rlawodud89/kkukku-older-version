using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteAlways]
public class FlowLayoutGroup : LayoutGroup
{
    [SerializeField] float spacingX = 20f;
    [SerializeField] float spacingY = 20f;

    public float SpacingX { get => spacingX; set { spacingX = value; SetDirty(); } }
    public float SpacingY { get => spacingY; set { spacingY = value; SetDirty(); } }

    public override void CalculateLayoutInputHorizontal()
    {
        base.CalculateLayoutInputHorizontal();
        CalcPositions();
    }
    public override void CalculateLayoutInputVertical() { }
    public override void SetLayoutHorizontal() { }
    public override void SetLayoutVertical() { }

    // ---------------- core ----------------
    void CalcPositions()
    {
        float parentW = rectTransform.rect.width;
        float usableW = parentW - padding.horizontal;

        // 첫 활성 자식의 폭을 샘플로 취득
        float cardW = 0;
        foreach (var c in rectChildren)
            if (c.gameObject.activeInHierarchy) { cardW = LayoutUtility.GetPreferredSize(c, 0); break; }

        if (cardW <= 0)                           // 빈 레이아웃 보호
        {
            SetLayoutInputForAxis(parentW, parentW, -1, 0);
            SetLayoutInputForAxis(padding.vertical, padding.vertical, -1, 1);
            return;
        }

        // ① 한 줄에 들어갈 카드 수(col) 계산
        int col = Mathf.Max(1, Mathf.FloorToInt((usableW + spacingX) / (cardW + spacingX)));

        // ② “가득 찬 줄”의 총폭 → 고정 센터 오프셋
        float fullRowW = cardW * col + spacingX * (col - 1);
        float centerPadX = Mathf.Max(0, (usableW - fullRowW) * 0.5f);

        // ③ 배치 루프
        float x = padding.left + centerPadX;
        float y = padding.top;
        float rowH = 0;
        int colIdx = 0;

        foreach (var child in rectChildren)
        {
            if (!child.gameObject.activeInHierarchy) continue;

            float w = cardW;                                      // 폭 동일 가정
            float h = LayoutUtility.GetPreferredSize(child, 1);

            if (colIdx >= col)                                    // 새 줄
            {
                colIdx = 0;
                x = padding.left + centerPadX;
                y += rowH + spacingY;
                rowH = 0;
            }

            SetChildAlongAxis(child, 0, x, w);
            SetChildAlongAxis(child, 1, y, h);

            x += w + spacingX;
            rowH = Mathf.Max(rowH, h);
            colIdx++;
        }

        float totalH = y + rowH + padding.bottom;
        SetLayoutInputForAxis(parentW, parentW, -1, 0);
        SetLayoutInputForAxis(totalH, totalH, -1, 1);
    }

    // ------------- boilerplate -------------
#if UNITY_EDITOR
    protected override void OnValidate() => SetDirty();
#endif
    protected override void OnEnable() { base.OnEnable(); SetDirty(); }
    protected override void OnTransformChildrenChanged() => SetDirty();
    protected override void OnRectTransformDimensionsChange() => SetDirty();
    void SetDirty()
    {
        if (IsActive())
            LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
    }
}
