using UnityEngine;

[CreateAssetMenu(fileName = "NewInterior", menuName = "ScriptableObject/InteriorScript")]
public class InteriorScript : ScriptableObject
{
    public string interiorName;
    public InteriorType interiorType;
    public Sprite image;

    [Header("구매 가격")]
    public int value;

    [Header("이불장 찼을 때 사진")]
    public Sprite fullImage;

    [Header("직원 일 단계, 직원 아닌 경우 NONE")]
    public WorkType workType;
}