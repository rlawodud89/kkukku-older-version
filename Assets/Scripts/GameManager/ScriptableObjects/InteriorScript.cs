using UnityEngine;

[CreateAssetMenu(fileName = "NewInterior", menuName = "ScriptableObject/InteriorScript")]
public class InteriorScript : ScriptableObject
{
    public string interiorName;
    public InteriorType interiorType;
    public Sprite image;
    public GameObject prefab; // 생성할 프리팹

    [Header("아이템 구매가")]
    public int value;

    [Header("테이블인 경우 지정, 아니면 NONE")]
    public TableType roominteriorType;

    [Header("이불장 찼을 때 사진")]
    public Sprite fullImage;

    [Header("직원이면 단계 지정, 아니면 NONE")]
    public WorkType workType;

    
}