using UnityEngine;

[CreateAssetMenu(fileName = "NewInterior", menuName = "ScriptableObject/InteriorScript")]
public class InteriorScript : ScriptableObject
{
    public string interiorName;
    public InteriorType interiorType;
    public Sprite image;
    public GameObject prefab; // 생성할 프리팹

    [Header("���� ����")]
    public int value;

    [Header("�̺��� á�� �� ����")]
    public Sprite fullImage;

    [Header("���� �� �ܰ�, ���� �ƴ� ��� NONE")]
    public WorkType workType;
}