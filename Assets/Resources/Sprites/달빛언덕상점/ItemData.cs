using UnityEngine;

[CreateAssetMenu(menuName = "Shop/Item Data")]
public class ItemData : ScriptableObject
{
    [Header("공통")]
    public string displayName;
    public Sprite icon;
    public int price;
    public bool isGold; // 일반 재화 사용하면 true, 월석 사용하면 false

    [Header("수량이 필요한 일반 아이템인가?")]
    public bool useQuantity = true;
    public int defaultQty = 1;
    public int minQty = 1;
    public int maxQty = 99;

    [Header("고용 카드일 경우")]
    public GameObject recruitPrefab;     // 고용할 유닛 프리팹 (없으면 일반 카드)
    public string description;
}
