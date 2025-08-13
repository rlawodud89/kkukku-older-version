using UnityEngine;

public class NavPoint : MonoBehaviour
{
    public enum Face { Auto, LeftDown, RightUp, LeftUp, RightDown } // 아이소 4방
    [Header("도착시 바라볼 방향 (Auto=이동 방향 유지)")]
    public Face face = Face.Auto;

    [Header("겹쳐 서도 되는가? (Start/문 포인트는 체크)")]
    public bool allowOverlap = false;

    [Header("접근 오프셋(셀 기준). 비우면 기본 4방(-x, -y, +y, +x)")]
    public Vector3Int[] approachOffsets;

    public Vector3Int[] GetOffsetsOrDefault() =>
        (approachOffsets != null && approachOffsets.Length > 0)
        ? approachOffsets
        : new Vector3Int[] { new(-1, 0, 0), new(0, -1, 0), new(0, 1, 0), new(1, 0, 0) };
}
