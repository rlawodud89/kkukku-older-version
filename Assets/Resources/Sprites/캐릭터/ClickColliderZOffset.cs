// 클릭 전용 콜라이더를 살짝 카메라 쪽으로
using UnityEngine;

[ExecuteAlways]
public class ClickColliderZOffset : MonoBehaviour
{
    [Tooltip("음수면 카메라 쪽 (기본 -0.02)")]
    public float localZOffset = -0.02f;

    void OnEnable() => Apply();
    void OnValidate() => Apply();

    void Apply()
    {
        var t = transform;
        var p = t.localPosition;
        p.z = localZOffset;
        t.localPosition = p;
    }
}
