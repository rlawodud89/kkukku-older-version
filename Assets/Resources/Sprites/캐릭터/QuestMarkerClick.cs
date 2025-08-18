// QuestMarkerClick.cs (새 스크립트, 마커 오브젝트에 부착)
using UnityEngine;

public class QuestMarkerClick : MonoBehaviour
{
    public AStarMover owner;

    void Awake()
    {
        if (!owner) owner = GetComponentInParent<AStarMover>();
        // Collider 없으면 자동 추가(2D 기준)
        if (!TryGetComponent<Collider2D>(out var _)) gameObject.AddComponent<BoxCollider2D>();
    }

    void OnMouseDown()
    {
        if (owner && owner.questMode) owner.AcceptQuest();
    }
}
